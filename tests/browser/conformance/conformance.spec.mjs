// UI conformance suite for kevinmmiller.us against
// visual-engineering/UI-LAYOUT-INTERACTION@1.0.0. Controls are resolved by
// role and accessible name, waits are web-first, and every action compares
// observed pre/post state. No coordinates and no fixed sleeps.
import { readFileSync } from "node:fs";
import { expect, test } from "@playwright/test";
import {
  CONTRACT_WIDTHS,
  expectActionOutcome,
  expectLabelIntact,
  expectNoOverflow,
  expectPeerAlignment,
  inventoryActions,
  recordTabSequence,
  scaleDefaultText,
  settleFontsAndFrames,
  wasFocusedDuringSequence
} from "./forma-conformance/index.mjs";
import { CHROME, DUPLICATE_TITLES, KNOWN_CONTENT_NAMES, NEUTRAL, ROUTES, SURFACES, declaredActionsFor, headingOf } from "./surfaces.mjs";

const NAV_NAMES = ["Home", "Writing", "Work", "Speaking", "About", "Contact"];
const escape = value => value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
const annotate = (type, description) => test.info().annotations.push({ type, description });
const pathPattern = destination => new RegExp(`^https?://[^/]+${escape(destination)}$`);
const nameMatches = (declared, actual) => (declared instanceof RegExp ? declared.test(actual) : declared === actual);

const SCOPES = Object.freeze({
  banner: page => page.getByRole("banner"),
  primary: page => page.getByRole("navigation", { name: "Primary" }),
  footer: page => page.getByRole("navigation", { name: "Footer" }),
  main: page => page.getByRole("main")
});
const scoped = (page, scope) => (scope ? SCOPES[scope](page) : page);
const control = (page, action) => scoped(page, action.scope).getByRole(action.control.role, { name: action.control.name, exact: typeof action.control.name === "string" });

// Every generated post, read from the generated sitemap.
const SITEMAP = readFileSync(new URL("../../../dist-v2/sitemap.xml", import.meta.url), "utf8");
const POSTS = [...SITEMAP.matchAll(/<loc>https?:\/\/[^/]+(\/\d{4}\/[^<]+)<\/loc>/g)].map(match => match[1]);

// --- ULI-L05/L06/L09 and ULI-A10 on every route at every contract width ------

for (const width of CONTRACT_WIDTHS) {
  test.describe(`at ${width}px`, () => {
    test.use({ viewport: { width, height: 900 } });
    for (const route of ROUTES) {
      test(`${route.path} renders without overflow, with intact navigation labels`, async ({ page }) => {
        annotate("uli-surface", route.id);
        annotate("uli-surface", "chrome");
        await page.goto(route.path);
        await expect(page.getByRole("heading", { level: 1 })).toHaveText(route.heading);
        await expectNoOverflow(page);
        for (const name of NAV_NAMES) await expectLabelIntact(SCOPES.primary(page).getByRole("link", { name, exact: true }));
        await expect(SCOPES.primary(page).locator("[aria-current=page]")).toHaveCount(route.current ? 1 : 0);
        if (route.current) await expect(SCOPES.primary(page).getByRole("link", { name: route.current, exact: true })).toHaveAttribute("aria-current", "page");
      });

      test(`${route.path} renders only declared, uniquely named actions`, async ({ page }) => {
        annotate("uli-surface", route.id);
        await page.goto(route.path);
        await settleFontsAndFrames(page);
        const declared = declaredActionsFor(route);
        const rendered = await inventoryActions(page.locator("body"));
        const undeclared = rendered.filter(item => !declared.some(action => action.control.role === item.role && nameMatches(action.control.name, item.name)));
        expect(undeclared.map(item => `${item.role} "${item.name}" -> ${item.href}`), "ULI-A10: enabled actions without a declared contract").toEqual([]);
        expect(rendered.filter(item => !item.name), "ULI-A06: actions without an accessible name").toEqual([]);
      });
    }
  });

  test.describe(`at ${width}px with 200% text`, () => {
    test.use({ viewport: { width, height: 900 } });
    for (const route of ROUTES) {
      test(`${route.path} reflows without overflow`, async ({ page }) => {
        annotate("uli-surface", route.id);
        await page.goto(route.path);
        await scaleDefaultText(page, 2);
        await expectNoOverflow(page);
      });
    }
  });
}

