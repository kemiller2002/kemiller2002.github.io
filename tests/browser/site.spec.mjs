import { expect, test } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";

const base = process.env.SITE_BASE_URL ?? "http://127.0.0.1:4001";
const pages = [
  "/",
  "/blog/",
  "/echelon-systems/",
  "/talks.html",
  "/about/",
  "/contact/",
  "/2026/02/01/ai-cant-do.html",
];
const widths = [1280, 768, 390, 320];
const wcag = ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"];

async function open(page, path, width, height = 900) {
  await page.setViewportSize({ width, height });
  await page.goto(base + path, { waitUntil: "domcontentloaded" });
}

async function overflow(page) {
  return page.evaluate(() =>
    Math.max(document.documentElement.scrollWidth, document.body.scrollWidth) -
    document.documentElement.clientWidth,
  );
}

for (const path of pages) {
  for (const width of widths) {
    test(path + " is contained at " + width + "px", async ({ page }) => {
      await open(page, path, width);
      expect(await overflow(page), path + " scrolls horizontally at " + width + "px").toBeLessThanOrEqual(1);
    });
  }

  test(path + " has no detectable WCAG 2.2 A/AA violations", async ({ page }) => {
    for (const width of [1280, 390]) {
      await open(page, path, width);
      const results = await new AxeBuilder({ page }).withTags(wcag).analyze();
      expect(results.violations, JSON.stringify(results.violations, null, 2)).toEqual([]);
    }
  });
}

test("primary navigation keeps usable touch targets on narrow screens", async ({ page }) => {
  for (const width of [390, 320]) {
    await open(page, "/", width);
    const links = page.locator(".ef-site-nav__link, .ef-site-header__brand, .ef-button");
    const count = await links.count();
    expect(count).toBeGreaterThan(5);

    for (let index = 0; index < count; index += 1) {
      const link = links.nth(index);
      await expect(link).toBeVisible();
      const box = await link.boundingBox();
      expect(box.height, (await link.textContent()) + " is shorter than 44px").toBeGreaterThanOrEqual(43.5);
      expect(box.width).toBeGreaterThanOrEqual(24);
    }
  }
});

test("skip link is first, becomes visible, and moves focus to main", async ({ page }) => {
  await open(page, "/", 1280);
  await page.keyboard.press("Tab");
  const skip = page.locator(".ef-skip-link");
  await expect(skip).toBeFocused();
  const box = await skip.boundingBox();
  expect(box.y).toBeGreaterThanOrEqual(0);
  await page.keyboard.press("Enter");
  await expect(page.locator("#main-content")).toBeFocused();
});

test("keyboard focus remains visibly delineated", async ({ page }) => {
  await open(page, "/", 1280);
  let checked = 0;
  for (let index = 0; index < 24; index += 1) {
    await page.keyboard.press("Tab");
    const state = await page.evaluate(() => {
      const element = document.activeElement;
      const style = getComputedStyle(element);
      return {
        body: element === document.body,
        skip: element.matches(".ef-skip-link"),
        outlineStyle: style.outlineStyle,
        outlineWidth: parseFloat(style.outlineWidth),
      };
    });
    if (state.body) break;
    if (state.skip) continue;
    expect(state.outlineStyle).toBe("solid");
    expect(state.outlineWidth).toBeGreaterThanOrEqual(2);
    checked += 1;
  }
  expect(checked).toBeGreaterThan(8);
});

test("200% text and WCAG text spacing do not create horizontal overflow", async ({ page }) => {
  for (const path of ["/", "/2026/02/01/ai-cant-do.html"]) {
    await open(page, path, 1280);
    await page.addStyleTag({ content: "html { font-size: 200% !important; }" });
    expect(await overflow(page), path + " overflows at 200% text").toBeLessThanOrEqual(1);
    await open(page, path, 390);
    await page.addStyleTag({
      content: "* { line-height: 1.5 !important; letter-spacing: 0.12em !important; word-spacing: 0.16em !important; } p { margin-block-end: 2em !important; }",
    });
    expect(await overflow(page), path + " overflows with text-spacing overrides").toBeLessThanOrEqual(1);
  }
});

test("reduced motion removes primary-action movement", async ({ page }) => {
  await page.emulateMedia({ reducedMotion: "reduce" });
  await open(page, "/", 1280);
  const button = page.locator('.ef-hero .ef-button[data-ef-variant="primary"]');
  await button.hover();
  const state = await button.evaluate((element) => ({
    duration: getComputedStyle(element).transitionDuration,
    transform: getComputedStyle(element).transform,
  }));
  expect(state.duration.split(",").every((duration) =>
    parseFloat(duration) * (duration.trim().endsWith("ms") ? 1 : 1000) < 1
  )).toBe(true);
  expect(state.transform).toBe("none");
});

test("forced colors preserve visible boundaries", async ({ page }) => {
  await page.emulateMedia({ forcedColors: "active" });
  await open(page, "/", 1280);
  for (const selector of ['.ef-hero .ef-button[data-ef-variant="primary"]', ".ef-site-footer"]) {
    const border = await page.locator(selector).first().evaluate(
      (element) => getComputedStyle(element).borderTopStyle,
    );
    expect(border, selector + " loses its boundary in forced colors").toBe("solid");
  }
});

test("print output removes site navigation", async ({ page }) => {
  await page.emulateMedia({ media: "print" });
  await open(page, "/", 1280);
  expect(await page.locator(".ef-site-nav").evaluate((element) => getComputedStyle(element).display)).toBe("none");
});

test("core navigation remains useful with JavaScript disabled", async ({ browser }) => {
  const context = await browser.newContext({ javaScriptEnabled: false });
  const page = await context.newPage();
  await page.goto(base + "/", { waitUntil: "domcontentloaded" });
  await expect(page.locator("h1")).toBeVisible();
  await expect(page.locator('a[href="/blog/"]')).toBeVisible();
  await expect(page.locator('a[href="/echelon-systems/"]')).toBeVisible();
  await context.close();
});
