using FinanceProject.Configuration;

namespace FinanceProject.Models;

public enum TransactionType
{
    Expense,
    Income
}

public class Expense
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = AppConfiguration.DefaultCategory;
    public DateTime Date { get; set; } = DateTime.Now;
    public TransactionType TransactionType { get; set; } = TransactionType.Expense;
}
