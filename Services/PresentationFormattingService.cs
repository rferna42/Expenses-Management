using System.Globalization;
using FinanceProject.Configuration;
using FinanceProject.Models;

namespace FinanceProject.Services;

public class PresentationFormattingService
{
    private readonly LocalizationService _localizationService;
    private readonly CategoryMappingService _categoryMappingService;

    public PresentationFormattingService(
        LocalizationService localizationService,
        CategoryMappingService categoryMappingService)
    {
        _localizationService = localizationService;
        _categoryMappingService = categoryMappingService;
    }

    public string GetSortOptionLabel(ExpenseSortOption option)
    {
        return option switch
        {
            ExpenseSortOption.DateDescending => _localizationService.T("date_descending"),
            ExpenseSortOption.DateAscending => _localizationService.T("date_ascending"),
            ExpenseSortOption.AmountDescending => _localizationService.T("amount_descending"),
            ExpenseSortOption.AmountAscending => _localizationService.T("amount_ascending"),
            ExpenseSortOption.CategoryAscending => _localizationService.T("category_ascending"),
            _ => option.ToString()
        };
    }

    public string GetTransactionTypeLabel(TransactionType transactionType)
    {
        return transactionType switch
        {
            TransactionType.Expense => _localizationService.T("transaction_expense"),
            TransactionType.Income => _localizationService.T("transaction_income"),
            _ => transactionType.ToString()
        };
    }

    public string GetCategoryLabel(string category)
    {
        var localizationKey = _categoryMappingService.ResolveLocalizationKey(category);
        return localizationKey is null
            ? category
            : _localizationService.T(localizationKey);
    }

    public string GetMonthLabel(int year, int month)
    {
        var culture = GetDisplayCulture();
        var monthLabel = new DateTime(year, month, 1).ToString("MMMM yyyy", culture);

        return _localizationService.CurrentLanguage == Language.Spanish
            ? char.ToUpper(monthLabel[0], culture) + monthLabel[1..]
            : monthLabel;
    }

    private CultureInfo GetDisplayCulture()
    {
        return _localizationService.CurrentLanguage == Language.Spanish
            ? new CultureInfo("es-ES")
            : new CultureInfo("en-US");
    }
}
