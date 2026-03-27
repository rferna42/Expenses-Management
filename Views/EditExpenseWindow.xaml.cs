using System.Globalization;
using System.Windows;
using FinanceProject.Models;

namespace FinanceProject;

public partial class EditExpenseWindow : Window
{
    private Expense _expense;

    public EditExpenseWindow(Expense expense)
    {
        InitializeComponent();
        _expense = expense;
        
        // Cargar datos del gasto
        DescriptionTextBox.Text = expense.Description;
        DatePicker.SelectedDate = expense.Date;
        CategoryComboBox.SelectedItem = expense.Category;
        AmountTextBox.Text = expense.Amount.ToString("F2", new CultureInfo("es-ES"));
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var amountText = AmountTextBox.Text.Replace(".", ",");
        
        if (decimal.TryParse(amountText, System.Globalization.NumberStyles.Any, new CultureInfo("es-ES"), out var amount) && 
            !string.IsNullOrWhiteSpace(DescriptionTextBox.Text) &&
            DatePicker.SelectedDate.HasValue)
        {
            _expense.Description = DescriptionTextBox.Text;
            _expense.Date = DatePicker.SelectedDate.Value;
            _expense.Category = CategoryComboBox.SelectedItem?.ToString() ?? "Otros";
            _expense.Amount = amount;
            
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Por favor, rellena todos los campos correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
