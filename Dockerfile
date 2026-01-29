# Dockerfile (matches GitHub Actions build)
FROM ruby:3.2

# System dependencies
RUN apt-get update -y \
    && apt-get install -y build-essential nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /srv/jekyll

# Install gems using Gemfile + Gemfile.lock to mirror CI
COPY Gemfile Gemfile.lock ./
RUN bundle config set --local path 'vendor/bundle' \
    && bundle config set force_ruby_platform true \
    && bundle install

# Copy the rest of the site into the container
COPY . .

ENV JEKYLL_ENV=production

# Run the same build step as GitHub Actions
CMD ["bundle", "exec", "jekyll", "build"]
