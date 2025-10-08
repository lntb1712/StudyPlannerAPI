````markdown
# 📘 StudyPlannerAPI

**StudyPlannerAPI** is a backend system built with **ASP.NET Core**, designed to help users efficiently organize and manage their study plans, tasks, and schedules.  
It serves as the backend service for a personal study planner web or mobile application, following **SOLID principles** and clean architecture for scalability and maintainability.

---

## 🚀 Key Features

- 🔐 **Authentication & Authorization** — Secure user registration and login using **JWT**.  
- 🗓️ **Study Plan Management** — Create, update, delete, and track learning goals and schedules.  
- ✅ **Task Management** — Manage study tasks with status tracking and due dates.  
- 📡 **Real-time Notifications (SignalR)** — Receive instant updates and reminders when tasks or schedules are modified.  
- ☁️ **Cloud Storage (Cloudinary)** — Upload, store, and manage images or files seamlessly in the cloud.  
- 📊 **Progress Tracking** — Monitor learning statistics and completion rates.  
- 🧱 **SOLID & Clean Architecture** — Ensures the system is easy to extend, maintain, and test.  
- ⚙️ **RESTful API** — Structured endpoints ready for frontend or mobile integration.  

---

## 🧱 Tech Stack

| Component | Technology |
|------------|-------------|
| **Language** | C# |
| **Framework** | ASP.NET Core Web API |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server |
| **Real-time Communication** | SignalR |
| **Cloud Storage** | Cloudinary |
| **Authentication** | JWT (JSON Web Token) |
| **Architecture** | SOLID, Clean Architecture |
| **Logging** | Serilog / Built-in .NET Logger |
| **API Documentation** | Swagger / Swashbuckle |

---

## ⚙️ Getting Started

### 1️⃣ Clone the repository
```bash
git clone https://github.com/lntb1712/StudyPlannerAPI.git
cd StudyPlannerAPI
````

### 2️⃣ Install dependencies

```bash
dotnet restore
```

### 3️⃣ Configure settings

Edit `appsettings.json` with your environment-specific values:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=StudyPlannerDB;Trusted_Connection=True;"
},
"Jwt": {
  "Key": "your-secret-key",
  "Issuer": "your-issuer",
  "Audience": "your-audience"
},
"Cloudinary": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
}
```

### 4️⃣ Apply migrations & create database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5️⃣ Run the API

```bash
dotnet run
```

Then open:
👉 `https://localhost:{port}/swagger`
to explore and test all available API endpoints.

---

## 📁 Project Structure

```
StudyPlannerAPI/
├── Controllers/
├── Models/
├── DTOs/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Repositories/
├── Data/
├── Hubs/                # SignalR Hubs for real-time features
├── Helpers/             # Cloudinary and utility helpers
├── appsettings.json
├── Program.cs
└── StudyPlannerAPI.sln
```
---

## ⚙️ Environment Variables

| Key                                    | Description                            |
| -------------------------------------- | -------------------------------------- |
| `ASPNETCORE_ENVIRONMENT`               | Environment (Development / Production) |
| `ConnectionStrings__DefaultConnection` | Database connection string             |
| `Jwt__Key`                             | JWT Secret Key                         |
| `Jwt__Issuer`                          | Token issuer                           |
| `Jwt__Audience`                        | Token audience                         |
| `Cloudinary__CloudName`                | Cloudinary cloud name                  |
| `Cloudinary__ApiKey`                   | Cloudinary API key                     |
| `Cloudinary__ApiSecret`                | Cloudinary API secret                  |

---

## 🧪 Testing

* Unit tests for **services** and **repositories**
* Integration tests for **controllers** and **database**
* Mocked **SignalR** and **Cloudinary** dependencies for isolated testing
* Recommended libraries: `xUnit`, `Moq`, `FluentAssertions`

---

## 🧱 SOLID Principles Applied

The project follows **SOLID design principles** to ensure maintainability and scalability:

* **S** – Single Responsibility: Each class has a focused responsibility.
* **O** – Open/Closed: Extend functionalities without modifying core classes.
* **L** – Liskov Substitution: Interface-based abstractions ensure flexibility.
* **I** – Interface Segregation: Separate, small interfaces for specific roles.
* **D** – Dependency Inversion: High-level modules depend on abstractions, not implementations.

This design pattern ensures the system remains modular, testable, and easily adaptable.

---

## 🚢 Deployment

* Supports **Docker** containerization.
* Environment variables configured via `.env` or CI/CD pipeline.
* Enable HTTPS, CORS, and secure JWT tokens in production.

---

## 🤝 Contributing

Contributions are always welcome!
To contribute:

1. Fork this repository
2. Create a new branch (`feature/my-feature`)
3. Commit and push your changes
4. Submit a Pull Request

---


### 👨‍💻 Author

**Thanh Bình Lê Nguyễn**
📧 [[nhocbinh7a8@gmail.com](nhocbinh7a8@gmail.com)]

---

> “A clean architecture makes great software last longer.”
> — Inspired by Uncle Bob & Microsoft Clean Architecture

```

---

Would you like me to make this version include **actual class names, endpoints, and database entities** from your code (like `UserController`, `TaskService`, etc.) so it looks *fully personalized* to your repo?  
If yes, please upload your `/Controllers` folder or `Program.cs`.
```
