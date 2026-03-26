using FinanceProject.Models;

namespace FinanceProject.Services;

public class MonthlySummary
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal Balance { get; set; }
}

public class MonthlySummaryService
{
    /// <summary>
    /// Obtiene resumen mensual de gastos e ingresos
    /// </summary>
    public MonthlySummary GetMonthlySummary(List<Expense> expenses, int year, int month)
    {
        var monthExpenses = expenses
            .Where(e => e.Date.Year == year && e.Date.Month == month)
            .ToList();

        var totalExpenses = monthExpenses
            .Where(e => e.TransactionType == TransactionType.Gasto)
            .Sum(e => e.Amount);

        var totalIncome = monthExpenses
            .Where(e => e.TransactionType == TransactionType.Ingreso)
            .Sum(e => e.Amount);

        var balance = totalIncome - totalExpenses;

        return new MonthlySummary
        {
            Year = year,
            Month = month,
            MonthName = new DateTime(year, month, 1).ToString("MMMM yyyy"),
            TotalExpenses = totalExpenses,
            TotalIncome = totalIncome,
            Balance = balance
        };
    }

    /// <summary>
    /// Obtiene resumen de últimos 12 meses
    /// </summary>
    public List<MonthlySummary> GetLast12MonthsSummary(List<Expense> expenses)
    {
        var summaries = new List<MonthlySummary>();
        var today = DateTime.Now;

        for (int i = 11; i >= 0; i--)
        {
            var date = today.AddMonths(-i);
            summaries.Add(GetMonthlySummary(expenses, date.Year, date.Month));
        }

        return summaries;
    }

    /// <summary>
    /// Resumen total actual (mes actual)
    /// </summary>
    public MonthlySummary GetCurrentMonthSummary(List<Expense> expenses)
    {
        var today = DateTime.Now;
        return GetMonthlySummary(expenses, today.Year, today.Month);
    }
}
