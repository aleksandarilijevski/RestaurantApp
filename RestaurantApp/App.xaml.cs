using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using RestaurantApp.Services;
using RestaurantApp.Services.Interface;
using RestaurantApp.ViewModels;
using System.Windows;

namespace RestaurantApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            MainWindow window = Container.Resolve<MainWindow>();
            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IDatabaseService, DatabaseService>();
            ViewModelLocationProvider.Register<MainWindow, MainWindowViewModel>();
        }
    }
}
