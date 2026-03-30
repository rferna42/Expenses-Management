using FinanceProject.Services;

namespace FinanceProject.Tests.Services;

public class CategoryManagerUiStateTests
{
    [Fact]
    public void Toggle_ChangesOpenState_AndRaisesEvent()
    {
        var state = new CategoryManagerUiState();
        var events = 0;
        state.OnChange += () => events++;

        state.Toggle();
        state.Toggle();

        Assert.False(state.IsOpen);
        Assert.Equal(2, events);
    }

    [Fact]
    public void OpenAndClose_AreIdempotent()
    {
        var state = new CategoryManagerUiState();
        var events = 0;
        state.OnChange += () => events++;

        state.Open();
        state.Open();
        state.Close();
        state.Close();

        Assert.False(state.IsOpen);
        Assert.Equal(2, events);
    }
}