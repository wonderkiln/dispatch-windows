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

            var provider = new GithubUpdateProvider("https://api.github.com/repos/wonderkiln/dispatch-windows/releases/latest", "50a6cb3f19ef948b8707255e35a09bb0675f1791");
            new ApplicationUpdater(provider).CheckForUpdate();

            Version.Text = ApplicationUpdater.CurrentVersion.ToString();
        }
    }
}
