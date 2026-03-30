namespace FinanceProject.Services;

public class CategoryMappingService
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["All"] = "All",
        ["Ingresos"] = "Ingresos",
        ["Cuotas"] = "Cuotas",
        ["Suscripciones"] = "Suscripciones",
        ["Salud"] = "Salud",
        ["Hogar"] = "Hogar",
        ["Alimentación"] = "Alimentación",
        ["Vehículos"] = "Vehículos",
        ["Viajes"] = "Viajes",
        ["Ocio"] = "Ocio",
        ["Restaurantes"] = "Restaurantes",
        ["Compras"] = "Compras",

        // Legacy aliases
        ["Income"] = "Ingresos",
        ["Loans"] = "Cuotas",
        ["Bills"] = "Cuotas",
        ["Utilities"] = "Hogar",
        ["Rent"] = "Hogar",
        ["Food"] = "Alimentación",
        ["Fuel"] = "Vehículos",
        ["Travels"] = "Viajes",
        ["Entertainment"] = "Ocio",
        ["Dining"] = "Restaurantes",
        ["Shopping"] = "Compras",
        ["Health"] = "Salud",
        ["Other"] = "Compras"
    };

    private static readonly Dictionary<string, string> LocalizationKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["All"] = "category_all",
        ["Ingresos"] = "category_income",
        ["Cuotas"] = "category_installments",
        ["Suscripciones"] = "category_subscriptions",
        ["Salud"] = "category_health",
        ["Hogar"] = "category_home",
        ["Alimentación"] = "category_food",
        ["Vehículos"] = "category_vehicles",
        ["Viajes"] = "category_travels",
        ["Ocio"] = "category_leisure",
        ["Restaurantes"] = "category_restaurants",
        ["Compras"] = "category_shopping"
    };

    public string NormalizeCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return category;
        }

        return Aliases.TryGetValue(category.Trim(), out var normalized)
            ? normalized
            : category;
    }

    public bool IsIncomeCategory(string category)
    {
        return string.Equals(NormalizeCategory(category), "Ingresos", StringComparison.OrdinalIgnoreCase);
    }

    public string? ResolveLocalizationKey(string category)
    {
        var normalized = NormalizeCategory(category);
        return LocalizationKeys.TryGetValue(normalized, out var key)
            ? key
            : null;
    }
}
