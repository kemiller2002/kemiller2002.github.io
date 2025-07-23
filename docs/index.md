---
layout: default
---

<div class="max-w-4xl mx-auto px-4 py-16 text-center">
  <h1 class="text-4xl md:text-5xl font-bold mb-4">Kevin Miller</h1>
  <p class="text-xl text-gray-600">Engineering Leader | Security Expert | AI Strategist</p>
  <p class="mt-4 text-gray-700">I help organizations modernize legacy systems, strengthen security posture, and deliver scalable technology solutions.</p>
  <div class="mt-6 flex justify-center space-x-4">
    <a href="mailto:kemiller2002@gmail.com" class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">Contact Me</a>
    <a href="/resume/executive.html" class="border border-blue-600 text-blue-600 px-4 py-2 rounded hover:bg-blue-50">Download Resume</a>
  </div>
</div>

<!-- Use Cases
<div class="bg-gray-100 py-12">
  <div class="max-w-5xl mx-auto px-4">
    <h2 class="text-3xl font-semibold mb-6 text-center">Consulting Use Cases</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      {% for use_case in site.data.use_cases %}
        <div class="bg-white p-6 rounded shadow hover:shadow-lg transition">
          <h3 class="text-xl font-bold mb-2">{{ use_case.title }}</h3>
          <p class="text-gray-600">{{ use_case.description }}</p>
        </div>
      {% endfor %}
    </div>
  </div>
</div>-->

<!-- Blog Posts -->
<div class="py-12">
  <div class="max-w-5xl mx-auto px-4">
    <h2 class="text-3xl font-semibold mb-6 text-center">Recent Blog Posts</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      {% for post in site.posts limit:4 %}
        <a href="{{ post.url }}" class="block bg-white rounded shadow p-6 hover:shadow-lg transition">
          <h3 class="text-xl font-bold">{{ post.title }}</h3>
          <p class="text-gray-600 mt-2">{{ post.excerpt | strip_html | truncate: 100 }}</p>
          <p class="text-sm text-gray-400 mt-2">{{ post.date | date: "%B %-d, %Y" }}</p>
        </a>
      {% endfor %}
    </div>
    <div class="text-center mt-6">
      <a href="/blog" class="text-blue-600 hover:underline">View All Posts →</a>
    </div>
  </div>
</div>

<!-- Speaking Engagements -->
<section class="py-12 bg-gray-50">
  <div class="max-w-6xl mx-auto px-4">
    <h2 class="text-3xl font-bold text-center mb-10">Selected Speaking Engagements</h2>
  
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
      <!-- C# and .NET -->
      <div class="bg-white p-6 rounded-lg shadow hover:shadow-md transition">
        <h3 class="text-xl font-semibold mb-3 text-blue-700">C# & .NET Development</h3>
        <ul class="list-disc list-inside text-gray-700 space-y-1 text-sm">
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

      <!-- JavaScript -->
      <div class="bg-white p-6 rounded-lg shadow hover:shadow-md transition">
        <h3 class="text-xl font-semibold mb-3 text-blue-700">JavaScript & Functional Programming</h3>
        <ul class="list-disc list-inside text-gray-700 space-y-1 text-sm">
          <li>Introduction to JavaScript</li>
          <li>Currying in JavaScript</li>
          <li>Functional Pipelining</li>
        </ul>
      </div>

      <!-- Security -->
      <div class="bg-white p-6 rounded-lg shadow hover:shadow-md transition">
        <h3 class="text-xl font-semibold mb-3 text-blue-700">Security & Hacking</h3>
        <ul class="list-disc list-inside text-gray-700 space-y-1 text-sm">
          <li>SQL Injection: Attacking the System</li>
          <li>Social Engineering</li>
          <li>The Psychopathic Profile</li>
          <li>Hacking .NET and IIS</li>
        </ul>
      </div>

      <!-- Agile -->
      <div class="bg-white p-6 rounded-lg shadow hover:shadow-md transition">
        <h3 class="text-xl font-semibold mb-3 text-blue-700">Software Development Culture & Agile</h3>
        <ul class="list-disc list-inside text-gray-700 space-y-1 text-sm">
          <li>Agile and Scrum using TFS</li>
          <li>The Cultural Requirements of Agile</li>
          <li>My Team is Awesome (and Yours Is Too)</li>
        </ul>
      </div>

      <!-- General Topics -->
      <div class="bg-white p-6 rounded-lg shadow hover:shadow-md transition">
        <h3 class="text-xl font-semibold mb-3 text-blue-700">General Technical Topics</h3>
        <ul class="list-disc list-inside text-gray-700 space-y-1 text-sm">
          <li>Regular Expressions</li>
          <li>Time: Taking a Minute to Ponder It</li>
          <li>Git: The Last Database You'll Ever Need ;)</li>
          <li>Introduction to F#</li>
        </ul>
      </div>
    </div>

  </div>
