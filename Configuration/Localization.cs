namespace FinanceProject.Configuration;

public enum Language
{
    Spanish,
    English
}

public static class LocalizationStrings
{
    private static readonly Dictionary<Language, Dictionary<string, string>> Translations = new()
    {
        {
            Language.Spanish,
            new Dictionary<string, string>
            {
                // Headers
                { "expense_distribution", "Desglose de gastos" },
                { "income_vs_expenses", "Ingresos vs Gastos" },
                
                // Labels
                { "month", "Mes" },
                { "filter_by_category", "Filtrar por categoría" },
                { "sort_by", "Ordenar por" },
                { "sort", "Ordenar" },
                { "date_descending", "Fecha Descendente" },
                { "date_ascending", "Fecha Ascendente" },
                { "amount_descending", "Cantidad Descendente" },
                { "amount_ascending", "Cantidad Ascendente" },
                { "category_ascending", "Categoría A-Z" },
                { "type", "Tipo" },
                { "amount", "Importe" },
                
                // Form fields
                { "date", "Fecha" },
                { "description", "Descripción" },
                { "category", "Categoría" },
                { "amount_eur", "Cantidad (EUR)" },
                { "transaction_type", "Tipo de Transacción" },
                { "transaction_expense", "Gasto" },
                { "transaction_income", "Ingreso" },

                // Categories
                { "category_all", "Todas" },
                { "category_income", "Ingresos" },
                { "category_installments", "Cuotas" },
                { "category_subscriptions", "Suscripciones" },
                { "category_health", "Salud" },
                { "category_home", "Hogar" },
                { "category_food", "Alimentación" },
                { "category_vehicles", "Vehículos" },
                { "category_travels", "Viajes" },
                { "category_leisure", "Ocio" },
                { "category_restaurants", "Restaurantes" },
                { "category_shopping", "Compras" },
                
                // Summary
                { "income", "Ingresos" },
                { "expense", "Gastos" },
                { "balance", "Balance" },
                { "expenses_total", "Total Gastos" },
                
                // Actions
                { "add", "Agregar" },
                { "edit", "Editar" },
                { "delete", "Eliminar" },
                { "save", "Guardar" },
                { "cancel", "Cancelar" },
                { "close", "Cerrar" },
                
                // Messages
                { "no_expense_data", "No hay datos de gastos para este mes." },
                { "no_transactions", "No se encontraron movimientos para el mes seleccionado." },
                { "category_error_empty", "El nombre de la categoría no puede estar vacío." },
                { "category_error_duplicate", "La categoría ya existe." },
                { "category_error_not_found", "La categoría no existe." },
                { "category_error_last", "Debe existir al menos una categoría." },
                { "category_error_generic", "No se pudo completar la operación de categorías." },
                { "delete_confirmation", "¿Está seguro de que desea eliminar este gasto?" },
                { "error_loading", "Error al cargar los gastos" },
                { "error_saving", "Error al guardar el gasto" },
                
                // Language selector
                { "language", "Idioma" },

                // Category manager
                { "manage_categories", "Gestionar categorías" },
                { "hide_categories", "Ocultar categorías" },
                { "category_name_placeholder", "Nombre de categoría" },
                { "add_category", "Añadir categoría" },
            }
        },
        {
            Language.English,
            new Dictionary<string, string>
            {
                // Headers
                { "expense_distribution", "Expense Distribution" },
                { "income_vs_expenses", "Income vs Expenses" },
                
                // Labels
                { "month", "Month" },
                { "filter_by_category", "Filter by category" },
                { "sort_by", "Sort by" },
                { "sort", "Sort" },
                { "date_descending", "Date Descending" },
                { "date_ascending", "Date Ascending" },
                { "amount_descending", "Amount Descending" },
                { "amount_ascending", "Amount Ascending" },
                { "category_ascending", "Category A-Z" },
                { "type", "Type" },
                { "amount", "Amount" },
                
                // Form fields
                { "date", "Date" },
                { "category", "Category" },
                { "amount_eur", "Amount (EUR)" },
                { "description", "Description" },
                { "transaction_type", "Transaction Type" },
                { "transaction_expense", "Expense" },
                { "transaction_income", "Income" },

                // Categories
                { "category_all", "All" },
                { "category_income", "Income" },
                { "category_installments", "Installments" },
                { "category_subscriptions", "Subscriptions" },
                { "category_health", "Health" },
                { "category_home", "Home" },
                { "category_food", "Food" },
                { "category_vehicles", "Vehicles" },
                { "category_travels", "Travel" },
                { "category_leisure", "Leisure" },
                { "category_restaurants", "Restaurants" },
                { "category_shopping", "Shopping" },
                
                // Summary
                { "income", "Income" },
                { "expense", "Expenses" },
                { "balance", "Balance" },
                { "expenses_total", "Total Expenses" },
                
                // Actions
                { "add", "Add" },
                { "edit", "Edit" },
                { "delete", "Delete" },
                { "save", "Save" },
                { "cancel", "Cancel" },
                { "close", "Close" },
                
                // Messages
                { "no_expense_data", "No expense data for this month." },
                { "no_transactions", "No transactions found for the selected month." },
                { "category_error_empty", "Category name cannot be empty." },
                { "category_error_duplicate", "Category already exists." },
                { "category_error_not_found", "Category was not found." },
                { "category_error_last", "At least one category must remain." },
                { "category_error_generic", "Category operation failed." },
                { "delete_confirmation", "Are you sure you want to delete this expense?" },
                { "error_loading", "Error loading expenses" },
                { "error_saving", "Error saving expense" },
                
                // Language selector
                { "language", "Language" },

                // Category manager
                { "manage_categories", "Manage categories" },
                { "hide_categories", "Hide categories" },
                { "category_name_placeholder", "Category name" },
                { "add_category", "Add category" },
            }
        }
    };

    public static string Get(string key, Language language = Language.English)
    {
        if (Translations.TryGetValue(language, out var dict) && dict.TryGetValue(key, out var value))
        {
            return value;
        }

        return key; // Return the key if translation is not found
    }
}
