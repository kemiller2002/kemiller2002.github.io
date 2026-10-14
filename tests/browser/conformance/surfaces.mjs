// Declared conformance inventory for visual-engineering/UI-LAYOUT-INTERACTION@1.0.0
// (pinned at visual-engineering 8da8fbc) for kevinmmiller.us, the F#-generated
// static site in dist-v2/. Data only: routes, their enabled actions and the
// alignment groups the composition intends. Content-driven index links (one
// per post, project or role) are declared as action classes: every rendered
// instance must match the class and its own href is the expected destination.

export const STANDARD = Object.freeze({ id: "visual-engineering/UI-LAYOUT-INTERACTION", version: "1.0.0", source: "kemiller2002/visual-engineering@8da8fbc" });

const link = name => ({ role: "link", name });

const navigate = (id, name, destination, scope) =>
  ({ id, control: link(name), kind: "navigate", destination, scope, intent: `Navigate to ${destination}`, expectedOutcome: `URL becomes ${destination} and that page's level-one heading is shown` });

const jump = (id, name, hash) =>
  ({ id, control: link(name), kind: "anchor", destination: hash, intent: `Move to ${hash}`, expectedOutcome: `URL hash becomes ${hash} and the main landmark is in view` });

// A class of content-driven links; each instance's href is its destination.
const indexClass = (id, pattern, intent, expectedOutcome) =>
  ({ id, control: link(pattern), kind: "class", intent, expectedOutcome });

export const ROUTES = Object.freeze([
  { id: "home", path: "/", current: "Home", heading: "I work on software systems where the difficult problem is understanding what is actually true." },
  { id: "writing", path: "/blog/", current: "Writing", heading: "Engineering, systems, leadership, and the evidence behind difficult decisions." },
  { id: "work", path: "/echelon-systems/", current: "Work", heading: "A connected engineering ecosystem for making state, authority, evidence, quality, and presentation explicit." },
  { id: "speaking", path: "/talks.html", current: "Speaking", heading: "Systems, diagnostics, and the judgment required when the easy story is incomplete." },
  { id: "about", path: "/about/", current: "About", heading: "I build and diagnose software systems where context matters." },
  { id: "contact", path: "/contact/", current: "Contact", heading: "Bring me the version of the problem people are avoiding." },
  { id: "speaker-bio", path: "/speaker-bio.html", current: "Speaking", heading: "Kevin M. Miller" },
  { id: "not-found", path: "/404.html", heading: "That page is not here." },
  { id: "post", path: "/2026/02/01/ai-cant-do.html", current: "Writing", heading: "When AI Fails, It’s Usually a Management Problem", representative: true }
]);

const headingOf = path => ROUTES.find(route => route.path === path)?.heading;

export const NEUTRAL = "/speaker-bio.html";

// Header (primary navigation) and footer render on every page.
export const CHROME = Object.freeze([
  jump("skip-link", "Skip to main content", "#main-content"),
  navigate("brand-home", "Kevin M. Miller", "/", "banner"),
  ...[["Home", "/"], ["Writing", "/blog/"], ["Work", "/echelon-systems/"], ["Speaking", "/talks.html"], ["About", "/about/"], ["Contact", "/contact/"]]
    .map(([name, destination]) => navigate(`nav-${name.toLowerCase()}`, name, destination, "primary")),
  ...[["Writing", "/blog/"], ["Speaking", "/talks.html"], ["About", "/about/"], ["Contact", "/contact/"]]
    .map(([name, destination]) => navigate(`footer-${name.toLowerCase()}`, name, destination, "footer"))
]);

const READ = indexClass("read-post", /^Read: .+/, "Open the named post", "Each instance names its post and its href resolves to that post (HTTP 200); the first instance is clicked and lands on the post");

