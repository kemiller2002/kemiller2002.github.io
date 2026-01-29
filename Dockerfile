# Dockerfile  (dev / local)
FROM ruby:3.1

# System dependencies
RUN apt-get update -y \
    && apt-get install -y build-essential nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /srv/jekyll

# Install gems using Gemfile (ignore Gemfile.lock to avoid platform quirks)
COPY Gemfile ./
RUN bundle config set --local path 'vendor/bundle' \
    && bundle config set force_ruby_platform true \
    && bundle install

# Copy the rest of the site into the container
COPY . .

ENV JEKYLL_ENV=development

EXPOSE 4000

# Run Jekyll via Bundler, but WITHOUT file watching so we avoid ffi/rb-inotify pain
CMD ["bundle", "exec", "jekyll", "serve", "--host", "0.0.0.0", "--port", "4000", "--no-watch"]