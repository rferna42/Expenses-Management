using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using OxyPlot;
using FinanceProject.Models;
using FinanceProject.Services;

namespace FinanceProject;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly ExpenseService _expenseService;
    private readonly ChartsService _chartsService;
    private readonly MonthlySummaryService _monthlySummaryService;
    private PlotModel? _chartModel;
    private PlotModel? _incomeExpenseChartModel;
    private MonthlySummary? _monthlySummary;
    
    public ObservableCollection<Expense> Expenses { get; set; } = [];
    private ObservableCollection<Expense> AllExpenses { get; set; } = [];
    private string SelectedCategory = "Todas";
    private string SortBy = "Fecha Descendente";
    private TransactionType _selectedTransactionType = TransactionType.Gasto;
    
    public PlotModel? ChartModel
    {
        get => _chartModel;
        set
        {
            if (_chartModel != value)
            {
                _chartModel = value;
                OnPropertyChanged(nameof(ChartModel));
            }
        }
    }
    
    public PlotModel? IncomeExpenseChartModel
    {
        get => _incomeExpenseChartModel;
        set
        {
            if (_incomeExpenseChartModel != value)
            {
                _incomeExpenseChartModel = value;
                OnPropertyChanged(nameof(IncomeExpenseChartModel));
            }
        }
    }
    
    public MonthlySummary? MonthlySummary
    {
        get => _monthlySummary;
        set
        {
            if (_monthlySummary != value)
            {
                _monthlySummary = value;
                OnPropertyChanged(nameof(MonthlySummary));
            }
        }
    }
    
    public TransactionType SelectedTransactionType
    {
        get => _selectedTransactionType;
        set
        {
            if (_selectedTransactionType != value)
            {
                _selectedTransactionType = value;
                OnPropertyChanged(nameof(SelectedTransactionType));
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainWindow()
    {
        _expenseService = new ExpenseService();
        _chartsService = new ChartsService();
        _monthlySummaryService = new MonthlySummaryService();
        InitializeComponent();
        
        // Establecer DataContext para bindings
        DataContext = this;
        
        ExpensesListView.ItemsSource = Expenses;
        Expenses.CollectionChanged += (s, e) => UpdateTotal();
        Expenses.CollectionChanged += (s, e) => RefreshChart();
        Expenses.CollectionChanged += (s, e) => RefreshMonthlySummary();
        
        // Cargar gastos guardados
        LoadExpenses();
        
        // Inicializar el combo de filtro con "Todas" seleccionado
        FilterComboBox.SelectedIndex = 0;
        CategoryComboBox.SelectedIndex = 0;
        SortComboBox.SelectedIndex = 0;
        TransactionTypeComboBox.SelectedIndex = 0;
        
        // Establecer la fecha por defecto a hoy
        DatePicker.SelectedDate = DateTime.Today;
        
        // Agregar manejadores de Enter a los TextBoxes
        DescriptionTextBox.KeyDown += TextBox_KeyDown;
        AmountTextBox.KeyDown += TextBox_KeyDown;
        
        // Manejador para cambios en tipo de transacción
        TransactionTypeComboBox.SelectionChanged += (s, e) =>
        {
            if (TransactionTypeComboBox.SelectedIndex == 0)
                SelectedTransactionType = TransactionType.Gasto;
            else if (TransactionTypeComboBox.SelectedIndex == 1)
                SelectedTransactionType = TransactionType.Ingreso;
        };
        
        // Inicializar gráfico y resumen
        RefreshChart();
        RefreshMonthlySummary();
    }
    
    private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Return)
        {
            AddExpense();
            e.Handled = true;
        }
    }

    private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
    {
        AddExpense();
    }

    private void AddExpense()
    {
        var amountText = AmountTextBox.Text.Replace(".", ",");
        var selectedCategory = CategoryComboBox.SelectedItem?.ToString() ?? "Otros";
        var selectedDate = DatePicker.SelectedDate ?? DateTime.Today;
        
        if (decimal.TryParse(amountText, System.Globalization.NumberStyles.Any, new CultureInfo("es-ES"), out var amount))
        {
            var expense = new Expense 
            { 
                Description = DescriptionTextBox.Text, 
                Amount = amount,
                Category = selectedCategory,
                Date = selectedDate,
                TransactionType = SelectedTransactionType
            };

            if (_expenseService.ValidateExpense(expense, out var errorMessage))
            {
                AllExpenses.Add(expense);
                DescriptionTextBox.Clear();
                AmountTextBox.Clear();
                DatePicker.SelectedDate = DateTime.Today;
                DescriptionTextBox.Focus();
                SaveExpenses();
                ApplyFilter();
            }
            else
            {
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        else
        {
            MessageBox.Show("Por favor, ingresa una descripción y un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void FilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        ApplyFilter();
    }
    
    private void SortComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        SelectedCategory = FilterComboBox.SelectedItem?.ToString() ?? "Todas";
        SortBy = SortComboBox.SelectedItem?.ToString() ?? "Fecha Descendente";
        
        var filtered = _expenseService.ApplyFilterAndSort(AllExpenses.ToList(), SelectedCategory, SortBy);
        
        Expenses.Clear();
        foreach (var expense in filtered)
        {
            Expenses.Add(expense);
        }
    }

    private void UpdateTotal()
    {
        var totalExpenses = Expenses.Where(e => e.TransactionType == TransactionType.Gasto).Sum(e => e.Amount);
        TotalTextBlock.Text = totalExpenses.ToString("N2");
    }
    
    private void RefreshChart()
    {
        try
        {
            ChartModel = _chartsService.CreateCategoryExpensesChart(AllExpenses.ToList());
            RefreshIncomeExpenseChart();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar gráfico: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void RefreshIncomeExpenseChart()
    {
        try
        {
            IncomeExpenseChartModel = _chartsService.CreateIncomeVsExpensesChart(AllExpenses.ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar gráfico de ingresos/gastos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void RefreshMonthlySummary()
    {
        try
        {
            MonthlySummary = _monthlySummaryService.GetCurrentMonthSummary(AllExpenses.ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al actualizar resumen: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void SaveExpenses()
    {
        _expenseService.SaveExpenses(AllExpenses.ToList());
    }
    
    private void LoadExpenses()
    {
        var expenses = _expenseService.LoadExpenses();
        foreach (var expense in expenses)
        {
            AllExpenses.Add(expense);
        }
        ApplyFilter();
    }
    
    public void DeleteExpense(Expense expense)
    {
        AllExpenses.Remove(expense);
        SaveExpenses();
        ApplyFilter();
    }
    
    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button button && button.DataContext is Expense expense)
        {
            var result = MessageBox.Show(
                $"¿Estás seguro de que deseas eliminar este gasto? \n{expense.Description} - {expense.Amount.ToString("C2", new CultureInfo("es-ES"))}", 
                "Confirmar eliminación", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                DeleteExpense(expense);
            }
        }
    }
    
    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button button && button.DataContext is Expense expense)
        {
            // Guardar valores originales por si cancela
            if (!expense.IsEditing)
            {
                expense.OriginalDescription = expense.Description;
                expense.OriginalDate = expense.Date;
                expense.OriginalCategory = expense.Category;
                expense.OriginalAmount = expense.Amount;
            }
            expense.IsEditing = !expense.IsEditing;
        }
    }
    
    public void SaveExpenseChanges(Expense expense)
    {
        if (string.IsNullOrWhiteSpace(expense.Description) || expense.Amount <= 0)
        {
            MessageBox.Show("Por favor, completa todos los campos correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        expense.IsEditing = false;
        SaveExpenses();
        ApplyFilter();
    }
    
    public void CancelExpenseChanges(Expense expense)
    {
        expense.Description = expense.OriginalDescription;
        expense.Date = expense.OriginalDate;
        expense.Category = expense.OriginalCategory;
        expense.Amount = expense.OriginalAmount;
        expense.IsEditing = false;
    }
    
    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button button && button.DataContext is Expense expense)
        {
            SaveExpenseChanges(expense);
        }
    }
    
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is System.Windows.Controls.Button button && button.DataContext is Expense expense)
        {
            CancelExpenseChanges(expense);
        }
    }
}
