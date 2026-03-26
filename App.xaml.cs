using System.Globalization;
using System.Windows;

namespace FinanceProject;

public partial class App : Application
{
    public App()
    {
        // Configurar la cultura a español para usar euros
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-ES");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("es-ES");
    }
}