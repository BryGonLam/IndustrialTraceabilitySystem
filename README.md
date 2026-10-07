<h1 align="center">Industrial Traceability System (ITS)</h1>

<p align="center">
  <em>A fictitious industrial traceability system inspired by real electronics manufacturing scenarios.</em>
</p>

<p align="center">
  <img alt="version" src="https://img.shields.io/badge/version-0.1.0--alpha-blue.svg" />
  <img alt="status" src="https://img.shields.io/badge/status-in%20development-yellow.svg" />
  <img alt="license" src="https://img.shields.io/badge/license-MIT-lightgrey.svg" />
  <img alt="dotnet" src="https://img.shields.io/badge/.NET-8.0-512BD4.svg" />
  <img alt="efcore" src="https://img.shields.io/badge/EF%20Core-8.0.10-512BD4.svg" />
  <img alt="sqlserver" src="https://img.shields.io/badge/SQL%20Server-Express-CC2927.svg" />
</p>

<hr />

<h2>Table of Contents</h2>

<ul>
  <li><a href="#about">About</a></li>
  <li><a href="#goals">Goals</a></li>
  <li><a href="#current-status">Current Status</a></li>
  <li><a href="#tech-stack">Tech Stack</a></li>
  <li><a href="#repository-structure">Repository Structure</a></li>
  <li><a href="#documentation">Documentation</a></li>
  <li><a href="#versioning">Versioning</a></li>
  <li><a href="#getting-started">Getting Started</a></li>
  <li><a href="#roadmap">Roadmap</a></li>
  <li><a href="#disclaimer">Disclaimer</a></li>
  <li><a href="#license">License</a></li>
</ul>

<hr />

<h2 id="about">About</h2>

<p>
  <strong>Industrial Traceability System (ITS)</strong> is a web system for industrial traceability,
  inspired by real electronics manufacturing scenarios but completely fictitious and independent of any
  proprietary system, code, database, name, configuration or confidential information.
</p>

<p>
  The system allows registering, querying and analyzing the lifecycle of serialized units inside a
  factory. A unit can represent a PCB, an electronic product, a module or any serialized item.
</p>

<p>With ITS you can know:</p>

<ul>
  <li>which unit was produced</li>
  <li>under which production order</li>
  <li>on which production line</li>
  <li>through which stations it passed</li>
  <li>which events occurred</li>
  <li>when they occurred</li>
  <li>what result each event had</li>
  <li>whether the unit had failures</li>
  <li>whether it went through rework</li>
  <li>what its current status is</li>
</ul>

<p>Conceptual example:</p>

<pre>
PCB000001

Printer
   ↓ PASS
SPI
   ↓ PASS
AOI
   ↓ FAIL
Rework
   ↓ COMPLETE
AOI
   ↓ PASS
Final Inspection
   ↓ PASS
</pre>

<hr />

<h2 id="goals">Goals</h2>

<p>The main goals of this project are:</p>

<ol>
  <li>Build a functional and understandable web traceability system.</li>
  <li>Register the production history of serialized units.</li>
  <li>Query events, stations, results and statuses per unit.</li>
  <li>Model common manufacturing scenarios: PASS, FAIL, REWORK, SCRAP, HOLD, START, STOP.</li>
  <li>Administer basic production data: orders, products, lines, stations, units.</li>
  <li>Incorporate authentication, users and roles in later stages.</li>
  <li>Provide basic dashboards and reports for production and traceability.</li>
  <li>Apply good practices for development, testing and version control.</li>
  <li>Document the construction process as part of the professional portfolio.</li>
  <li>Keep the project free of any real, confidential or proprietary information.</li>
</ol>

<p>Development priorities:</p>

<pre>
Understanding &gt; complexity
Quality       &gt; quantity
Justified decisions &gt; patterns used by trend
</pre>

<hr />

<h2 id="current-status">Current Status</h2>

<p>
  <strong>Version:</strong> 0.1.0-alpha
  <br />
  <strong>Stage:</strong> Backend foundation
  <br />
  <strong>Last update:</strong> October 2026
</p>

<p>What is done:</p>

<ul>
  <li>Project documentation (6 design documents in <code>docs/</code>)</li>
  <li>Backend solution structure (.NET 8 Web API)</li>
  <li>Domain entities and enums</li>
  <li>Entity Framework Core <code>AppDbContext</code></li>
  <li>Initial migration applied to SQL Server Express</li>
  <li>User Secrets configured for local development</li>
</ul>

<p>What is in progress:</p>

<ul>
  <li>Seed data for local development</li>
  <li>First API endpoint (<code>GET /api/v1/products</code>)</li>
