# FinanceProject

A WPF desktop application for managing personal finances in a simple and visual way.

## Purpose

FinanceProject is designed for users who want to track daily income and expenses, review monthly performance, and make better financial decisions with visual insights.

Main goals:

- Register transactions quickly.
- Keep a clear view of monthly balance.
- Understand which categories consume most spending.
- Compare income versus expenses for the current month.

## Features

The application includes:

- Add transactions with description, type (expense or income), date, category, and amount.
- Edit and delete transactions from the main list.
- Filter by category.
- Sort by date, amount, or category.
- Monthly summary with total income, total expenses, and balance.
- Local JSON persistence so data is kept between sessions.

## Charts

The main screen includes two complementary charts:

- Expense distribution by category (pie chart):
  - Includes only expense transactions.
  - Helps quickly identify highest-impact categories.

- Income vs expenses for the current month (vertical bar chart):
  - Compares monthly totals with vertical bars.
  - Makes it easy to see whether the monthly balance is positive or negative.

## UI Mockups (Low-Fidelity)

These mockups are text wireframes meant for documentation and planning.

Main Window layout:

```text
+--------------------------------------------------------------------------------+
| Header: Expense Manager                                                        |
+--------------------------------------------------------------------------------+
| [Description____] [Type v] [Date v] [Category v] [Amount__] [Add]             |
+--------------------------------------------------------------------------------+
| Filter: [Category v]   Sort: [Criteria v]                                      |
+--------------------------------------+-----------------------------------------+
| Transaction List                     | Charts                                  |
| - Date | Category | Desc | Amount    | [Pie: Expenses by Category]             |
| - ...                               | [Bar: Income vs Expenses]                |
+--------------------------------------+-----------------------------------------+
| Monthly Summary: Income | Expenses | Balance                                   |
+--------------------------------------------------------------------------------+
```

Edit Transaction Window layout:

```text
+----------------------------------------------+
| Edit Transaction                             |
+----------------------------------------------+
| Description: [___________________________]   |
| Date:        [______/______/______]          |
| Category:    [_____________________ v]       |
| Amount:      [___________________________]   |
|                                              |
|                          [Cancel] [Save]     |
+----------------------------------------------+
```

## Architecture

The project follows a layered structure inspired by Clean Architecture to reduce coupling and improve maintainability.

- Views/
  - WPF presentation layer.
  - Contains UI windows and code-behind.
  - Example: MainWindow, EditExpenseWindow.

- Models/
  - Domain entities.
  - Example: Expense, TransactionType.

- Services/
  - Business and application rules.
  - Validation, filtering, sorting, monthly summary, and chart model generation.

- Domain/Repositories/
  - Data access contracts (abstractions).
  - Example: IExpenseRepository.

- Infrastructure/Repositories/
  - Concrete persistence implementations.
  - Example: JsonExpenseRepository.

Core rule:

- UI depends on abstractions.
- Infrastructure implements those abstractions.
- Business logic does not depend on storage details.

## Data Persistence

Transactions are stored as JSON in the user's local application data folder (LocalApplicationData). This allows:

- Keeping data after closing the app.
- Avoiding database dependency for this scenario.
- Simpler local setup and execution.

## Requirements

- .NET 8 SDK
- Windows OS (WPF application)

## Run

From the project root:

1. Build:

```bash
dotnet build
```

2. Run:

```bash
dotnet run
```

## Unit Tests

The project includes unit tests for core logic in tests/FinanceProject.Tests.

Current coverage focuses on:

- ExpenseService (validation, filtering, and sorting).
- MonthlySummaryService (monthly summary calculations).
- ChartsService (chart model generation).

Run tests:

```bash
dotnet test
```

## Typical Usage Flow

1. Register monthly income and expenses.
2. Apply filters and sorting to review transactions.
3. Check the footer summary for current balance.
4. Use charts to identify spending patterns and adjust decisions.

## Current State and Next Steps

The project is ready to evolve further toward a full MVVM approach, separating presentation logic even more from code-behind.