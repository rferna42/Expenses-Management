using System.ComponentModel;

namespace FinanceProject.Models;

public enum TransactionType
{
    Gasto,
    Ingreso
}

public class Expense : INotifyPropertyChanged
{
    private string _description = string.Empty;
    private decimal _amount;
    private string _category = "Otros";
    private DateTime _date = DateTime.Now;
    private bool _isEditing;
    private TransactionType _transactionType = TransactionType.Gasto;

    public string Description
    {
        get => _description;
        set
        {
            if (_description != value)
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }

    public decimal Amount
    {
        get => _amount;
        set
        {
            if (_amount != value)
            {
                _amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
    }

    public string Category
    {
        get => _category;
        set
        {
            if (_category != value)
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }
    }

    public DateTime Date
    {
        get => _date;
        set
        {
            if (_date != value)
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }
    }

    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (_isEditing != value)
            {
                _isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
            }
        }
    }

    public TransactionType TransactionType
    {
        get => _transactionType;
        set
        {
            if (_transactionType != value)
            {
                _transactionType = value;
                OnPropertyChanged(nameof(TransactionType));
            }
        }
    }

    // Valores originales para poder cancelar
    public string OriginalDescription { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public string OriginalCategory { get; set; } = "Otros";
    public DateTime OriginalDate { get; set; } = DateTime.Now;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
