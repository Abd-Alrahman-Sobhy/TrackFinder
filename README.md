# TrackFinder

TrackFinder is a web platform that helps students discover the tech career track that suits them, then learn it through courses, community discussion, and achievements, all in one place.

Students take a personality and interest assessment, get their top three matching tracks (with skills, tech stacks, tools, and a roadmap for each), and can continue straight into instructor-created courses. Instructors publish courses and lessons, admins manage the whole platform, and a community space lets members share posts and discuss.

---

## Features

### Career track assessment
- A randomized Likert-scale questionnaire (the questions are in Arabic).
- Each question carries a per-track weight. Answers are scored against every track, and the three best matches are returned with a **similarity percentage**.
- Each recommended track links to its **skills**, **tech stacks**, **tools**, and a **roadmap URL**.
- Past results are saved and can be revisited from an assessment history page.
- A duplicate-submission guard prevents accidental double inserts.

### Courses and learning
- Instructors create courses, lessons, and downloadable materials.
- Students browse the course catalog, enroll, view their own courses, and follow lessons.
- Enrollment and purchase records, plus exam, question, option, and attempt models, are in the data model.

### Community
- Posts, comments, joining communities, and reporting posts.
- Admin moderation (post approval status and reports).

### Achievements
- Admins create badges and certificates and assign them to users and students.
- Students see their certificates on a dedicated page.

### Accounts and security
- Registration with **email OTP verification** and a password reset flow via OTP.
- Three roles: **Student**, **Instructor**, and **Admin**.
- Instructor accounts must be **approved by an admin** before they can sign in.
- ASP.NET Core Identity with cookie auth (HttpOnly, Secure, SameSite=Strict, 2-hour sliding expiry).
- Rate limiting: a global per-IP limiter and a stricter policy for auth endpoints.
- Centralized exception-handling middleware.

### Admin panel
Dashboard plus management screens for users, students, instructors, account approvals, courses, tracks, track stacks, communities, badges, and certificates.

---

## Tech stack

| Layer | Technology |
| --- | --- |
| Framework | ASP.NET Core MVC on **.NET 10** (`net10.0`) |
| Language | C# (nullable reference types enabled) |
| Data access | Entity Framework Core 10 with SQL Server |
| Auth | ASP.NET Core Identity (`User` / `Role`) |
| Email | MailKit (SMTP) |
| Hashing | BCrypt.Net-Next |
| Front end | Razor views, CSS (`site.css`, `admin.css`, `dashboard.css`), vanilla JavaScript |

---

## Project structure

```
TrackFinder/
└── Track Finder/                 # ASP.NET Core project
    ├── Controllers/              # MVC controllers
    │   ├── Admin/                # Admin panel controllers
    │   ├── Auth Controllers/     # Login and registration
    │   └── Course Controllers/   # Course creation, lessons, materials, sales
    ├── Models/                   # Entities, grouped by domain
    │   ├── Achievement Models/
    │   ├── Assessment Models/
    │   ├── Community Models/
    │   ├── Course Models/
    │   ├── Orders And Payments Models/
    │   └── User Models/
    ├── Model Configuration/      # EF Core Fluent API configurations
    ├── Context/AppDbContext.cs   # EF Core DbContext
    ├── Migrations/               # EF Core migrations
    ├── Services/                 # Business logic (auth, profile, assessment)
    ├── Interfaces/               # Service contracts
    ├── Providers/                # Email sender, token and password providers
    ├── Filters/                  # Global MVC filters
    ├── Middleware/               # Exception-handling middleware
    ├── ViewModels/               # View models
    ├── Views/                    # Razor views
    ├── wwwroot/                  # Static files (css, js, uploads, materials)
    ├── Assets/                   # Logo, color palette, design references
    ├── Program.cs                # App startup and DI configuration
    └── appsettings.json
```

---

## Getting started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance, Docker, or a hosted database)
- An SMTP account for sending OTP and welcome emails (for example, a Gmail app password)

### 1. Clone the repository

```bash
git clone https://github.com/Abd-Alrahman-Sobhy/TrackFinder.git
cd TrackFinder/"Track Finder"
```

### 2. Configure your settings

> **Do not commit real credentials.** Keep secrets out of `appsettings.json` and use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables instead.

The app reads these settings:

| Key | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenMinutes` | Token provider settings |
| `Email:SmtpHost`, `Email:SmtpPort`, `Email:FromAddress`, `Email:FromName`, `Email:Username`, `Email:Password` | SMTP settings for OTP and welcome emails |

Example using user secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=TrackFinder;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet user-secrets set "Jwt:Key" "<a-long-random-secret>"
dotnet user-secrets set "Email:Username" "<smtp-username>"
dotnet user-secrets set "Email:Password" "<smtp-app-password>"
dotnet user-secrets set "Email:FromAddress" "<from-address>"
```

### 3. Create the database

```bash
dotnet tool install --global dotnet-ef   # if you don't have it yet
dotnet ef database update
```

The assessment needs track, skill, stack, tool, and question-weight data in the database before it can produce results.

### 4. Run the app

```bash
dotnet run --launch-profile https
```

Then open `https://localhost:7047` (or `http://localhost:5091`).

---

## Roles at a glance

| Role | What they can do |
| --- | --- |
| **Student** | Take the assessment, view results and history, browse and enroll in courses, join communities, view certificates and badges |
| **Instructor** | Create and manage courses, lessons, and materials (requires admin approval to sign in) |
| **Admin** | Manage users, approve instructors, manage tracks, stacks, courses, and communities, and issue badges and certificates |

---

## Design assets

The `Track Finder/Assets` folder contains the project logo, the color palette, and a reference screenshot of the assessment question layout.

---

## Roadmap ideas

- Real payment integration for course purchases
- Localized (English/Arabic) assessment questions
- Automated tests and CI
