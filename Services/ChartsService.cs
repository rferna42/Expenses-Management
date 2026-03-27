using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FinanceProject.Configuration;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class ChartsService
{
    /// <summary>
    /// Builds a pie chart with expense totals by category.
    /// </summary>
    public PlotModel CreateCategoryExpensesChart(List<Expense> expenses)
    {
        // Include only expense transactions in the category chart.
        var onlyExpenses = expenses.Where(e => e.TransactionType == TransactionType.Expense).ToList();
        
        var model = new PlotModel 
        { 
            Title = AppConfiguration.PieChartTitle,
            TitleFontSize = 16,
            Background = OxyColor.FromRgb(255, 255, 255),
            Padding = new OxyThickness(10, 10, 10, 10)
        };
        
        var pieSeries = new PieSeries 
        { 
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.65,
            FontSize = 10,
            FontWeight = OxyPlot.FontWeights.Bold,
            TextColor = OxyColor.FromRgb(255, 255, 255)
        };

        // Group expenses by category while excluding income category label.
        var groupedExpenses = onlyExpenses
            .Where(e => e.Category != AppConfiguration.IncomeCategory)
            .GroupBy(e => e.Category)
            .Select(g => new
            {
                Category = g.Key,
                Total = g.Sum(e => e.Amount)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        if (groupedExpenses.Count == 0)
        {
            return model;
        }

        // Predefined palette for consistent category colors.
        var colors = new OxyColor[]
        {
            OxyColor.FromRgb(255, 87, 34),
            OxyColor.FromRgb(233, 30, 99),
            OxyColor.FromRgb(63, 81, 181),
            OxyColor.FromRgb(0, 150, 136),
            OxyColor.FromRgb(156, 39, 176),
            OxyColor.FromRgb(244, 67, 54),
            OxyColor.FromRgb(76, 175, 80),
            OxyColor.FromRgb(255, 152, 0),
            OxyColor.FromRgb(33, 150, 243),
            OxyColor.FromRgb(139, 195, 74)
        };

        for (int i = 0; i < groupedExpenses.Count; i++)
        {
            var item = groupedExpenses[i];
            var color = colors[i % colors.Length];
            
            pieSeries.Slices.Add(new PieSlice(
                $"{item.Category} €{item.Total:F1}",
                (double)item.Total
            )
            {
                Fill = color
            });
        }

        model.Series.Add(pieSeries);
        return model;
    }

    /// <summary>
    /// Builds the current-month income versus expenses chart.
    /// </summary>
    public PlotModel CreateIncomeVsExpensesChart(List<Expense> expenses)
    {
        var today = DateTime.Now;
        
        // Keep only current-month transactions.
        var monthExpenses = expenses
            .Where(e => e.Date.Year == today.Year && e.Date.Month == today.Month)
            .ToList();

        var totalExpenses = monthExpenses
            .Where(e => e.TransactionType == TransactionType.Expense)
            .Sum(e => e.Amount);

        var totalIncome = monthExpenses
            .Where(e => e.TransactionType == TransactionType.Income)
            .Sum(e => e.Amount);

        var model = new PlotModel
        {
            Title = $"{AppConfiguration.IncomeVsExpensesChartTitle} - {today:MMMM yyyy}",
            TitleFontSize = 16,
            Background = OxyColor.FromRgb(255, 255, 255),
            Padding = new OxyThickness(60, 20, 20, 60)
        };

        // X axis categories.
        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Key = "Categories",
            FontSize = 12
        };
        categoryAxis.Labels.Add("Income");
        categoryAxis.Labels.Add("Expenses");
        model.Axes.Add(categoryAxis);

        // Y axis values.
        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Key = "Values",
            FontSize = 12,
            Title = AppConfiguration.AmountAxisTitle,
            TitleFontSize = 12
        };
        model.Axes.Add(valueAxis);

        // Vertical bars using RectangleBarSeries for OxyPlot 2.2 compatibility.
        var barsSeries = new RectangleBarSeries
        {
            XAxisKey = "Categories",
            YAxisKey = "Values",
            StrokeThickness = 0
        };

        // x0, y0, x1, y1
        barsSeries.Items.Add(new RectangleBarItem(-0.3, 0, 0.3, (double)totalIncome)
        {
            Color = OxyColor.FromRgb(76, 175, 80)
        });
        
        barsSeries.Items.Add(new RectangleBarItem(0.7, 0, 1.3, (double)totalExpenses)
        {
            Color = OxyColor.FromRgb(244, 67, 54)
        });

        model.Series.Add(barsSeries);
        return model;
    }

    /// <summary>
    /// Returns total expenses grouped by category.
    /// </summary>
    public Dictionary<string, decimal> GetExpensesByCategory(List<Expense> expenses)
    {
        return expenses
            .Where(e => e.TransactionType == TransactionType.Expense && e.Category != AppConfiguration.IncomeCategory)
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount))
            .OrderByDescending(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
