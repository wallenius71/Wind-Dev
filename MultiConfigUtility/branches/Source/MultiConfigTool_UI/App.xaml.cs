using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace MultiConfigTool_UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow  main = new MainWindow ();
            var viewmodel = new MainWindowViewModel();
            main.DataContext = viewmodel;
            main.WindowState = WindowState.Maximized;
            main.Show();
        }
    }
}