</ul>

<hr />

<h2 id="tech-stack">Tech Stack</h2>

<h3>Backend</h3>

<ul>
  <li>.NET 8.0</li>
  <li>ASP.NET Core Web API</li>
  <li>Entity Framework Core 8.0.10</li>
  <li>SQL Server Express</li>
</ul>

<h3>Frontend (planned)</h3>

<ul>
  <li>React</li>
  <li>Vite</li>
  <li>TypeScript</li>
</ul>

<h3>Authentication (planned)</h3>

<ul>
  <li>JWT</li>
</ul>

<h3>Testing (planned)</h3>

<ul>
  <li>xUnit</li>
</ul>

<h3>Version control</h3>

<ul>
  <li>Git</li>
  <li>GitHub</li>
</ul>

<hr />

<h2 id="repository-structure">Repository Structure</h2>

<pre>
IndustrialTraceabilitySystem/
│
├── .github/
│   └── workflows/
│
├── docs/
│   ├── 01-project-overview.md
│   ├── 02-requirements.md
│   ├── 03-domain-model.md
│   ├── 04-architecture-overview.md
│   ├── 05-database-design.md
│   └── 06-api-design.md
│
├── src/
│   ├── backend/
│   │   └── IndustrialTraceabilitySystem.Api/
│   │       ├── Controllers/
│   │       ├── Domain/
│   │       │   ├── Entities/
│   │       │   └── Enums/
│   │       ├── Infrastructure/
│   │       │   └── Persistence/
│   │       ├── Migrations/
│   │       ├── Properties/
│   │       ├── Program.cs
│   │       ├── appsettings.json
│   │       └── IndustrialTraceabilitySystem.Api.csproj
│   │
│   └── frontend/
│       └── (to be created)
│
├── tests/
│   └── (to be created)
│
├── IndustrialTraceabilitySystem.sln
├── README.md
└── .gitignore
</pre>

<hr />

<h2 id="documentation">Documentation</h2>

<p>All design decisions are documented before implementation. Available documents:</p>

<table>
  <thead>
    <tr>
      <th>#</th>
      <th>Document</th>
      <th>Purpose</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>01</td>
      <td><a href="docs/01-project-overview.md">Project Overview</a></td>
      <td>Vision, problem, goals, scope and target users.</td>
    </tr>
    <tr>
      <td>02</td>
      <td><a href="docs/02-requirements.md">Requirements</a></td>
      <td>Functional, non-functional requirements and business rules.</td>
    </tr>
    <tr>
      <td>03</td>
      <td><a href="docs/03-domain-model.md">Domain Model</a></td>
      <td>Entities, relationships and domain vocabulary.</td>
    </tr>
    <tr>
      <td>04</td>
      <td><a href="docs/04-architecture-overview.md">Architecture Overview</a></td>
      <td>High-level architecture and technical decisions.</td>
    </tr>
    <tr>
      <td>05</td>
      <td><a href="docs/05-database-design.md">Database Design</a></td>
      <td>Relational schema, tables, columns and constraints.</td>
    </tr>
    <tr>
      <td>06</td>
      <td><a href="docs/06-api-design.md">API Design</a></td>
      <td>Endpoints, resources, HTTP methods and DTOs.</td>
    </tr>
  </tbody>
</table>

<hr />

<h2 id="versioning">Versioning</h2>

<p>
  This project follows <a href="https://semver.org/">Semantic Versioning</a> (SemVer)
  in the format <code>MAJOR.MINOR.PATCH</code>.
</p>

<table>
  <thead>
    <tr>
      <th>Segment</th>
      <th>Meaning</th>
      <th>Example</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>MAJOR</td>
      <td>Incompatible or structural changes</td>
      <td>1.0.0</td>
    </tr>
    <tr>
      <td>MINOR</td>
      <td>New features, backward compatible</td>
      <td>0.2.0</td>
    </tr>
    <tr>
      <td>PATCH</td>
      <td>Bug fixes and small adjustments</td>
      <td>0.1.1</td>
    </tr>
  </tbody>
</table>

<h3>Current version</h3>

<p><code>0.1.0-alpha</code></p>

<h3>Version history</h3>

<table>
  <thead>
    <tr>
      <th>Version</th>
      <th>Date</th>
      <th>Summary</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>0.1.0-alpha</td>
      <td>2026-10</td>
      <td>Backend foundation: domain entities, EF Core, initial migration.</td>
    </tr>
  </tbody>
</table>

<h3>Branch strategy</h3>

<p>This project follows a staged branching model:</p>

