using ByteSizeLib;
using Dispatch.Updater;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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

namespace Dispatch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Load();
        }

        private async void Load()
        {
            Version.Text = ApplicationUpdater.CurrentVersion.ToString(2);

            var provider = new GithubUpdateProvider("https://api.github.com/repos/wonderkiln/dispatch-windows/releases/latest", "a2aeafe429f92d49fda639405deed94957b73aec");
            var updater = new ApplicationUpdater(provider);

            await updater.CheckForUpdate();
            UpdateVersion.Text = updater.LatestUpdate.Version.ToString(2);
        }
    }
}
