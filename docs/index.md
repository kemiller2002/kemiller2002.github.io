---
layout: default
---

<section class="hero">
  <div class="hero-meta">Engineering Leader · Security Expert · AI Strategist</div>
  <h1>Kevin M Miller</h1>
  <p class="hero-lede">
    I help organizations modernize legacy systems, strengthen security posture, and deliver scalable technology solutions.
  </p>
  <div class="hero-actions">
    <a href="/contact/" class="cta-button">Contact Me</a>
    <a href="/resume" class="ghost-button">Download Resume</a>
  </div>
</section>

<section class="section">
  <div class="section-heading">
    <div class="eyebrow">Latest Writing</div>
    <h2>Recent Blog Posts</h2>
    <a class="section-link" href="/blog">View all posts →</a>
  </div>
  <div class="posts-grid">
    {% for post in site.posts limit:6 %}
      <a class="post-card" href="{{ post.url }}">
        <div class="post-card-date">{{ post.date | date: "%B %d, %Y" }}</div>
        <h3 class="post-card-title">{{ post.title }}</h3>
        <p class="post-card-excerpt">{{ post.excerpt | strip_html | truncate: 130 }}</p>
      </a>
    {% endfor %}
  </div>
</section>

<section class="section">
  <div class="section-heading">
    <div class="eyebrow">On Stage</div>
    <h2>Selected Speaking Engagements</h2>
  </div>
  <div class="grid-two">
    <div class="card">
      <h3 class="card-title">C# &amp; .NET Development</h3>
      <ul class="card-list">
        <li>Covariance and Contravariance in C#</li>
        <li>Code Contracts</li>
        <li>Dependency Injection</li>
        <li>Dynamic Language Runtime in C#</li>
        <li>Functional Programming in C#</li>
        <li>Introduction to LINQ</li>
        <li>New Features in C# 7</li>
        <li>Obfuscation in .NET</li>
        <li>App Performance Monitoring Solution</li>
      </ul>
    </div>
    <div class="card">
      <h3 class="card-title">JavaScript &amp; Functional Programming</h3>
      <ul class="card-list">
        <li>Introduction to JavaScript</li>
        <li>Currying in JavaScript</li>
        <li>Functional Pipelining</li>
      </ul>
    </div>
    <div class="card">
      <h3 class="card-title">Security &amp; Hacking</h3>
      <ul class="card-list">
        <li>SQL Injection: Attacking the System</li>
        <li>Social Engineering</li>
        <li>The Psychopathic Profile</li>
        <li>Hacking .NET and IIS</li>
      </ul>
    </div>
    <div class="card">
      <h3 class="card-title">Software Culture &amp; Agile</h3>
      <ul class="card-list">
        <li>Agile and Scrum using TFS</li>
        <li>The Cultural Requirements of Agile</li>
        <li>My Team is Awesome (and Yours Is Too)</li>
      </ul>
    </div>
    <div class="card">
      <h3 class="card-title">General Technical Topics</h3>
      <ul class="card-list">
        <li>Regular Expressions</li>
        <li>Time: Taking a Minute to Ponder It</li>
        <li>Git: The Last Database You'll Ever Need</li>
        <li>Introduction to F#</li>
      </ul>
    </div>
  </div>
</section>

<section class="section">
  <div class="section-heading">
    <div class="eyebrow">Leadership</div>
    <h2>Executive Experience</h2>
  </div>
  <div class="stacked-cards">
    <div class="card">
      <div class="card-top">
        <h3 class="card-title">Ren</h3>
        <div class="card-subtitle">Director of Engineering • 2020 – 2025</div>
      </div>
      <ul class="card-list">
        <li>Led 9 engineering teams supporting 17 of the top 20 financial institutions in the DAF space.</li>
        <li>Reduced time-to-market from 6 weeks to 2 days via CI/CD, branching, and release strategy redesign.</li>
        <li>Cut over $500K in annual OPEX by consolidating vendors and modernizing infrastructure.</li>
        <li>Improved roadmap predictability to within 10% variance by aligning delivery with metrics and planning.</li>
        <li>Served on Enterprise Architecture and Security Compliance Boards guiding org-wide technical governance.</li>
      </ul>
    </div>
    <div class="card">
      <div class="card-top">
        <h3 class="card-title">T2 Systems</h3>
        <div class="card-subtitle">Senior Developer • 2018 – 2020</div>
      </div>
      <ul class="card-list">
        <li>Stabilized 300+ production environments through coordinated deployment strategies.</li>
        <li>Built a low-overhead .NET CLR Profiler for deep diagnostics and exception tracing.</li>
        <li>Reformed cross-team Agile practices and modernized Azure DevOps pipelines.</li>
      </ul>
    </div>
    <div class="card">
      <div class="card-top">
        <h3 class="card-title">TCC Software Solutions</h3>
        <div class="card-subtitle">Development Manager • 2015 – 2018</div>
      </div>
      <ul class="card-list">
        <li>Reduced software licensing costs by $250K/year by reconfiguring Microsoft Gold Partner usage.</li>
        <li>Architected statewide system for Indiana DNR to manage water resource data and infrastructure.</li>
        <li>Founded internship program and community tech meetups to grow hiring funnel and brand visibility.</li>
      </ul>
    </div>
  </div>
  <p class="muted center-note">
    Known for blending deep engineering expertise with strategic leadership across C#/F#/.NET, JavaScript/TypeScript, distributed systems, microservices, and cloud architecture (AWS, Azure).
  </p>
</section>

<section class="cta-strip">
  <div class="cta-strip-inner">
    <div>
      <h3>Let’s Connect</h3>
      <p>Have a project, advisory need, or just want to chat about AI and security?</p>
    </div>
    <a href="/contact/" class="cta-button">Email Me</a>
  </div>
</section>
