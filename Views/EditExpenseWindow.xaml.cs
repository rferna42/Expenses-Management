using System.Windows;
using FinanceProject.Configuration;
using FinanceProject.Models;

namespace FinanceProject;

public partial class EditExpenseWindow : Window
{
    private Expense _expense;

    public EditExpenseWindow(Expense expense)
    {
        InitializeComponent();
        _expense = expense;

        CategoryComboBox.ItemsSource = AppConfiguration.Categories;
        
        // Load current transaction values.
        DescriptionTextBox.Text = expense.Description;
        DatePicker.SelectedDate = expense.Date;
        CategoryComboBox.SelectedItem = expense.Category;
        AmountTextBox.Text = expense.Amount.ToString("F2");
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var amountText = AmountTextBox.Text.Trim();
        
        if (AppConfiguration.TryParseAmount(amountText, out var amount) && 
            !string.IsNullOrWhiteSpace(DescriptionTextBox.Text) &&
            DatePicker.SelectedDate.HasValue)
        {
            _expense.Description = DescriptionTextBox.Text;
            _expense.Date = DatePicker.SelectedDate.Value;
            _expense.Category = CategoryComboBox.SelectedItem?.ToString() ?? AppConfiguration.DefaultCategory;
            _expense.Amount = amount;
            
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show(AppConfiguration.InvalidEditMessage, AppConfiguration.ErrorDialogTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
