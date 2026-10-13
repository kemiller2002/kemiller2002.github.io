// Static server for the generated site (dist-v2/) used by the UI conformance suite.
// It mirrors GitHub Pages resolution: "/x/" serves "/x/index.html", and an
// unknown path is a 404, so a dead internal link is observable as a failure.
import { createServer } from "node:http";
import { readFile } from "node:fs/promises";
import { extname, join, normalize, resolve } from "node:path";
import { fileURLToPath } from "node:url";

const root = resolve(fileURLToPath(new URL("../../../dist-v2/", import.meta.url)));
const port = Number(process.env.UI_CONFORMANCE_PORT ?? 4317);

const TYPES = Object.freeze({
  ".html": "text/html; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".js": "text/javascript; charset=utf-8",
  ".png": "image/png",
  ".jpeg": "image/jpeg",
  ".jpg": "image/jpeg",
  ".svg": "image/svg+xml"
});

const candidatePath = pathname => {
  const decoded = decodeURIComponent(pathname);
  const relative = decoded.endsWith("/") ? `${decoded}index.html` : decoded;
  const absolute = normalize(join(root, relative));
  return absolute.startsWith(root) ? absolute : null;
};

const respond = (response, status, type, body) => {
  response.writeHead(status, { "content-type": type });
  response.end(body);
};

createServer(async (request, response) => {
  const path = candidatePath(new URL(request.url, "http://localhost").pathname);
  if (!path) return respond(response, 403, "text/plain", "Forbidden");
  try {
    const body = await readFile(path);
    return respond(response, 200, TYPES[extname(path)] ?? "application/octet-stream", body);
  } catch {
    return respond(response, 404, "text/plain", "Not found");
  }
}).listen(port, "127.0.0.1", () => process.stdout.write(`serving ${root} on http://127.0.0.1:${port}\n`));
