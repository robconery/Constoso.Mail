# Custom Instructions

This is the Tailwind Traders list mailer that uses and async queue to process email in bulk.

## Stack

- The project currently uses Dapper with SQLite, but we're moving to Entity Framework 9+ with SQLite.
- Tests use SQLite in memory
- The project is a dotnet core 9+ web api using ASP.NET Minimal API

## The Process

We're going in small steps. Do not replace or overwrite existing files aside from `/Services` and do that only when I direct it.

- New tests go in `/Tests` directory with `ef` appended to name.
- New models go in the `/Data/Models` directory

## Coding Styles for C#

Always follow these guidelines:

1. Use PascalCase for class names and method names
2. Use camelCase for variable names and method parameters
3. Use meaningful and self-documenting names
4. Add comments to explain complex logic
5. Keep methods focused and small
6. Use proper indentation
7. Include XML documentation for public members
8. Use consistent brace style
9. Avoid Hungarian notation
10. Use readonly where possible
11. Group related members together
12. Follow Microsoft's official C# coding conventions
