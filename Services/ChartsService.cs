using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class ChartsService
{
    /// <summary>
    /// Crea un gráfico de pastel mostrando gastos por categoría (solo gastos, excluye ingresos)
    /// </summary>
    public PlotModel CreateCategoryExpensesChart(List<Expense> expenses)
    {
        // Filtrar solo gastos
        var onlyExpenses = expenses.Where(e => e.TransactionType == TransactionType.Gasto).ToList();
        
        var model = new PlotModel 
        { 
            Title = "Distribución de Gastos por Categoría", 
            TitleFontSize = 16,
            Background = OxyColor.FromRgb(255, 255, 255),
            Padding = new OxyThickness(10, 10, 10, 10)
        };
        
        var pieSeries = new PieSeries 
        { 
            StrokeThickness = 2.0,
            InsideLabelPosition = 0.65, // Etiquetas dentro del pastel
            FontSize = 10,
            FontWeight = OxyPlot.FontWeights.Bold,
            TextColor = OxyColor.FromRgb(255, 255, 255) // Texto blanco
        };

        // Agrupar gastos por categoría (excluyendo "Ingresos")
        var groupedExpenses = onlyExpenses
            .Where(e => e.Category != "Ingresos")
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
            return model; // Retornar gráfico vacío si no hay gastos
        }

        // Definir colores vibrantes y diferenciados
        var colors = new OxyColor[]
        {
            OxyColor.FromRgb(255, 87, 34),      // Naranja profundo
            OxyColor.FromRgb(233, 30, 99),      // Rosa
            OxyColor.FromRgb(63, 81, 181),      // Índigo
            OxyColor.FromRgb(0, 150, 136),      // Verde azulado
            OxyColor.FromRgb(156, 39, 176),     // Púrpura
            OxyColor.FromRgb(244, 67, 54),      // Rojo
            OxyColor.FromRgb(76, 175, 80),      // Verde
            OxyColor.FromRgb(255, 152, 0),      // Naranja
            OxyColor.FromRgb(33, 150, 243),     // Azul claro
            OxyColor.FromRgb(139, 195, 74)      // Lima
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
    /// Crea un gráfico de columnas comparando ingresos vs gastos
    /// </summary>
    public PlotModel CreateIncomeVsExpensesChart(List<Expense> expenses)
    {
        var today = DateTime.Now;
        
        // Filtrar por mes actual
        var monthExpenses = expenses
            .Where(e => e.Date.Year == today.Year && e.Date.Month == today.Month)
            .ToList();

        var totalExpenses = monthExpenses
            .Where(e => e.TransactionType == TransactionType.Gasto)
            .Sum(e => e.Amount);

        var totalIncome = monthExpenses
            .Where(e => e.TransactionType == TransactionType.Ingreso)
            .Sum(e => e.Amount);

        var model = new PlotModel
        {
            Title = $"Ingresos vs Gastos - {today:MMMM yyyy}",
            TitleFontSize = 16,
            Background = OxyColor.FromRgb(255, 255, 255),
            Padding = new OxyThickness(60, 20, 20, 60)
        };

        // Crear CategoryAxis para el eje X (abajo)
        var categoryAxis = new CategoryAxis
        {
            Position = AxisPosition.Bottom,
            Key = "Categories",
            FontSize = 12
        };
        categoryAxis.Labels.Add("Ingresos");
        categoryAxis.Labels.Add("Gastos");
        model.Axes.Add(categoryAxis);

        // Crear LinearAxis para el eje Y (izquierda) - Valores
        var valueAxis = new LinearAxis
        {
            Position = AxisPosition.Left,
            Key = "Values",
            FontSize = 12,
            Title = "Cantidad (€)",
            TitleFontSize = 12
        };
        model.Axes.Add(valueAxis);

        // Crear barras verticales usando RectangleBarSeries (compatible con OxyPlot 2.2)
        var barsSeries = new RectangleBarSeries
        {
            XAxisKey = "Categories",
            YAxisKey = "Values",
            StrokeThickness = 0
        };

        // x0, y0, x1, y1
        barsSeries.Items.Add(new RectangleBarItem(-0.3, 0, 0.3, (double)totalIncome)
        {
            Color = OxyColor.FromRgb(76, 175, 80) // Verde
        });
        
        barsSeries.Items.Add(new RectangleBarItem(0.7, 0, 1.3, (double)totalExpenses)
        {
            Color = OxyColor.FromRgb(244, 67, 54) // Rojo
        });

        model.Series.Add(barsSeries);
        return model;
    }

    /// <summary>
    /// Calcula el total de gastos por categoría (solo gastos)
    /// </summary>
    public Dictionary<string, decimal> GetExpensesByCategory(List<Expense> expenses)
    {
        return expenses
            .Where(e => e.TransactionType == TransactionType.Gasto && e.Category != "Ingresos")
            .GroupBy(e => e.Category)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount))
            .OrderByDescending(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);
    }
}
