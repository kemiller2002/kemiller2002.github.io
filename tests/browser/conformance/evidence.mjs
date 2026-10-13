// Pure mapping from Playwright results to evidence that validates against
// visual-engineering framework/contracts/ui-conformance.schema.json
// (standardVersion 1.0.0). Lack of evidence is "unknown", never "pass".
import { CONTRACT_WIDTHS } from "./forma-conformance/evaluate.mjs";
import { STANDARD, SURFACES } from "./surfaces.mjs";

const FAILED = new Set(["failed", "timedOut", "interrupted"]);

// The schema's relation vocabulary (1.0.0) has no left/right edge, although
// the Forma helpers measure them. Such groups are still run and reported, in
// the surface evidence, rather than mislabelled with an allowed relation.
const SCHEMA_RELATIONS = new Set(["top-edge", "text-baseline", "centerline", "bottom-edge"]);

// Results recorded under one annotation key -> contract status.
export const statusOf = results => {
  if (results.length === 0) return "unknown";
  if (results.some(result => FAILED.has(result.status))) return "fail";
  if (results.every(result => result.status === "passed")) return "pass";
  return "unknown";
};

const describe = result => `${result.status}: ${result.title} (${result.location})`;

const indexResults = results => results.reduce((index, result) => result.annotations.reduce((acc, { type, description }) => {
  const key = `${type}:${description}`;
  return { ...acc, [key]: [...(acc[key] ?? []), result] };
}, index), {});

const nameText = name => (name instanceof RegExp ? `/${name.source}/` : name);

const UNKNOWN_SCOPE = Object.freeze([
  "Translated and right-to-left text are not exercised (the site ships English only).",
  "Long labels beyond the shipped content are not injected.",
  "Browser page zoom is approximated by the contract widths plus a 200% default text size; WebKit and Firefox are not exercised by this suite.",
  "Availability of external destinations is not verified; only their href is asserted."
]);

const SURFACE_UNKNOWN = Object.freeze({
  post: ["Only /2026/02/01/ai-cant-do.html is exercised at every width; every other post is checked at 320px for overflow and action names.", "Seven legacy posts are published twice under the same title (content debt), so their \"Read: <title>\" names repeat for two destinations; two of them also link two different examples as \"Running Example\", and three other legacy posts link two destinations as \"here\"."],
  home: ["The closing action row wraps 2+1 at 320px; no alignment relation is declared at that width."],
  chrome: ["Primary navigation wraps into two rows below 768px; centerline alignment is declared only at 768px and wider, label integrity at every width."]
});

const buildAction = (surface, index) => action => {
  const results = [...(index[`uli-action:${surface.id}/${action.id}`] ?? []), ...(index[`uli-destination:${surface.id}/${action.id}`] ?? [])];
  return {
    id: action.id,
    control: { role: action.control.role, name: nameText(action.control.name) },
    intent: action.intent,
    expectedOutcome: action.expectedOutcome,
    status: statusOf(results),
    evidence: results.map(describe),
    ...(action.resetStrategy ? { resetStrategy: action.resetStrategy } : {})
  };
};

const buildGroup = (surface, index) => group => {
  const results = group.widths.flatMap(width => index[`uli-group:${surface.id}/${group.id}@${width}`] ?? []);
  return {
    id: group.id,
    relation: group.relation,
    toleranceCssPx: 1,
    targets: group.peers.map(peer => ({ role: peer.role, name: nameText(peer.name) })),
    rationale: `${group.ruleId}: ${group.rationale} (declared at ${group.widths.join(", ")}px)`,
    evidence: results.map(describe),
    status: statusOf(results)
  };
};

const surfaceStatus = statuses => {
  if (statuses.includes("fail")) return "fail";
  if (statuses.length > 0 && statuses.every(status => status === "pass")) return "pass";
  return "unknown";
};

const buildSurface = index => surface => {
  const own = index[`uli-surface:${surface.id}`] ?? [];
  const actions = surface.actions.map(buildAction(surface, index));
  const groups = surface.groups.map(buildGroup(surface, index));
  const statuses = [statusOf(own), ...actions.map(action => action.status), ...groups.map(group => group.status)];
  return {
    id: surface.id,
    path: surface.path,
    kind: surface.kind,
    status: surfaceStatus(statuses),
    viewportWidths: [...CONTRACT_WIDTHS],
    alignmentGroups: groups.filter(group => SCHEMA_RELATIONS.has(group.relation))
      .map(({ status, ...group }) => ({ ...group, evidence: [`status: ${status}`, ...group.evidence] })),
    actions,
    evidence: [
      ...own.map(describe),
      ...groups.filter(group => !SCHEMA_RELATIONS.has(group.relation)).map(group =>
        `alignment group ${group.id} (${group.relation}; not expressible in the schema 1.0.0 relation enum) status ${group.status}: ${group.rationale}; peers ${group.targets.map(target => `${target.role} "${target.name}"`).join(", ")}; ${group.evidence.length} measurement(s): ${group.evidence.join("; ")}`)
    ],
    unknownScope: [...UNKNOWN_SCOPE, ...(SURFACE_UNKNOWN[surface.id] ?? [])]
  };
};

export const buildEvidence = ({ results, revision, repository }) => ({
  standardVersion: STANDARD.version,
  repository,
  revision,
  classification: "site-ui",
  surfaces: SURFACES.map(buildSurface(indexResults(results))),
  exceptions: [],
  notes: `Contract ${STANDARD.id}@${STANDARD.version} (${STANDARD.source}). Helpers: @echelon-foundry/forma-conformance 0.1.0 vendored from kemiller2002/forma@f66dba6. Generated by tests/browser npm run ui:conformance against dist-v2; statuses derive only from executed Playwright tests.`
});

export const totals = evidence => {
  const items = evidence.surfaces.flatMap(surface => [surface, ...surface.actions]);
  return items.reduce((acc, item) => ({ ...acc, [item.status]: (acc[item.status] ?? 0) + 1 }), {});
};
