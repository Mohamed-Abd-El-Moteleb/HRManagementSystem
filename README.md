# HR Management System (Web API)

**HR Management System** is a robust, enterprise-grade backend solution built with ASP.NET Core Web API. It is designed using Clean Architecture and Domain-Driven Design (DDD) principles to provide a scalable, maintainable, and highly secure platform for managing organizational hierarchies and personnel.

## Architecture & Design Patterns
- **Domain Layer:** Contains Rich Entities, Value Objects, and Domain Logic (The heart of the system).
- **Application Layer:** Contains DTOs, Mapping Profiles, and Application Services (Orchestration).
- **Infrastructure Layer:** Data persistence using Entity Framework Core and Repository Pattern.
- **Presentation (API):** RESTful endpoints with Swagger documentation.
  
## Technical Excellence (DDD & Quality)
- **DDD Implementation:** Implements Value Objects for Email, Phone, National ID (14 digits), and Salary to ensure business integrity at the lowest level.
- **Rich Domain Models:** Business rules are encapsulated within Entities (e.g., age validation, contract date logic) to prevent "Anemic Domain Models."
- **Advanced Validation:** A multi-layered validation approach using FluentValidation for request DTOs and Domain exceptions for business rules.
- **Automated Mapping:** Seamless data transformation between Entities and DTOs using AutoMapper.
- **Comprehensive Employee Management:** Handles complex profiles, profile pictures, and department transitions.
- **Department Governance:** Secure management of departments with logic to handle "Unassigned" states and manager assignments.
  
## Features
**Employee Management**
- Full Lifecycle: Add, edit, and view comprehensive employee profiles. 
- Employment Details: Manage job titles, hire dates, and contract details with automated logic.
- Media Support: Integrated profile picture management.
- Dynamic Assignment: Seamlessly transfer employees between departments while maintaining history.
  
**Department Governance**
- Structural Management: CRUD operations for departments with Data Protection (Prevents deletion of departments with active employees).
- Unassigned Safety Net: Built-in logic to handle employees in the 'Unassigned' category to avoid orphan records.
- Managerial Roles: Assign and track department managers with validation to ensure they belong to the correct unit.

## Technologies Used

- **Framework:** .NET 9 (ASP.NET Core Web API)
- **Language:** C#
- **Database:** SQL Server, Entity Framework Core
- **ORM**: Entity Framework Core (Code First)
- **Mapping**: AutoMapper
- **Documentation**: Swagger (OpenAPI)

## Target Users

- HR Managers  
- Team Leaders  
- Admins in mid-sized organizations  

## Getting Started
 **Prerequisites**:
 - .NET 9.0 SDK or later
 - SQL Server
   
**1. Clone the repository:**
   ```bash
   git clone https://github.com/Mohamed-Abd-El-Moteleb/HRManagementSystem.git
   ```
**2. Open the solution in Visual Studio.**

**3. Update the "appsettings.json" with your SQL Server connection string.**
   
**5. Apply migrations to create the database:**
    ```bash
   dotnet ef database update
    ```

**7. Run the project and enjoy managing your HR system!**

## Testing

The system is designed for high testability. You can test the business logic via:

- Unit Tests: Testing Value Objects and Domain Logic.

- Postman: A collection is provided (optional) to test RESTful endpoints.

## Author
**Mohamed Abd El-Moteleb** Full Stack Developer