</section>

<!-- Experience -->
<section class="py-12 bg-white">
  <div class="max-w-5xl mx-auto px-4">
    <h2 class="text-3xl font-bold text-center mb-10">Executive Experience</h2>

    <!-- Ren -->
    <div class="bg-gray-50 p-6 rounded-lg shadow mb-6 hover:shadow-md transition">
      <h3 class="text-xl font-semibold text-blue-700">Ren</h3>
      <p class="text-sm text-gray-500 mb-3">Director of Engineering • 2020 – 2025</p>
      <ul class="list-disc list-inside text-gray-700 text-sm space-y-1">
        <li>Led 9 engineering teams supporting 17 of the top 20 financial institutions in the DAF space</li>
        <li>Reduced time-to-market from 6 weeks to 2 days via CI/CD, branching, and release strategy redesign</li>
        <li>Cut over $500K in annual OPEX by consolidating vendors and modernizing infrastructure</li>
        <li>Improved roadmap predictability to within 10% variance by aligning delivery with metrics and planning</li>
        <li>Served on Enterprise Architecture and Security Compliance Boards guiding org-wide technical governance</li>
      </ul>
    </div>

    <!-- T2 Systems -->
    <div class="bg-gray-50 p-6 rounded-lg shadow mb-6 hover:shadow-md transition">
      <h3 class="text-xl font-semibold text-blue-700">T2 Systems</h3>
      <p class="text-sm text-gray-500 mb-3">Senior Developer • 2018 – 2020</p>
      <ul class="list-disc list-inside text-gray-700 text-sm space-y-1">
        <li>Stabilized 300+ production environments through coordinated deployment strategies</li>
        <li>Built a low-overhead .NET CLR Profiler for deep diagnostics and exception tracing</li>
        <li>Reformed cross-team Agile practices and modernized Azure DevOps pipelines</li>
      </ul>
    </div>

    <!-- TCC Software Solutions -->
    <div class="bg-gray-50 p-6 rounded-lg shadow mb-6 hover:shadow-md transition">
      <h3 class="text-xl font-semibold text-blue-700">TCC Software Solutions</h3>
      <p class="text-sm text-gray-500 mb-3">Development Manager • 2015 – 2018</p>
      <ul class="list-disc list-inside text-gray-700 text-sm space-y-1">
        <li>Reduced software licensing costs by $250K/year by reconfiguring Microsoft Gold Partner usage</li>
        <li>Architected statewide system for Indiana DNR to manage water resource data and infrastructure</li>
        <li>Founded internship program and community tech meetups to grow hiring funnel and brand visibility</li>
      </ul>
    </div>

    <!-- Technical Depth Note -->
    <div class="text-sm text-gray-600 text-center mt-10 max-w-2xl mx-auto italic">
      Known for blending deep engineering expertise with strategic leadership.
      Experienced in C#, F#, .NET, JavaScript/TypeScript, distributed systems, microservices, and cloud architecture (AWS, Azure).
    </div>

  </div>
</section>

<!-- Contact -->
<div class="bg-blue-50 py-12">
  <div class="max-w-3xl mx-auto px-4 text-center">
    <h2 class="text-3xl font-semibold mb-4">Let’s Connect</h2>
    <p class="text-gray-700 mb-4">Have a project, advisory need, or just want to chat about AI and security?</p>
    <a href="mailto:kemiller2002@gmail.com" class="inline-block bg-blue-600 text-white px-6 py-3 rounded hover:bg-blue-700">Email Me</a>
  </div>
</div>
