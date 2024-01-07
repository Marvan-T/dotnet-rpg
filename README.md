[![Unit Tests](https://github.com/Marvan-T/dotnet-rpg/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Marvan-T/dotnet-rpg/actions/workflows/dotnet.yml)

# dotnet-rpg

This is a personal project that I've built to enhance my skills in ASP.NET Core following a Udemy course. The project is a role-playing game (RPG) API that provides endpoints for managing characters, weapons, and skills in the game.

## Features

- **Character Management**: The [`CharacterController`](command:_github.copilot.openSymbolInFile?%5B%22dotnet-rpg%2FControllers%2FCharacterController.cs%22%2C%22CharacterController%22%5D "dotnet-rpg/Controllers/CharacterController.cs") provides endpoints for managing game characters.
- **Weapon and Skill Management**: The [`WeaponController`](command:_github.copilot.openSymbolInFile?%5B%22dotnet-rpg%2FControllers%2FWeaponController.cs%22%2C%22WeaponController%22%5D "dotnet-rpg/Controllers/WeaponController.cs") and [`SkillController`](command:_github.copilot.openSymbolInFile?%5B%22dotnet-rpg%2FControllers%2FSkillController.cs%22%2C%22SkillController%22%5D "dotnet-rpg/Controllers/SkillController.cs") provide endpoints for managing weapons and skills respectively.
- **Score Management**: The [`ScoreController`](command:_github.copilot.openSymbolInFile?%5B%22dotnet-rpg%2FControllers%2FScoreController.cs%22%2C%22ScoreController%22%5D "dotnet-rpg/Controllers/ScoreController.cs") provides an endpoint for retrieving character scores.
- **Automated Tests**: The project includes a suite of automated tests. Check out the [`dotnet-rpg.Tests`](command:_github.copilot.openRelativePath?%5B%22dotnet-rpg.Tests%22%5D "dotnet-rpg.Tests") directory for more details.
- **Authentication**: The project uses JWT for authentication. Check out the [`AuthController`](command:_github.copilot.openSymbolInFile?%5B%22dotnet-rpg%2FControllers%2FAuthController.cs%22%2C%22AuthController%22%5D "dotnet-rpg/Controllers/AuthController.cs") for more details.

## Learning Journey

I've learned a lot about ASP.NET Core, Entity Framework Core, and other related technologies building this. I've also learned about best practices in building APIs and writing clean, maintainable code.

While the core functionality of this project is based on the Udemy course, I've incorporated several enhancements based on my own insights and experiences:

- **Unit Tests**: I've added unit tests following the best practices that I've adhered to in my previous projects. 
- **Design Patterns**: The project employs a few design patterns, including the Strategy and Specification patterns. While these might seem overkill for a project of this scale, they were implemented as a learning experience to understand their practical applications.
- **Architecture Design Records (ADRs)**: I've utilized ADRs to document a few key decisions made during the development process. You can find these in the ADR directory.
- **Repository Pattern**: To abstract the data layer and ensure a clean separation of concerns, I've implemented the Repository pattern.

## How to Run

To run this project, you need to have .NET 7.0 SDK installed on your machine. Once you have that, you can clone this repository and navigate to the [`dotnet-rpg`](command:_github.copilot.openRelativePath?%5B%22dotnet-rpg%22%5D "dotnet-rpg") directory. From there, you can run the following command in your terminal:

```sh
dotnet run
```

