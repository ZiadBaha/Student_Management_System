# 🎓 Student Management System - ASP.NET Core Web API Project
_________________________________________________________

## 📘 Introduction
The Student Management System (SMS) is a robust and extensible ASP.NET Core Web API solution designed to streamline the management of students, teachers, departments, courses, authentication, and academic activities. Developed using modern architecture principles, it supports secure, scalable, and maintainable backend infrastructure for educational institutions.
_________________________________________________________

## 📦 Project Overview
This system is built using layered architecture principles and integrates:
- Secure authentication with JWT and ASP.NET Identity
- Role-based access control
- RESTful APIs
- FluentValidation
- AutoMapper
- Full email integration
- Excel import/export for bulk data operations
- Middleware for exception handling and request logging
- Logging via ILogger
_________________________________________________________

## 💻 Technologies Used
- **ASP.NET Core 9**
- **C#**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Identity**
- **JWT Authentication**
- **AutoMapper**
- **FluentValidation**
- **Swagger / Swashbuckle**
- **EPPlus or ClosedXML** (Excel support)
- **MailKit & MimeKit** (SMTP Email)
- **ILogger** (Logging)
_________________________________________________________

## 📁 Project Structure

| Project Part          | Description                                          |
|---------------------- |------------------------------------------------------|
| **Controllers**       | Contains all API endpoint controllers                |
| **DTOs**              | Data Transfer Objects used for input/output          |
| **Models**            | EF Core models for Students, Teachers, Courses, etc. |
| **Services**          | Interfaces and implementations of business logic     |
| **Middlewares**       | Custom middleware for logging and error handling     |
| **Validators**        | FluentValidation classes for input validation        |
| **Helpers**           | Mapping profiles for AutoMapper                      |
| **wwwroot/Templates** | Email HTML templates                                 |

______

## 🔐 Authentication & Authorization
- **JWT Token-based Authentication**
- **Roles:** `Admin`, `Teacher`, `Student`
- **Admin** creates users with email confirmed by default
- **No OTP or confirmation links** for admin-created accounts
- **Role-based endpoint protection**
_________________________________________________________

## 💌 Mail Configuration
```json
"MailSettings": {
  "Email": "example@gmail.com",
  "Password": "your-password",
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "DisplayedName": "Student Management System"
}
```
- HTML templates support placeholders like:
  - `{{FullName}}`
  - `{{Email}}`
  - `{{Password}}`
_________________________________________________________

## 📡 Standard API Response
```json
// Success:
{
  "success": true,
  "message": "Student added successfully.",
  "data": {
    "id": 1,
    "name": "Ahmed Ali"
  }
}

// Error:
{
  "success": false,
  "message": "Validation failed.",
  "data": null
}
```
_________________________________________________________

## 📚 Controllers & Endpoints
### 1. AccountController
```http
POST    /api/account/login
POST    /api/account/register
GET     /api/account/get-users
GET     /api/account/get-user-info/{id}
DELETE  /api/account/delete-user/{id}
PUT     /api/account/update-user
```

### 2. StudentController
```http
POST    /api/student/add
GET     /api/student/get-all
GET     /api/student/{id}
PUT     /api/student/update/{id}
DELETE  /api/student/delete/{id}
```

### 3. TeacherController
```http
POST    /api/teacher/add
GET     /api/teacher/get-all
GET     /api/teacher/{id}
PUT     /api/teacher/update/{id}
DELETE  /api/teacher/delete/{id}
```

### 4. CourseController
```http
POST    /api/course/add
GET     /api/course/get-all
PUT     /api/course/update/{id}
DELETE  /api/course/delete/{id}
```

### 5. DepartmentController
```http
POST    /api/department/create
GET     /api/department/get-all
PUT     /api/department/update/{id}
DELETE  /api/department/delete/{id}
```

### 6. StudentCourseController
```http
POST    /api/student-course/add
PUT     /api/student-course/update
DELETE  /api/student-course/remove
GET     /api/student-course/{studentId}
```

### 7. TeacherCourseController
```http
POST    /api/teacher-course/add
PUT     /api/teacher-course/update
DELETE  /api/teacher-course/remove
GET     /api/teacher-course/{teacherId}
```
_________________________________________________________

## 📥 Excel Import/Export
- Import/Export via EPPlus or ClosedXML
- Applies to: Students, Teachers, Courses
- Example:
```csharp
public async Task<ApiResponse<string>> ImportStudentsAsync(IFormFile file)
```
_________________________________________________________

## ✅ Validation using FluentValidation
```csharp
public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
```
_________________________________________________________

## 🔁 AutoMapper Usage
```csharp
CreateMap<AddStudentDto, Student>();
CreateMap<Student, StudentResponseDto>();
```
Auto-registered via DI.
_________________________________________________________

## 🔥 Middleware
- `ExceptionHandlingMiddleware`
- `RequestLoggingMiddleware`
- `ValidationFilter`
_________________________________________________________

## 🧪 Unit Testing
- Test Framework: XUnit
- Tests: Services Layer
- Approach: Mock DbContext & dependency injection
_________________________________________________________

## 📦 NuGet Packages
| Package | Purpose |
|---------|---------|
| `Microsoft.EntityFrameworkCore.SqlServer` | EF Core DB provider |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | Identity |
| `Swashbuckle.AspNetCore` | Swagger docs |
| `AutoMapper` | DTO Mapping |
| `FluentValidation.AspNetCore` | Input validation |
| `MailKit`, `MimeKit` | Email handling |
| `System.Drawing.Common` | File/Image handling |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT authentication |
_________________________________________________________

## 🏁 Run the Project Locally
```bash
git clone https://github.com/your-username/sms-project.git
cd sms-project
dotnet restore
dotnet ef database update
dotnet run
```
Visit `https://localhost:{port}/swagger` to explore the API
_________________________________________________________
## 🏗️ Architecture & Design

The project follows a clean, modular architecture with clear separation of concerns:

| Layer        | Description |
|--------------|-------------|
| **API**      | Contains all controllers, middlewares, and configurations for request handling. It's the entry point of the application. |
| **Core**     | Holds domain models, DTOs, enums, interfaces, validators, and mapping profiles. This is the heart of business logic and contracts. |
| **Repository** | Contains the Entity Framework DbContext, migrations, and implementations of data access logic (e.g., repositories) and Implements application services, interacts with Core interfaces, handles business logic, email sending, Excel import/export, and logging. |
| **Service**  |  |
| **Tests**    | Unit testing project using XUnit. Includes service-level tests, validator tests, and mocks for external dependencies. |

📌 Each layer is fully decoupled and communicates through interfaces, promoting testability and long-term maintainability.
_________________________________________________________


## 🧑‍💻 Author
- Ziad Bahaa
- Backend Developer – ASP.NET | Clean Architecture
- 🔗 [LinkedIn](https://www.linkedin.com/in/ziad-bahaa-b04561265/)  
- 🐙 [GitHub](https://github.com/ZiadBaha)
- 📧 [Email](ziadbahaa41@gmail.com)
- 📞 [Phone](01022673000)

_________________________________________________________
