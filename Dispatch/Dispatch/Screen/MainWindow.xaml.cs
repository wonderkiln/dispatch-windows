using ByteSizeLib;
using Dispatch.Updater;
using Dispatch.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Dispatch.Screen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<TabViewModel> Tabs { get; set; } = new ObservableCollection<TabViewModel>() { new TabViewModel() };

        private ApplicationUpdater updater = new ApplicationUpdater(new GithubUpdateProvider("https://api.github.com/repos/wonderkiln/dispatch-windows/releases/latest", "a2aeafe429f92d49fda639405deed94957b73aec"));

        public MainWindow()
        {
            InitializeComponent();
            _ = updater.CheckForUpdate();
        }

        private void TabView_OnConnected(object sender, TabViewModel e)
        {
            Tabs.Add(new TabViewModel());
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _ = updater.CheckForUpdate(false);
        }
    }
}
