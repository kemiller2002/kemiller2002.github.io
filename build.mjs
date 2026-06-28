import fs from "node:fs/promises";
import path from "node:path";

const rootDir = process.cwd();
const sourceDir = path.join(rootDir, "site-src");
const outputDir = path.join(rootDir, "docs");

const site = {
  title: "Kevin M. Miller",
  description: "Engineering leadership, modernization, AI, security, and decision-making under constraint.",
  url: "https://kevinmmiller.us",
};

const pages = [
  {
    slug: "index",
    title: site.title,
    source: path.join(sourceDir, "pages", "index.html"),
    output: "index.html",
    template: "default",
  },
  {
    slug: "about",
    title: `About | ${site.title}`,
    source: path.join(sourceDir, "pages", "about.html"),
    output: path.join("about", "index.html"),
    template: "default",
  },
  {
    slug: "blog",
    title: `Writing | ${site.title}`,
    source: path.join(sourceDir, "pages", "blog.html"),
    output: path.join("blog", "index.html"),
    template: "default",
  },
  {
    slug: "contact",
    title: `Contact | ${site.title}`,
    source: path.join(sourceDir, "pages", "contact.html"),
    output: path.join("contact", "index.html"),
    template: "default",
  },
  {
    slug: "404",
    title: `Not Found | ${site.title}`,
    source: path.join(sourceDir, "pages", "404.html"),
    output: "404.html",
    template: "default",
  },
  {
    slug: "talks",
    title: `Speaking | ${site.title}`,
    source: path.join(sourceDir, "pages", "talks.html"),
    output: "talks.html",
    template: "default",
  },
  {
    slug: "speaker-bio",
    source: path.join(sourceDir, "pages", "speaker-bio.html"),
    output: "speaker-bio.html",
    passthrough: true,
  },
];

async function main() {
  const posts = await loadPosts();
  await resetOutputDir();
  await copyStaticAssets();

  for (const page of pages) {
    if (page.passthrough) {
      const html = await fs.readFile(page.source, "utf8");
      await writeOutput(page.output, html);
      continue;
    }

    const content = await fs.readFile(page.source, "utf8");
    const html = renderPage(page, content, posts);
    await writeOutput(page.output, html);
  }

  for (const post of posts) {
    const html = renderPost(post);
    await writeOutput(post.outputPath, html);
  }
}

async function loadPosts() {
  const postsDir = path.join(sourceDir, "posts");
  const filenames = (await fs.readdir(postsDir))
    .filter((name) => name.endsWith(".md"))
    .sort();

  const posts = [];

  for (const filename of filenames) {
    const fullPath = path.join(postsDir, filename);
    const raw = await fs.readFile(fullPath, "utf8");
    const { data, content } = parseFrontMatter(raw);
    const meta = buildPostMeta(filename, data);
    const html = renderMarkdown(content);
    const excerpt = truncate(
      decodeHtmlEntities(stripTags(html)).replace(/\s+/g, " ").trim(),
      170,
    );

    posts.push({
      ...meta,
      sourcePath: fullPath,
      html,
      excerpt,
    });
  }

  return posts.sort((a, b) => b.dateValue - a.dateValue);
}

function buildPostMeta(filename, data) {
  const match = filename.match(/^(\d{4})-(\d{1,2})-(\d{1,2})-(.+)\.md$/);
  if (!match) {
    throw new Error(`Unsupported post filename format: ${filename}`);
  }

  const [, year, month, day, slug] = match;
  const paddedMonth = month.padStart(2, "0");
  const paddedDay = day.padStart(2, "0");
  const isoDate = `${year}-${paddedMonth}-${paddedDay}`;
  const title = data.title ?? slugToTitle(slug);
  const dateValue = new Date(`${isoDate}T00:00:00Z`);
  const urlPath = `/${year}/${paddedMonth}/${paddedDay}/${slug}.html`;

  return {
    filename,
    title,
    slug,
    dateValue,
    dateIso: isoDate,
    dateDisplay: formatDisplayDate(dateValue),
    urlPath,
    outputPath: path.join(year, paddedMonth, paddedDay, `${slug}.html`),
  };
}

