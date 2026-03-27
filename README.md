# FinanceProject

A simple WPF application for managing personal expenses.

## Architecture

The project now follows a layered structure inspired by Clean Architecture:

- `Models/`: domain entities (`Expense`, `TransactionType`).
- `Views/`: WPF views and code-behind (`MainWindow`, `EditExpenseWindow`).
- `Services/`: domain/application rules (validation, filtering, sorting, charts, monthly summary).
- `Domain/Repositories/`: abstractions (contracts), for example `IExpenseRepository`.
- `Infrastructure/Repositories/`: external implementation details (JSON persistence).

Main rule: UI depends on abstractions, and infrastructure implements those abstractions.

## Requirements

- .NET 8 SDK

## How to Run

1. Install .NET 8 SDK from https://dotnet.microsoft.com/download
2. Open the project in VS Code
3. Run `dotnet build` to build
4. Run `dotnet run` to launch

## Features

- Add expenses with description and amount
- View list of expenses
- Filter and sort by category/date/amount
- Monthly income, expenses and balance summary
- Charts for category distribution and income vs expenses