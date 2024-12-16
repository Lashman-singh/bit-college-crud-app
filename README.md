# BIT College CRUD Application

## Project Overview

The **BIT College CRUD Application** is a C# and ASP.NET MVC-based project developed to streamline and manage student, course, and enrollment records for a college. This application demonstrates CRUD (Create, Read, Update, Delete) operations, ensuring effective management and retrieval of essential academic data.

---

## Features

### 1. **Student Management**
- Add, edit, view, and delete student information.
- Fields include:
  - Student Name
  - Date of Birth
  - Contact Information

### 2. **Course Management**
- Add, edit, view, and delete course records.
- Fields include:
  - Course Name
  - Course Description
  - Credit Hours

### 3. **Enrollment Management**
- Manage student enrollments for specific courses.
- Track enrollment date and student grades.

### 4. **Search and Filter Options**
- Search functionality for quickly locating students and courses.
- Filters to refine results based on specific criteria.

### 5. **Validation**
- Server-side and client-side validation to ensure data integrity and reliability.

---

## Technologies Used

- **Programming Language**: C#
- **Framework**: ASP.NET MVC
- **Database**: SQL Server
- **Frontend**: Razor Views, HTML, CSS, and Bootstrap
- **Tools**: Visual Studio, Entity Framework

---

## Installation and Setup

1. Clone the repository:
   ```bash
   git clone <repository_url>
   ```

2. Open the project in Visual Studio.

3. Restore NuGet packages:
   - Navigate to **Tools > NuGet Package Manager > Manage NuGet Packages for Solution...**.
   - Restore the required dependencies.

4. Configure the database:
   - Open the `appsettings.json` file.
   - Update the connection string to match your SQL Server setup.

5. Run database migrations:
   ```bash
   Update-Database
   ```

6. Run the application:
   - Press `F5` in Visual Studio or click on **Start** to launch the application.

---

## Usage

1. Navigate to the **Students** section to manage student records.
2. Access the **Courses** section to add or modify course details.
3. Use the **Enrollments** section to enroll students in courses and manage their grades.
4. Utilize search and filter options for efficient data handling.

---

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -m 'Add feature-name'`).
4. Push to the branch (`git push origin feature-name`).
5. Open a Pull Request.
   
---

## Author

- **Lashman Singh**
- Business Information Technology, Red River College

---

## Acknowledgements

- Special thanks to the instructors and peers who supported the development of this project.
- Tools and frameworks used in this project.
