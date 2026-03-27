using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using FinanceProject.Configuration;
using FinanceProject.Domain.Repositories;
using FinanceProject.Infrastructure.Repositories;
using OxyPlot;
using FinanceProject.Models;
using FinanceProject.Services;

namespace FinanceProject;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly ExpenseService _expenseService;
    private readonly IExpenseRepository _expenseRepository;
    private readonly ChartsService _chartsService;
    private readonly MonthlySummaryService _monthlySummaryService;
    private PlotModel? _chartModel;
    private PlotModel? _incomeExpenseChartModel;
    private MonthlySummary? _monthlySummary;
    
    public ObservableCollection<Expense> Expenses { get; set; } = [];
    private ObservableCollection<Expense> AllExpenses { get; set; } = [];
    private string SelectedCategory = AppConfiguration.AllCategoriesLabel;
    private ExpenseSortOption SortBy = AppConfiguration.DefaultSortOption;
    private TransactionType _selectedTransactionType = TransactionType.Expense;

    public IReadOnlyList<string> Categories => AppConfiguration.Categories;
    public IReadOnlyList<string> FilterCategories => AppConfiguration.FilterCategories;
    public IReadOnlyList<SortOptionItem> SortOptions => AppConfiguration.SortOptions;
    public IReadOnlyList<TransactionType> TransactionTypes => AppConfiguration.TransactionTypes;
    
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
        _expenseRepository = new SqliteExpenseRepository();
        _chartsService = new ChartsService();
        _monthlySummaryService = new MonthlySummaryService();
        InitializeComponent();
        
        // Set DataContext for UI bindings.
        DataContext = this;
        
        ExpensesListView.ItemsSource = Expenses;
        Expenses.CollectionChanged += (s, e) => UpdateTotal();
        Expenses.CollectionChanged += (s, e) => RefreshChart();
        Expenses.CollectionChanged += (s, e) => RefreshMonthlySummary();
        
        // Load persisted transactions.
        LoadExpenses();
        
        // Initialize combo boxes with default selections.
        FilterComboBox.SelectedIndex = 0;
        CategoryComboBox.SelectedIndex = 0;
        SortComboBox.SelectedIndex = 0;
        TransactionTypeComboBox.SelectedIndex = 0;
        
        // Set default date.
        DatePicker.SelectedDate = DateTime.Today;
        
        // Enable Enter key submission.
        DescriptionTextBox.KeyDown += TextBox_KeyDown;
        AmountTextBox.KeyDown += TextBox_KeyDown;
        
        // Keep selected transaction type in sync with the combo box.
        TransactionTypeComboBox.SelectionChanged += (s, e) =>
        {
            if (TransactionTypeComboBox.SelectedItem is TransactionType selectedType)
            {
                SelectedTransactionType = selectedType;
            }
        };
        
        // Initialize chart and summary.
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
        var amountText = AmountTextBox.Text.Trim();
        var selectedCategory = CategoryComboBox.SelectedItem?.ToString() ?? AppConfiguration.DefaultCategory;
        var selectedDate = DatePicker.SelectedDate ?? DateTime.Today;
        
        if (AppConfiguration.TryParseAmount(amountText, out var amount))
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
                MessageBox.Show(errorMessage, AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        else
        {
            MessageBox.Show(AppConfiguration.InvalidExpenseMessage, AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
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
        SelectedCategory = FilterComboBox.SelectedItem?.ToString() ?? AppConfiguration.AllCategoriesLabel;
        SortBy = SortComboBox.SelectedValue is ExpenseSortOption selectedSort
            ? selectedSort
            : AppConfiguration.DefaultSortOption;
        
        var filtered = _expenseService.ApplyFilterAndSort(AllExpenses.ToList(), SelectedCategory, SortBy);
        
        Expenses.Clear();
        foreach (var expense in filtered)
        {
            Expenses.Add(expense);
        }
    }

    private void UpdateTotal()
    {
        var totalExpenses = Expenses.Where(e => e.TransactionType == TransactionType.Expense).Sum(e => e.Amount);
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
            MessageBox.Show($"{AppConfiguration.ChartErrorPrefix} {ex.Message}", AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show($"{AppConfiguration.IncomeExpenseChartErrorPrefix} {ex.Message}", AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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
            MessageBox.Show($"{AppConfiguration.SummaryErrorPrefix} {ex.Message}", AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void SaveExpenses()
    {
        try
        {
            _expenseRepository.Save(AllExpenses.ToList());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{AppConfiguration.SaveErrorPrefix} {ex.Message}", AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void LoadExpenses()
    {
        try
        {
            var expenses = _expenseRepository.Load();
            foreach (var expense in expenses)
            {
                AllExpenses.Add(expense);
            }
            ApplyFilter();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{AppConfiguration.LoadErrorPrefix} {ex.Message}", AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
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
                AppConfiguration.BuildDeleteConfirmationText(expense.Description, expense.Amount),
                AppConfiguration.DeleteDialogTitle,
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
            // Keep original values so changes can be reverted.
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
            MessageBox.Show(AppConfiguration.InvalidEditMessage, AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
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
