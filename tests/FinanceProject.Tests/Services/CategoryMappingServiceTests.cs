using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class CategoryMappingServiceTests
{
    private readonly CategoryMappingService _service = new();

    [Theory]
    [InlineData("Income", "Ingresos")]
    [InlineData("Bills", "Cuotas")]
    [InlineData("Utilities", "Hogar")]
    [InlineData("Fuel", "Vehículos")]
    [InlineData("Shopping", "Compras")]
    [InlineData("Compras", "Compras")]
    public void NormalizeCategory_ReturnsExpectedCanonicalValue(string input, string expected)
    {
        var normalized = _service.NormalizeCategory(input);

        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData("Income", true)]
    [InlineData("Ingresos", true)]
    [InlineData("Compras", false)]
    public void IsIncomeCategory_DetectsIncomeAliases(string input, bool expected)
    {
        var isIncome = _service.IsIncomeCategory(input);

        Assert.Equal(expected, isIncome);
    }

    [Fact]
    public void ResolveLocalizationKey_ReturnsNull_ForUnknownCategory()
    {
        var key = _service.ResolveLocalizationKey("CategoriaInventada");

        Assert.Null(key);
    }
}
