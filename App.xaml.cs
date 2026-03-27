using System.Globalization;
using System.Windows;

namespace FinanceProject;

public partial class App : Application
{
    public App()
    {
        // Use English culture defaults across UI formatting.
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-IE");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-IE");
    }
}