---
layout: default
---

<div class="max-w-4xl mx-auto px-4 py-16 text-center">
  <h1 class="text-4xl md:text-5xl font-bold mb-4">Kevin Miller</h1>
  <p class="text-xl text-gray-600">Engineering Leader | Security Expert | AI Strategist</p>
  <p class="mt-4 text-gray-700">I help organizations modernize legacy systems, strengthen security posture, and deliver scalable technology solutions.</p>
  <div class="mt-6 flex justify-center space-x-4">
    <a href="mailto:kemiller2002@gmail.com" class="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">Contact Me</a>
    <a href="/assets/resume.pdf" class="border border-blue-600 text-blue-600 px-4 py-2 rounded hover:bg-blue-50">Download Resume</a>
  </div>
</div>

<!-- Use Cases -->
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
</div>

<!-- Speaking Engagements -->
<div class="py-12">
  <div class="max-w-4xl mx-auto px-4">
    <h2 class="text-3xl font-semibold mb-6 text-center">Speaking Engagements</h2>
    <ul class="space-y-4">
      <li><strong>DerbyCon</strong> – Secure DevOps for High-Stakes Environments</li>
      <li><strong>Internal Security Summit</strong> – Threat Modeling at Scale</li>
      <li><strong>AI & Trust Panel</strong> – Responsible AI in Regulated Markets</li>
    </ul>
  </div>
</div>

<!-- Experience -->
<div class="bg-gray-50 py-12">
  <div class="max-w-4xl mx-auto px-4">
    <h2 class="text-3xl font-semibold mb-6 text-center">Experience</h2>
    <div class="space-y-6 text-left">
      <div>
        <h3 class="text-xl font-bold">Ren — Director of Engineering</h3>
        <p class="text-gray-600">Led platform modernization, DAF innovation, and security transformation initiatives across teams.</p>
      </div>
      <div>
        <h3 class="text-xl font-bold">Equifax — Application Security Lead</h3>
        <p class="text-gray-600">Built threat modeling program and enabled secure development practices across major product lines.</p>
      </div>
      <!-- Add more roles as needed -->
    </div>
  </div>
</div>

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

<!-- Contact -->
<div class="bg-blue-50 py-12">
  <div class="max-w-3xl mx-auto px-4 text-center">
    <h2 class="text-3xl font-semibold mb-4">Let’s Connect</h2>
    <p class="text-gray-700 mb-4">Have a project, advisory need, or just want to chat about AI and security?</p>
    <a href="mailto:kemiller2002@gmail.com" class="inline-block bg-blue-600 text-white px-6 py-3 rounded hover:bg-blue-700">Email Me</a>
  </div>
</div>
