using Microsoft.JSInterop;
using FinanceProject.Configuration;

namespace FinanceProject.Services;

public class LocalizationService
{
    private readonly IJSRuntime _jsRuntime;
    private Language _currentLanguage = Language.Spanish;
    private const string StorageKey = "app-language";

    public event Action? OnLanguageChanged;

    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnLanguageChanged?.Invoke();
            }
        }
    }

    public LocalizationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var savedLanguage = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrEmpty(savedLanguage) && Enum.TryParse<Language>(savedLanguage, out var lang))
            {
                _currentLanguage = lang;
            }
        }
        catch
        {
            // If localStorage fails, default to Spanish
            _currentLanguage = Language.Spanish;
        }
    }

    public async Task SetLanguageAsync(Language language)
    {
        CurrentLanguage = language;
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, language.ToString());
        }
        catch
        {
            // Silently fail if localStorage is not available
        }
    }

    public string T(string key) => LocalizationStrings.Get(key, _currentLanguage);
}