export const SURFACES = Object.freeze([
  { id: "chrome", path: "/* (shared header and footer on every page)", kind: "marketing", actions: CHROME, groups: [
    { id: "primary-nav", ruleId: "ULI-L10", relation: "centerline", edge: "center", widths: [768, 1280], peers: ["Home", "Writing", "Work", "Speaking", "About", "Contact"].map(link), scope: "primary", rationale: "Primary navigation is one row at 768px and wider (it wraps into two rows of three below; label integrity is asserted there)" },
    { id: "footer-nav", ruleId: "ULI-L10", relation: "centerline", edge: "center", widths: [320, 390, 768, 1280], peers: ["Writing", "Speaking", "About", "Contact"].map(link), scope: "footer", rationale: "Footer destinations share one row" }
  ] },
  { id: "home", path: "/", kind: "marketing", actions: [
    navigate("home-read-writing", "Read the writing", "/blog/", "main"),
    navigate("home-see-work", "See current work", "/echelon-systems/", "main"),
    navigate("home-all-writing", "View all writing", "/blog/", "main"),
    READ,
    indexClass("explore-project", /^Explore: .+/, "Open the named project", "Each instance names its project and links to an https destination"),
    indexClass("about-role", /^About: .+/, "Read about the named role on the About page", "Each instance names its role and resolves to /about/"),
    navigate("home-speaking", "Speaking", "/talks.html", "main"),
    navigate("home-about", "About", "/about/", "main"),
    navigate("home-contact", "Contact", "/contact/", "main")
  ], groups: [
    { id: "hero-actions-row", ruleId: "ULI-L10", relation: "top-edge", edge: "top", widths: [390, 768, 1280], peers: [link("Read the writing"), link("See current work")], rationale: "Hero calls to action share one row from 390px" },
    { id: "hero-actions-stack", ruleId: "ULI-L10", relation: "left-edge", edge: "left", widths: [320], peers: [link("Read the writing"), link("See current work")], rationale: "Hero calls to action stack on one left edge at 320px" },
    { id: "closing-actions-row", ruleId: "ULI-L10", relation: "top-edge", edge: "top", widths: [390, 768, 1280], peers: [link("Speaking"), link("About"), link("Contact")], scope: "main", rationale: "Closing actions share one row from 390px" }
  ] },
  { id: "writing", path: "/blog/", kind: "marketing", actions: [READ], groups: [] },
  { id: "work", path: "/echelon-systems/", kind: "marketing", actions: [
    indexClass("explore-system", /^Explore: .+/, "Open the named system's repository", "Each instance names its system and links to an https destination"),
    { ...navigate("work-echelon-foundry", "Explore Echelon Foundry", "https://echelonfoundry.com", "main"), kind: "external", intent: "Open echelonfoundry.com", expectedOutcome: "href is https://echelonfoundry.com (availability not asserted)" }
  ], groups: [] },
  { id: "speaking", path: "/talks.html", kind: "marketing", actions: [
    navigate("speaking-invite", "Invite Kevin to speak", "/contact/", "main"),
    indexClass("discuss-talk", /^Discuss this talk: .+/, "Discuss the named talk", "Each instance names its talk and resolves to /contact/")
  ], groups: [] },
  { id: "about", path: "/about/", kind: "marketing", actions: [
    navigate("about-writing", "Writing", "/blog/", "main"),
    navigate("about-work", "Work", "/echelon-systems/", "main"),
    navigate("about-contact", "Contact", "/contact/", "main")
  ], groups: [] },
  { id: "contact", path: "/contact/", kind: "marketing", actions: [
    ...[["Echelon Foundry", "https://echelonfoundry.com"], ["LinkedIn", "https://www.linkedin.com/in/kemiller2002"], ["GitHub", "https://github.com/kemiller2002/"]]
      .map(([name, href]) => ({ id: `contact-${name.toLowerCase().replace(/\s+/g, "-")}`, control: link(name), kind: "external", destination: href, scope: "main", intent: `Open ${href}`, expectedOutcome: `href is ${href} (availability not asserted)` }))
  ], groups: [] },
  { id: "speaker-bio", path: "/speaker-bio.html", kind: "marketing", actions: [
    navigate("bio-view-talks", "View talks", "/talks.html", "main"),
    navigate("bio-speaking-inquiries", "Speaking inquiries", "/contact/", "main")
  ], groups: [
    { id: "bio-actions-row", ruleId: "ULI-L10", relation: "top-edge", edge: "top", widths: [390, 768, 1280], peers: [link("View talks"), link("Speaking inquiries")], rationale: "Speaker-bio actions share one row from 390px" },
    { id: "bio-actions-stack", ruleId: "ULI-L10", relation: "left-edge", edge: "left", widths: [320], peers: [link("View talks"), link("Speaking inquiries")], rationale: "Speaker-bio actions stack on one left edge at 320px" }
  ] },
  { id: "not-found", path: "/404.html", kind: "marketing", actions: [
    navigate("404-home", "Go home", "/", "main"),
    navigate("404-writing", "Browse writing", "/blog/", "main")
  ], groups: [
    { id: "not-found-actions", ruleId: "ULI-L10", relation: "top-edge", edge: "top", widths: [320, 390, 768, 1280], peers: [link("Go home"), link("Browse writing")], rationale: "Recovery actions share one row" }
  ] },
  { id: "post", path: "/2026/02/01/ai-cant-do.html (representative of every post)", kind: "marketing", actions: [
    READ,
    indexClass("post-newer", /^Newer: .+/, "Open the next newer post", "Names the newer post and resolves (HTTP 200)"),
    indexClass("post-older", /^Older: .+/, "Open the next older post", "Names the older post and resolves (HTTP 200)"),
    navigate("post-archive", "Writing archive", "/blog/", "main")
  ], groups: [] }
]);

export const surfaceForRoute = route => SURFACES.find(surface => surface.id === route.id);

export const declaredActionsFor = route => [
  ...CHROME.map(action => ({ ...action, surfaceId: "chrome" })),
  ...(surfaceForRoute(route)?.actions ?? []).map(action => ({ ...action, surfaceId: route.id }))
];

export { headingOf };

// Legacy posts duplicated under two routes with the same title (pre-existing
// content debt, tracked in the Praxis backlog). Their "Read: <title>" names
// repeat for two destinations; every other name must map to one destination.
export const DUPLICATE_TITLES = Object.freeze([
  "The Coffee Queue: How Developers Solve Real Problems",
  "Looking back at C#",
  "Indy.Code()",
  "I'm out of Range? You're out of Range!",
  "Where's The Scope",
  "Non Failing Scripts",
  "The Book Of F#"
]);

// Historical post content that links two different examples with the same
// visible text (pre-existing content debt, published under two routes).
export const KNOWN_CONTENT_NAMES = Object.freeze({
  "/2014/03/20/Where-s-The-Scope.html": ["Running Example"],
  "/2014/03/20/wheres-the-scope.html": ["Running Example"],
  "/2016/03/21/Project-Oxford---Image-Text-Detection.html": ["here"],
  "/2015/04/25/Compile-Time-Stored-Procedures.html": ["here"],
  "/2015/03/30/i-thought-you-were-the-new-method.html": ["here"]
});
