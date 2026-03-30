using System.Globalization;
using FinanceProject.Components;
using FinanceProject.Domain.Repositories;
using FinanceProject.Infrastructure.Repositories;
using FinanceProject.Services;

var culture = new CultureInfo("en-IE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<SqliteExpenseRepository>();
builder.Services.AddSingleton<IExpenseRepository>(sp => sp.GetRequiredService<SqliteExpenseRepository>());
builder.Services.AddSingleton<ICategoryRepository>(sp => sp.GetRequiredService<SqliteExpenseRepository>());
builder.Services.AddScoped<ExpenseApplicationService>();
builder.Services.AddScoped<CategoryApplicationService>();
builder.Services.AddSingleton<CategoryMappingService>();
builder.Services.AddScoped<PresentationFormattingService>();
builder.Services.AddScoped<ExpenseDashboardService>();
builder.Services.AddSingleton<ExpenseService>();
builder.Services.AddSingleton<MonthlySummaryService>();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<CategoryManagerUiState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();