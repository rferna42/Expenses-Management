# FinanceProject

A Blazor web application for managing personal finances with a month-based workflow, SQLite persistence, and visual monthly summaries.

## Purpose

FinanceProject is designed for tracking income and expenses month by month from a browser, reviewing a selected month in detail, and understanding spending behavior through lightweight charts and summaries.

Main goals:

- Register transactions quickly.
- Review activity for a specific month.
- Keep a clear view of income, expenses, and balance.
- Understand which categories consume most spending.
- Compare income versus expenses for the selected month.

## Current Features

The application currently supports:

- Add transactions with description, type, date, category, and amount.
- Edit and delete transactions directly from the main list.
- Display whether each movement is an income or an expense.
- Select a month to view only that month's movements.
- Automatically refresh summary and charts for the selected month.
- Filter visible movements by category.
- Sort visible movements by date, amount, or category.
- Persist data locally using SQLite.

## Web UI Overview

The current web application includes:

- A transaction form at the top of the page.
- A toolbar with month, category, and sort selectors.
- A monthly summary row placed above the movements and charts area for better visibility.
- A main transaction list with inline editing.
- A category distribution chart rendered as a donut.
- An income vs expenses comparison rendered as a bar chart.

Date handling is standardized in the application logic around `dd/MM/yyyy`.

## Monthly Workflow

The app is organized around a selected month:

- The month selector determines which movements are visible.
- The list, summary, and both charts are recalculated for that month.
- Adding a transaction keeps the selected month in sync with the newly saved movement.
- Editing or deleting movements refreshes the available month list so the UI cannot drift out of sync.

## Architecture

The project now combines reusable business logic with a Blazor web front end.

- Components/
  - Blazor UI components and layouts.
  - Main page: `Components/Pages/Home.razor`.

- Models/
  - Domain and UI support models.
  - Examples: `Expense`, `TransactionType`, `ExpenseSortOption`, `MonthOption`.

- Services/
  - Business and application rules.
  - Includes validation, sorting, monthly summaries, and chart-oriented data logic.

- Domain/Repositories/
  - Data access contracts.
  - Example: `IExpenseRepository`.

- Infrastructure/Repositories/
  - Concrete persistence implementation.
  - Current implementation: `SqliteExpenseRepository`.

Core rule:

- UI depends on abstractions and domain services.
- Persistence is isolated in the repository layer.
- Business logic remains reusable outside the UI.

## Data Persistence

Transactions are stored locally in SQLite.

Current storage details:

- Database engine: SQLite
- File name: `expenses.db`
- Folder: `LocalApplicationData/FinanceProject`

Additional persistence improvements applied:

- Each transaction now has a stable Id.
- Amounts are stored as text using invariant decimal serialization to avoid floating-point precision issues.
- Saves now use upsert and delete-missing behavior instead of deleting and recreating the entire table every time.
- Legacy SQLite schemas are migrated automatically on startup.

## Categories

The application currently includes these categories:

- Shopping
- Rent
- Dining
- Food
- Fuel
- Health
- Bills
- Loans
- Utilities
- Entertainment
- Travels
- Other
- Income

## Requirements

- .NET 10 SDK

## Run

From the project root:

1. Build

```bash
dotnet build
```

2. Run the web app

```bash
dotnet run
```

Then open the local URL shown in the console, for example:

```text
http://localhost:5072
```

## Tests

The project includes tests in `tests/FinanceProject.Tests`.

Current coverage includes:

- `ExpenseService` validation, filtering, and sorting.
- `MonthlySummaryService` monthly summary calculations.
- `SqliteExpenseRepository` persistence, migration, decimal-safe storage, and delete/update behavior.

Run tests:

```bash
dotnet test
```

## Typical Usage Flow

1. Select the month you want to review.
2. Add income and expenses with the appropriate date and category.
3. Filter or sort the visible list if needed.
4. Edit or delete movements inline.
5. Review the monthly summary and charts.

## Current State

The application now runs as a web app instead of a desktop app and currently provides:

- Blazor-based UI.
- SQLite-based local persistence.
- Month-based transaction browsing.
- Inline editing and deletion.
- Visual monthly charts and summary cards.
- Automated tests for services and repository behavior.