<pre>
main       ← stable, production-ready releases
  ↑
release/*  ← release candidates before merging to main
  ↑
develop    ← integration branch for finished features
  ↑
feature/*  ← individual features, bug fixes, small changes
</pre>

<p>All changes flow from <code>feature/*</code> to <code>develop</code> via Pull Request, then to <code>release/*</code>, and finally to <code>main</code>.</p>

<h3>Commit conventions</h3>

<p>Commits follow <a href="https://www.conventionalcommits.org/">Conventional Commits</a>:</p>

<pre>
feat:      new feature
fix:       bug fix
docs:      documentation changes
chore:     maintenance tasks
refactor:  code change without new behavior
test:      adding or changing tests
style:     formatting changes
</pre>

<hr />

<h2 id="getting-started">Getting Started</h2>

<p><strong>Prerequisites:</strong></p>

<ul>
  <li>.NET 8.0 SDK</li>
  <li>SQL Server Express (or any compatible SQL Server instance)</li>
  <li>Visual Studio 2022 or VS Code</li>
  <li>EF Core CLI tools: <code>dotnet tool install --global dotnet-ef --version 8.0.10</code></li>
</ul>

<p><strong>Setup:</strong></p>

<ol>
  <li>
    Clone the repository:
    <pre>git clone https://github.com/BryGonLam/IndustrialTraceabilitySystem.git
cd IndustrialTraceabilitySystem</pre>
  </li>
  <li>
    Configure the database connection using User Secrets:
    <pre>cd src/backend/IndustrialTraceabilitySystem.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=IndustrialTraceabilitySystemDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"</pre>
  </li>
  <li>
    Apply migrations:
    <pre>dotnet ef database update</pre>
  </li>
  <li>
    Run the API:
    <pre>dotnet run</pre>
  </li>
  <li>
    Open Swagger:
    <pre>https://localhost:&lt;port&gt;/swagger</pre>
  </li>
</ol>

<p>
  <strong>Note:</strong> real credentials and connection strings are never stored in the repository.
  They live in User Secrets (local only).
</p>

<hr />

<h2 id="roadmap">Roadmap</h2>

<table>
  <thead>
    <tr>
      <th>Stage</th>
      <th>Description</th>
      <th>Status</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>1</td>
      <td>Project Overview</td>
      <td>Done</td>
    </tr>
    <tr>
      <td>2</td>
      <td>Requirements</td>
      <td>Done</td>
    </tr>
    <tr>
      <td>3</td>
      <td>Domain Model</td>
      <td>Done</td>
    </tr>
    <tr>
      <td>4</td>
      <td>Architecture Overview</td>
      <td>Done</td>
    </tr>
    <tr>
      <td>5</td>
      <td>Database Design</td>
      <td>Done</td>
    </tr>
    <tr>
      <td>6</td>
      <td>API Design</td>
      <td>Done</td>
    </tr>
    <tr>
      <td>7</td>
      <td>Backend Setup (entities, DbContext, migration)</td>
      <td>In progress</td>
    </tr>
    <tr>
      <td>8</td>
      <td>Production Endpoints</td>
      <td>Pending</td>
    </tr>
    <tr>
      <td>9</td>
      <td>Traceability Endpoints</td>
      <td>Pending</td>
    </tr>
    <tr>
      <td>10</td>
      <td>Authentication (JWT, users, roles)</td>
      <td>Pending</td>
    </tr>
    <tr>
      <td>11</td>
      <td>Frontend Setup</td>
      <td>Pending</td>
    </tr>
    <tr>
      <td>12</td>
      <td>Monitoring and Reports</td>
      <td>Pending</td>
    </tr>
    <tr>
      <td>13</td>
      <td>Testing, CI and final documentation</td>
      <td>Pending</td>
    </tr>
  </tbody>
</table>

<hr />

<h2 id="disclaimer">Disclaimer</h2>

<p>
  This project is <strong>completely fictitious</strong>. It is inspired by real electronics
  manufacturing scenarios but is independent of any company, proprietary system, code, database,
  name, configuration or confidential information.
</p>

<p>
  All data, names of lines, stations, models and products used in this repository are invented and
  must not be interpreted as real information.
</p>

<p>The project does not connect to real machines, PLCs, sensors or industrial equipment.</p>

<hr />

<h2 id="license">License</h2>

<p>
  This project is released under the <strong>MIT License</strong>. See the repository for details.
</p>

<hr />

<p align="center">
  <em>Built as a professional portfolio project by <a href="https://github.com/BryGonLam">Bryant Gonzalez</a>.</em>
</p>