// --- ULI-L04: one top rhythm for every page ---------------------------------

for (const width of CONTRACT_WIDTHS) {
  test(`every route starts its content at the same distance below the header at ${width}px`, async ({ page }) => {
    await page.setViewportSize({ width, height: 900 });
    const measure = async route => {
      await page.goto(route.path);
      await settleFontsAndFrames(page);
      const [header, eyebrow] = await Promise.all([
        page.getByRole("banner").boundingBox(),
        page.getByRole("main").locator(".ef-eyebrow").first().boundingBox()
      ]);
      return { path: route.path, gap: Math.round(eyebrow.y - (header.y + header.height)) };
    };
    const observed = await ROUTES.reduce(async (acc, route) => [...(await acc), await measure(route)], Promise.resolve([]));
    ROUTES.forEach(route => annotate("uli-surface", route.id));
    expect(observed).toEqual(observed.map(item => ({ path: item.path, gap: observed[0].gap })));
  });
}

// --- Destinations of every declared action and every class instance ---------

for (const route of ROUTES) {
  test.describe(`${route.path} link destinations`, () => {
    for (const action of declaredActionsFor(route)) {
      test(`${action.surfaceId}/${action.id} is valid`, async ({ page, request }) => {
        annotate("uli-destination", `${action.surfaceId}/${action.id}`);
        await page.goto(route.path);
        const instances = control(page, action);
        if (action.kind === "class") {
          const hrefs = await instances.evaluateAll(elements => elements.map(element => element.getAttribute("href")));
          expect(hrefs.length, `${action.id} has instances on ${route.path}`).toBeGreaterThan(0);
          for (const href of hrefs) {
            if (href.startsWith("/")) expect((await request.get(href)).status(), href).toBe(200);
            else expect(new URL(href).protocol, href).toBe("https:");
          }
          return;
        }
        await expect(instances).toHaveCount(1);
        await expect(instances).toHaveAttribute("href", action.destination);
        if (action.kind === "navigate") expect((await request.get(action.destination)).status(), action.destination).toBe(200);
        if (action.kind === "anchor") await expect(page.locator(action.destination)).toHaveCount(1);
        if (action.kind === "external") expect(new URL(action.destination).protocol).toBe("https:");
      });
    }
  });
}

// --- ULI-A02/A06: navigation outcomes ----------------------------------------

const outcomeActions = SURFACES.flatMap(surface => surface.actions
  .filter(action => action.kind === "navigate" || action.kind === "anchor" || action.kind === "class")
  .map(action => ({ surface, action })));

const startFor = (surface, action) => {
  if (surface.id === "chrome") return action.kind === "anchor" ? "/about/" : NEUTRAL;
  return ROUTES.find(route => route.id === surface.id).path;
};

