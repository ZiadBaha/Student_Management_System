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
  "Email": "ziadbahaa41@gmail.com",
  "Password": "password",
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
| Property     | Type     | Description                                  |
|--------------|----------|----------------------------------------------|
| `statusCode` | `int`    | HTTP-like status code (e.g. 200, 400, 500)   |
| `message`    | `string` | Success or error message                     |
| `data`       | `T`      | Generic response payload (nullable if error) |
```json
// ✅ 200 OK
{
  "statusCode": 200,
  "message": "Fetched successfully.",
  "data": {
    "id": "123",
    "name": "Ahmed"
  }
}

// ❌ 400 Bad Request (Validation)
{
  "statusCode": 400,
  "message": "Invalid input data.",
  "data": null
}

// ❌ 401 Unauthorized
{
  "statusCode": 401,
  "message": "User not authenticated.",
  "data": null
}

// ❌ 500 Internal Server Error
{
  "statusCode": 500,
  "message": "An error occurred. Try again",
  "data": null
}

```
_________________________________________________________

## 📚 Controllers & Endpoints
### 1. AccountController
```http
POST    /api/account/login
POST    /api/account/forget-password
POST    /api/account/verify-otp
POST    /api/account/reset-password
GET     /api/account/confirm-email
POST    /api/account/change-password
PUT     /api/account/update-user-info
GET     /api/account/get-user-info
```

### 2. StudentController
```http
PUT     /api/student/UpdateStudent/{id}
DELETE  /api/student/DeleteStudent/{id}
GET     /api/student/GetAllStudents
GET     /api/student/GetStudentById/{id}
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
POST    /api/course/CreateCourse
PUT     /api/course/UpdateCourse
DELETE  /api/course/DeleteCourse/{id}
GET     /api/course/GetAllCourses
GET     /api/course/GetCourseById/{id}
GET     /api/course/GetEnrolledStudents/{courseId}
GET     /api/course/GetCourseTeachers/{courseId}
```

### 5. DepartmentController
```http
POST    /api/department/CreateDepartment
PUT     /api/department/UpdateDepartment
DELETE  /api/department/DeleteDepartment/{id}
GET     /api/department/GetAllDepartments
GET     /api/department/GetDepartmentById/{id}
GET     /api/department/GetDepartmentTeachers/{departmentId}
GET     /api/department/ExportDepartmentsToExcel
POST    /api/department/ImportDepartmentsFromExcel
```

### 6. StudentCourseController
```http
POST    /api/student-course/add
POST    /api/student-course/remove
PUT     /api/student-course/update
GET     /api/student-course/{studentId}
```

### 7. TeacherCourseController
```http
POST    /api/teacher-course/add
POST    /api/teacher-course/remove
PUT     /api/teacher-course/update
GET     /api/teacher-course/{teacherId}
GET     /api/teacher-course/all-with-courses
```
### 8. AdminController
```http
POST    /api/admin/add-student
POST    /api/admin/add-teacher
POST    /api/admin/import-students
POST    /api/admin/import-teachers
GET     /api/admin/export-students
GET     /api/admin/export-teachers
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
| Package                                                 | Purpose                        |
|---------------------------------------------------------|--------------------------------|
| `FluentValidation (12.0.0)`                             | Model validation              |
| `FluentValidation.DependencyInjectionExtensions (12.0.0)`| Registering validators via DI |
| `Microsoft.AspNetCore.Hosting (2.3.0)`                  | ASP.NET hosting                |
| `Microsoft.AspNetCore.Mvc.NewtonsoftJson (9.0.7)`       | JSON serialization             |
| `Microsoft.AspNetCore.OpenApi (9.0.4)`                  | OpenAPI support                |
| `Microsoft.EntityFrameworkCore.Design (9.0.7)`          | EF Core design-time tools      |
| `Microsoft.EntityFrameworkCore.SqlServer (9.0.7)`       | SQL Server EF provider         |
| `Microsoft.EntityFrameworkCore.Tools (9.0.7)`           | EF Core CLI tools              |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore (9.0.7)` | Identity support with EF |
| `Microsoft.AspNetCore.Mvc.Core (2.3.0)`                 | ASP.NET MVC core               |
| `Microsoft.Extensions.Hosting.Abstractions (9.0.7)`     | Hosting abstractions           |
| `Microsoft.Extensions.Configuration.Aptractions (9.0.7)`| Config abstraction             |
| `Microsoft.AspNetCore.Authentication.JwtBearer (9.0.7)` | JWT authentication             |
| `Newtonsoft.Json (13.0.3)`                              | JSON handling                  |
| `Swashbuckle.AspNetCore (9.0.3)`                        | Swagger UI documentation       |
| `AutoMapper (14.0.0)`                                   | Object mapping                 |
| `ClosedXML (0.105.0)`                                   | Excel export/import            |
| `ExcelDataReader (3.7.0)`                               | Excel file reader              |
| `ExcelDataReader.DataSet (3.7.0)`                       | Excel DataSet support          |
| `MailKit (4.13.0)`                                      | Email sending                  |
| `MimeKit (4.13.0)`                                      | MIME format support            |
| `Otp.NET (1.4.0)`                                       | OTP generation/verification    |
| `FluentAssertions (8.5.0)`                              | Unit test assertions           |
| `Microsoft.EntityFrameworkCore.InMemory (9.0.7)`        | In-memory EF DB for testing    |
| `Microsoft.NET.Test.Sdk (17.14.1)`                      | xUnit test SDK                 |
| `Moq (4.20.72)`                                         | Mocking for unit tests         |
| `xunit (2.9.3)`                                         | Unit testing framework         |
| `xunit.runner.visualstudio (3.1.3)`                     | Test runner support            |
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
