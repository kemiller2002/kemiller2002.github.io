# Jekyll + Docker Template (GitHub Pages Compatible)

This repo provides a reusable Docker + Gemfile setup for running a GitHub Pages–style Jekyll site locally without installing Ruby or Jekyll on your machine.

It’s designed to match the GitHub Pages environment by using the `github-pages` gem, and it avoids common Linux watcher issues (ffi/rb-inotify) by disabling file watching in Docker.

## Files

- `Gemfile`  
  Uses `github-pages` + `webrick` to mirror GitHub Pages’ Jekyll stack.

- `Dockerfile`  
  Dev-focused container that runs `bundle exec jekyll serve --no-watch` on port 4000.

- `Dockerfile.prod` (optional)  
  Builds the site (`jekyll build`) and then serves the static `_site` output via a tiny Ruby HTTP server.

- `docker-compose.yml` (optional)  
  Convenience wrapper to build and run the dev container with a single command.

## Usage (Dev)

From the repo root:

```bash
docker build -t jekyll-site .
docker run --rm -p 4000:4000 jekyll-site