function parseFrontMatter(raw) {
  if (!raw.startsWith("---\n")) {
    return { data: {}, content: raw };
  }

  const end = raw.indexOf("\n---\n", 4);
  if (end === -1) {
    return { data: {}, content: raw };
  }

  const frontMatter = raw.slice(4, end).trim();
  const content = raw.slice(end + 5).trimStart();
  const data = {};

  for (const line of frontMatter.split("\n")) {
    const match = line.match(/^([A-Za-z0-9_-]+):\s*(.*)$/);
    if (!match) {
      continue;
    }

    const [, key, value] = match;
    data[key] = value.replace(/^["']|["']$/g, "").trim();
  }

  return { data, content };
}

function renderPage(page, content, posts) {
  const recentPosts = posts.slice(0, 6);
  const htmlContent = content
    .replace("{{RECENT_POSTS}}", renderRecentPosts(recentPosts))
    .replace("{{ALL_POSTS}}", renderArchivePosts(posts));

  return renderShell({
    title: page.title,
    content: htmlContent,
    pageSlug: page.slug,
  });
}

function renderPost(post) {
  const content = `
    <div class="post-shell">
      <header class="post-header">
        <div class="post-kicker">Article</div>
        <h1 class="post-title">${escapeHtml(post.title)}</h1>
        <p class="post-date">${escapeHtml(post.dateDisplay)}</p>
      </header>

      <article class="post-body">
        ${post.html}
      </article>

      <footer class="post-footer">
        <a href="/blog/" class="back-link">&larr; Back to Writing</a>
      </footer>
    </div>
  `;

  return renderShell({
    title: `${post.title} | ${site.title}`,
    content,
    pageSlug: "post",
  });
}

function renderShell({ title, content, pageSlug }) {
  const nav = [
    { href: "/", label: "Home", slug: "index" },
    { href: "/blog/", label: "Writing", slug: "blog" },
    { href: "/talks.html", label: "Speaking", slug: "talks" },
    { href: "/about/", label: "About", slug: "about" },
    { href: "/contact/", label: "Contact", slug: "contact" },
  ];

  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>${escapeHtml(title)}</title>
  <meta name="description" content="${escapeHtml(site.description)}" />
  <link rel="canonical" href="${site.url}" />
  <link rel="stylesheet" href="/custom.css" />
</head>
<body class="page">
  <header class="site-header">
    <div class="site-header-inner">
      <a class="site-brand" href="/">Kevin M. Miller</a>
      <nav class="site-nav" aria-label="Primary">
        ${nav
          .map((item) => {
            const current = item.slug === pageSlug ? ' aria-current="page"' : "";
            return `<a href="${item.href}"${current}>${item.label}</a>`;
          })
          .join("\n        ")}
      </nav>
    </div>
  </header>
  <main class="page-shell">
    ${content}
  </main>
  <footer class="site-footer">
    <div class="site-footer-inner">
      <div class="footer-brand">Kevin M. Miller</div>
      <div class="footer-links">
        <a href="/contact/">Contact</a>
        <a href="/blog/">Writing</a>
        <a href="/talks.html">Speaking</a>
        <a href="/about/">About</a>
      </div>
      <p class="footer-related">
        Related:
        <a href="https://echelonfoundry.com">Echelon Foundry</a>
        <span> · </span>
        <a href="https://helixnote.com">HelixNote</a>
        <span> · </span>
        <a href="https://decisionposture.com">Decision Posture</a>
      </p>
    </div>
  </footer>
</body>
</html>`;
}

function renderRecentPosts(posts) {
  return posts
    .map(
      (post) => `
      <a class="post-card" href="${post.urlPath}">
        <div class="post-card-date">${escapeHtml(post.dateDisplay)}</div>
        <h3 class="post-card-title">${escapeHtml(post.title)}</h3>
        <p class="post-card-excerpt">${escapeHtml(post.excerpt)}</p>
      </a>
    `,
    )
    .join("");
}

function renderArchivePosts(posts) {
  return posts
    .map(
      (post) => `
      <a class="post-list-card" href="${post.urlPath}">
        <div class="post-card-date">${escapeHtml(post.dateDisplay)}</div>
        <h2 class="post-card-title">${escapeHtml(post.title)}</h2>
        <p class="post-card-excerpt">${escapeHtml(post.excerpt)}</p>
        <span class="post-card-read">Read more →</span>
      </a>
    `,
    )
    .join("");
}

function renderMarkdown(source) {
  const normalized = source.replace(/\r\n/g, "\n");
  const lines = normalized.split("\n");
  const html = [];
  let index = 0;

  while (index < lines.length) {
    const line = lines[index];
    const trimmed = line.trim();

    if (!trimmed) {
      index += 1;
      continue;
    }

    if (trimmed.startsWith("```")) {
      const fence = trimmed.slice(3).trim();
      const codeLines = [];
      index += 1;

      while (index < lines.length && !lines[index].trim().startsWith("```")) {
        codeLines.push(lines[index]);
        index += 1;
      }

      if (index < lines.length) {
        index += 1;
      }

      const className = fence ? ` class="language-${escapeHtml(fence)}"` : "";
      html.push(`<pre><code${className}>${escapeHtml(codeLines.join("\n"))}</code></pre>`);
      continue;
    }

    if (/^#{1,6}\s+/.test(trimmed)) {
      const level = trimmed.match(/^#+/)[0].length;
      let content = line.trim().replace(/^#{1,6}\s+/, "");
      while (
        hasUnbalancedDoubleAsterisks(content) &&
        index + 1 < lines.length &&
        lines[index + 1].trim()
      ) {
        index += 1;
        content += `\n${lines[index].trim()}`;
      }
      html.push(`<h${level}>${renderInline(content)}</h${level}>`);
      index += 1;
      continue;
    }

    if (/^(-{3,}|\*{3,})$/.test(trimmed)) {
      html.push("<hr />");
      index += 1;
      continue;
    }

    if (/^>\s?/.test(trimmed)) {
      const quoteLines = [];
      while (index < lines.length && /^>\s?/.test(lines[index].trim())) {
        quoteLines.push(lines[index].trim().replace(/^>\s?/, ""));
        index += 1;
      }
      html.push(`<blockquote><p>${renderInline(quoteLines.join("\n"))}</p></blockquote>`);
      continue;
    }

    if (isRawHtml(trimmed)) {
      const rawLines = [line];
      index += 1;
      while (index < lines.length && lines[index].trim()) {
        if (/^#{1,6}\s+/.test(lines[index].trim()) || /^(-|\*|\+)\s+/.test(lines[index].trim())) {
          break;
        }
        rawLines.push(lines[index]);
        index += 1;
      }
      html.push(rawLines.join("\n"));
      continue;
    }

    if (/^(-|\*|\+)\s+/.test(trimmed)) {
      const items = [];
      while (index < lines.length && /^(-|\*|\+)\s+/.test(lines[index].trim())) {
        items.push(lines[index].trim().replace(/^(-|\*|\+)\s+/, ""));
        index += 1;
      }
      html.push(`<ul>${items.map((item) => `<li>${renderInline(item)}</li>`).join("")}</ul>`);
      continue;
    }

    if (/^\d+\.\s+/.test(trimmed)) {
      const items = [];
      while (index < lines.length && /^\d+\.\s+/.test(lines[index].trim())) {
        items.push(lines[index].trim().replace(/^\d+\.\s+/, ""));
        index += 1;
      }
      html.push(`<ol>${items.map((item) => `<li>${renderInline(item)}</li>`).join("")}</ol>`);
      continue;
    }

    const paragraphLines = [line];
    index += 1;
    while (index < lines.length) {
      const next = lines[index];
      const nextTrimmed = next.trim();
      if (
        !nextTrimmed ||
        nextTrimmed.startsWith("```") ||
        /^#{1,6}\s+/.test(nextTrimmed) ||
        /^(-{3,}|\*{3,})$/.test(nextTrimmed) ||
        /^>\s?/.test(nextTrimmed) ||
        /^(-|\*|\+)\s+/.test(nextTrimmed) ||
        /^\d+\.\s+/.test(nextTrimmed) ||
        isRawHtml(nextTrimmed)
      ) {
        break;
      }
      paragraphLines.push(next);
      index += 1;
    }

    html.push(`<p>${renderInline(paragraphLines.join("\n"))}</p>`);
  }

  return html.join("\n\n");
}

function renderInline(input) {
  const htmlTokens = [];
  let rendered = input.replace(/<[^>]+>/g, (match) => {
    const token = `%%HTMLTOKEN${htmlTokens.length}%%`;
    htmlTokens.push(match);
    return token;
  });

  const codeTokens = [];
  rendered = rendered.replace(/`([^`]+)`/g, (_, code) => {
    const token = `%%CODETOKEN${codeTokens.length}%%`;
    codeTokens.push(`<code>${escapeHtml(code)}</code>`);
    return token;
  });

  rendered = escapeHtml(rendered);
  rendered = rendered.replace(/!\[([^\]]*)\]\(([^)]+)\)/g, '<img src="$2" alt="$1" />');
  rendered = rendered.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2">$1</a>');
  rendered = replaceStrong(rendered, "**");
  rendered = replaceStrong(rendered, "__");
  rendered = replaceEmphasis(rendered, "*");
  rendered = replaceEmphasis(rendered, "_");
  rendered = rendered.replace(/\n/g, "<br />\n");

  rendered = rendered.replace(/%%CODETOKEN(\d+)%%/g, (_, index) => codeTokens[Number(index)]);
  rendered = rendered.replace(/%%HTMLTOKEN(\d+)%%/g, (_, index) => htmlTokens[Number(index)]);
  return rendered;
}

function replaceStrong(input, delimiter) {
  const escaped = escapeRegExp(delimiter);
  const pattern = new RegExp(`${escaped}([\\s\\S]+?)${escaped}`, "g");
  const tokens = [];

  const replaced = input.replace(pattern, (_, inner) => {
    const token = `%%STRONGTOKEN${tokens.length}%%`;
    tokens.push(`<strong>${replaceEmphasis(inner, "*")}</strong>`);
    return token;
  });

  return replaced.replace(/%%STRONGTOKEN(\d+)%%/g, (_, index) => tokens[Number(index)]);
}

function replaceEmphasis(input, delimiter) {
  const escaped = escapeRegExp(delimiter);
  const pattern = new RegExp(`${escaped}([^${escaped}\\n]+?)${escaped}`, "g");
  return input.replace(pattern, "<em>$1</em>");
}

function escapeRegExp(input) {
  return input.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

function isRawHtml(line) {
  return /^<\/?[A-Za-z!][^>]*>$/.test(line) || /^<[^>]+>.*<\/[^>]+>$/.test(line);
}

function stripTags(html) {
  return html.replace(/<[^>]*>/g, " ");
}

function decodeHtmlEntities(text) {
  return text
    .replaceAll("&amp;", "&")
    .replaceAll("&lt;", "<")
    .replaceAll("&gt;", ">")
    .replaceAll("&quot;", '"')
    .replaceAll("&#39;", "'");
}

function truncate(text, length) {
  if (text.length <= length) {
    return text;
  }

  return `${text.slice(0, length).trimEnd()}…`;
}

function formatDisplayDate(date) {
  return new Intl.DateTimeFormat("en-US", {
    month: "long",
    day: "numeric",
    year: "numeric",
    timeZone: "UTC",
  }).format(date);
}

function slugToTitle(slug) {
  return slug
    .replace(/[-_]+/g, " ")
    .replace(/\b\w/g, (char) => char.toUpperCase());
}

function escapeHtml(input) {
  return input
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

function hasUnbalancedDoubleAsterisks(input) {
  const matches = input.match(/\*\*/g);
  return matches !== null && matches.length % 2 === 1;
}

async function resetOutputDir() {
  await fs.rm(outputDir, { recursive: true, force: true });
  await fs.mkdir(outputDir, { recursive: true });
}

async function copyStaticAssets() {
  const assetsDir = path.join(sourceDir, "assets");
  const entries = await fs.readdir(assetsDir, { withFileTypes: true });

  for (const entry of entries) {
    const sourcePath = path.join(assetsDir, entry.name);
    const destinationPath = path.join(outputDir, entry.name);
    if (entry.isDirectory()) {
      await fs.cp(sourcePath, destinationPath, { recursive: true });
    } else {
      await fs.copyFile(sourcePath, destinationPath);
    }
  }
}

async function writeOutput(relativePath, content) {
  const destination = path.join(outputDir, relativePath);
  await fs.mkdir(path.dirname(destination), { recursive: true });
  await fs.writeFile(destination, content);
}

main().catch((error) => {
  console.error(error);
  process.exitCode = 1;
});