for (const width of [390, 1280]) {
  test.describe(`navigation outcomes at ${width}px`, () => {
    test.use({ viewport: { width, height: 900 } });
    for (const { surface, action } of outcomeActions) {
      test(`${surface.id}: ${action.id}`, async ({ page }) => {
        annotate("uli-action", `${surface.id}/${action.id}`);
        const start = startFor(surface, action);
        await page.goto(start);
        if (action.kind === "class") {
          // The first rendered instance stands for the class; its own href is
          // the declared destination.
          const first = control(page, action).first();
          const name = await first.evaluate(element => element.getAttribute("aria-label") ?? element.textContent.trim());
          const href = await first.getAttribute("href");
          if (!href.startsWith("/")) {
            await expect(first).toHaveAttribute("href", /^https:\/\//);
            return;
          }
          const report = await expectActionOutcome(page, { ruleId: "ULI-A02", action: { role: "link", name, scope: scoped(page, action.scope) }, outcome: { kind: "url", expected: pathPattern(href) } });
          expect(report.status).toBe("PASS");
          await expect(page.getByRole("heading", { level: 1 })).toBeVisible();
          return;
        }
        if (action.kind === "external") return;
        await expectActionOutcome(page, {
          ruleId: "ULI-A02",
          action: { ...action.control, scope: action.scope ? scoped(page, action.scope) : undefined },
          outcome: { kind: "url", expected: pathPattern(action.kind === "anchor" ? `${start}${action.destination}` : action.destination) }
        });
        if (action.kind === "anchor") await expect(page.locator(action.destination)).toBeInViewport();
        if (action.kind === "navigate" && headingOf(action.destination)) await expect(page.getByRole("heading", { level: 1 })).toHaveText(headingOf(action.destination));
      });
    }
  });
}

// External links: the destination is the observable outcome.
for (const surface of SURFACES) {
  for (const action of surface.actions.filter(item => item.kind === "external")) {
    test(`${surface.id}: ${action.id} points at ${action.destination}`, async ({ page }) => {
      annotate("uli-action", `${surface.id}/${action.id}`);
      await page.goto(ROUTES.find(route => route.id === surface.id).path);
      await expect(control(page, action)).toHaveAttribute("href", action.destination);
    });
  }
}

// --- ULI-L01/L10: declared alignment groups ----------------------------------

for (const surface of SURFACES) {
  for (const group of surface.groups) {
    for (const width of group.widths) {
      test(`${surface.id}: ${group.id} aligns at ${width}px`, async ({ page }) => {
        annotate("uli-group", `${surface.id}/${group.id}@${width}`);
        await page.setViewportSize({ width, height: 900 });
        await page.goto(surface.id === "chrome" ? NEUTRAL : ROUTES.find(route => route.id === surface.id).path);
        await expectPeerAlignment(page, { ruleId: group.ruleId, id: group.id, edge: group.edge, peers: group.peers, scope: group.scope ? scoped(page, group.scope) : undefined });
      });
    }
  }
}

// --- Every generated post at the narrowest width ----------------------------

test.describe("every post at 320px", () => {
  test.use({ viewport: { width: 320, height: 900 } });
  test("the sitemap lists the generated posts", () => {
    annotate("uli-surface", "post");
    expect(POSTS.length).toBeGreaterThan(100);
  });
  for (const path of POSTS) {
    test(`${path} has no overflow and unambiguous action names`, async ({ page }) => {
      annotate("uli-surface", "post");
      await page.goto(path);
      await expect(page.getByRole("heading", { level: 1 })).toBeVisible();
      await expectNoOverflow(page);
      const rendered = await inventoryActions(page.locator("body"));
      expect(rendered.filter(item => !item.name), "ULI-A06: actions without an accessible name").toEqual([]);
      const destinations = rendered.reduce((acc, item) => ({ ...acc, [item.name]: [...new Set([...(acc[item.name] ?? []), item.href])] }), {});
      const ambiguous = Object.entries(destinations)
        .filter(([name, hrefs]) => hrefs.length > 1 && !DUPLICATE_TITLES.some(title => name === `Read: ${title}`) && !(KNOWN_CONTENT_NAMES[path] ?? []).includes(name))
        .map(([name, hrefs]) => `${name} -> ${hrefs.join(" | ")}`);
      expect(ambiguous, "ULI-A06: one name, several destinations").toEqual([]);
    });
  }
});

// The writing archive is the one page that lists every post.
test("the writing archive names each destination once, apart from declared duplicate posts", async ({ page }) => {
  annotate("uli-surface", "writing");
  await page.goto("/blog/");
  const rendered = await inventoryActions(page.locator("body"));
  const destinations = rendered.reduce((acc, item) => ({ ...acc, [item.name]: [...new Set([...(acc[item.name] ?? []), item.href])] }), {});
  const ambiguous = Object.entries(destinations).filter(([, hrefs]) => hrefs.length > 1).map(([name]) => name);
  expect(ambiguous.sort()).toEqual(DUPLICATE_TITLES.map(title => `Read: ${title}`).sort());
});

// --- ULI-A08: keyboard reachability of the chrome ----------------------------

test("every chrome action is reachable by keyboard", async ({ page }) => {
  annotate("uli-surface", "chrome");
  await page.goto(NEUTRAL);
  await recordTabSequence(page, 40);
  for (const action of CHROME) {
    expect(await wasFocusedDuringSequence(control(page, action)), action.id).toBe(true);
  }
});
