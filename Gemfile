source "https://rubygems.org"

# Use GitHub Pages' pinned Jekyll + plugins
gem "github-pages", group: :jekyll_plugins

# Ruby 3 needs webrick as a separate gem
gem "webrick"

# (Optional) Windows / JRuby bits – harmless on macOS
platforms :mingw, :x64_mingw, :mswin, :jruby do
  gem "tzinfo", "~> 1.2"
  gem "tzinfo-data"
end

gem "wdm", "~> 0.1.1", platforms: [:mingw, :x64_mingw, :mswin]
