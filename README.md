# FinanceProject

A WPF desktop application for managing personal finances with a month-based workflow, local persistence, and visual summaries.

## Purpose

FinanceProject is intended for users who want to register income and expenses month by month, review financial activity for a selected month, and understand spending patterns through charts.

Main goals:

- Register transactions quickly.
- Review movements for a specific month.
- Keep a clear view of monthly income, expenses, and balance.
- Understand which categories consume most spending.
- Compare income versus expenses for the selected month.

## Current Features

The application currently supports:

- Add transactions with description, type, date, category, and amount.
- Edit and delete transactions directly from the main list.
- Display whether each movement is an income or an expense in the transaction list.
- Select a specific month to view only the movements for that month.
- Automatically calculate summary and charts for the selected month.
- Filter the visible list by category.
- Sort the visible list by date, amount, or category.
- Persist data locally between sessions using SQLite.

## Monthly Workflow

The app is now organized around a selected month:

- The top filter bar includes a month selector.
- The transaction list shows only the movements of the selected month.
- The summary footer updates for the selected month.
- Both charts update for the selected month.
- When a transaction is added, it is stored using the date selected in the DatePicker.

Date handling is standardized to `dd/MM/yyyy` in the UI.

## Charts

The main screen includes two complementary charts:

- Expense distribution by category (pie chart):
  - Includes only expense transactions.
  - Uses the currently selected month.
  - Helps identify the categories with the highest spending.

- Income vs expenses (vertical bar chart):
  - Compares total income and total expenses for the selected month.
  - Makes it easy to see whether the balance is positive or negative.

## UI Mockups (Low-Fidelity)

These mockups are text wireframes meant for documentation and planning.

Main Window layout:

```text
+--------------------------------------------------------------------------------------------------+
| Header: Expense Manager                                                                          |
+--------------------------------------------------------------------------------------------------+
| [Description____] [Type v] [Date dd/MM/yyyy v] [Category v] [Amount__] [Add]                    |
+--------------------------------------------------------------------------------------------------+
| Month: [March 2026 v]   Filter: [Category v]   Sort: [Criteria v]                                |
+-----------------------------------------------+--------------------------------------------------+
| Transaction List                              | Charts                                           |
| - Date | Type | Category | Desc | Amount      | [Pie: Expenses by Category]                      |
| - ...                                        | [Bar: Income vs Expenses for selected month]     |
+-----------------------------------------------+--------------------------------------------------+
| Monthly Summary: Income | Expenses | Balance                                                 |
+--------------------------------------------------------------------------------------------------+
```

## Architecture

The project follows a layered structure inspired by Clean Architecture to reduce coupling and improve maintainability.

- Views/
  - WPF presentation layer.
  - Contains UI windows and code-behind.
  - Main logic is currently centered in `MainWindow`.

- Models/
  - Domain entities and UI support models.
  - Examples: `Expense`, `TransactionType`, `ExpenseSortOption`, `MonthOption`.

- Services/
  - Business and application logic.
  - Includes validation, filtering, sorting, monthly summary calculation, and chart model generation.

- Domain/Repositories/
  - Data access contracts.
  - Example: `IExpenseRepository`.

- Infrastructure/Repositories/
  - Concrete persistence implementation.
  - Example: `SqliteExpenseRepository`.

Core rule:

- UI depends on abstractions.
- Infrastructure implements those abstractions.
- Business logic does not depend on storage details.

## Data Persistence

Transactions are stored locally in SQLite.

Current storage details:

- Database engine: SQLite
- File name: `expenses.db`
- Folder: `LocalApplicationData/FinanceProject`

This provides:

- Data persistence between sessions.
- Lightweight local storage with no external server.
- A simpler setup than a remote or full relational database deployment.

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

- .NET 8 SDK
- Windows OS (WPF application)

## Run

From the project root:

1. Build

```bash
dotnet build
```

2. Run

```bash
dotnet run
```

## Unit Tests

The project includes unit tests for the core logic in `tests/FinanceProject.Tests`.

Current coverage focuses on:

- `ExpenseService` for validation, filtering, and sorting.
- `MonthlySummaryService` for monthly summary calculations.
- `ChartsService` for chart model generation and month-specific chart behavior.

Run tests:

```bash
dotnet test
```

## Typical Usage Flow

1. Select the month you want to review.
2. Add income and expenses with the appropriate date and category.
3. Filter or sort the visible transactions if needed.
4. Review the summary footer for income, expenses, and balance.
5. Use the charts to understand category spending and compare income versus expenses.

## Current State

The application currently provides a functional month-based personal finance tracker with:

- English UI and documentation.
- SQLite-based local persistence.
- Category-based filtering and sorting.
- Monthly summaries and charts.
- Inline editing from the main transaction list.

The next logical architectural step would be to move more presentation behavior from code-behind to a full MVVM structure.