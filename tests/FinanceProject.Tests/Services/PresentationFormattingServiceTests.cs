using System.Globalization;
using FinanceProject.Configuration;
using FinanceProject.Models;
using FinanceProject.Services;
using Microsoft.JSInterop;

namespace FinanceProject.Tests.Services;

public class PresentationFormattingServiceTests
{
    [Fact]
    public void GetSortOptionLabel_ReturnsLocalizedSpanishLabel()
    {
        var localizationService = CreateLocalizationService(Language.Spanish);
        var service = new PresentationFormattingService(localizationService, new CategoryMappingService());

        var label = service.GetSortOptionLabel(ExpenseSortOption.DateAscending);

        Assert.Equal("Fecha Ascendente", label);
    }

    [Fact]
    public void GetTransactionTypeLabel_ReturnsLocalizedEnglishLabel()
    {
        var localizationService = CreateLocalizationService(Language.English);
        var service = new PresentationFormattingService(localizationService, new CategoryMappingService());

        var label = service.GetTransactionTypeLabel(TransactionType.Expense);

        Assert.Equal("Expense", label);
    }

    [Fact]
    public void GetCategoryLabel_UsesLegacyCategoryMappingBeforeLocalization()
    {
        var localizationService = CreateLocalizationService(Language.Spanish);
        var service = new PresentationFormattingService(localizationService, new CategoryMappingService());

        var label = service.GetCategoryLabel("Fuel");

        Assert.Equal("Vehículos", label);
    }

    [Fact]
    public void GetMonthLabel_ReturnsCapitalizedSpanishMonth()
    {
        var localizationService = CreateLocalizationService(Language.Spanish);
        var service = new PresentationFormattingService(localizationService, new CategoryMappingService());

        var label = service.GetMonthLabel(2026, 3);

        Assert.Equal("Marzo 2026", label);
    }

    [Fact]
    public void GetMonthLabel_ReturnsEnglishMonthName()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUICulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");

            var localizationService = CreateLocalizationService(Language.English);
            var service = new PresentationFormattingService(localizationService, new CategoryMappingService());

            var label = service.GetMonthLabel(2026, 3);

            Assert.Equal("March 2026", label);
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUICulture;
        }
    }

    private static LocalizationService CreateLocalizationService(Language language)
    {
        var service = new LocalizationService(new FakeJsRuntime())
        {
            CurrentLanguage = language
        };

        return service;
    }

    private sealed class FakeJsRuntime : IJSRuntime
    {
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            return ValueTask.FromResult(default(TValue)!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            return ValueTask.FromResult(default(TValue)!);
        }
    }
}
