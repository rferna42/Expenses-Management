namespace FinanceProject.Services;

public class CategoryManagerUiState
{
    public bool IsOpen { get; private set; }

    public event Action? OnChange;

    public void Toggle()
    {
        IsOpen = !IsOpen;
        OnChange?.Invoke();
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;
        OnChange?.Invoke();
    }

    public void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        IsOpen = false;
        OnChange?.Invoke();
    }
}
