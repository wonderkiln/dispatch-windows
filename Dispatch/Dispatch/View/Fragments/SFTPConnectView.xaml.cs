﻿using Dispatch.Service.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class SFTPConnectView : UserControl
    {
        public string Address { get; set; } = "127.0.0.1";
        public int? Port { get; set; } = 22;
        public string User { get; set; } = "Adrian";
        public string Password { get; set; } = "root";
        public string Root { get; set; } = "/Downloads";

        public static readonly DependencyProperty ConnectViewProperty = DependencyProperty.Register("ConnectView", typeof(IConnectView), typeof(SFTPConnectView));

        public IConnectView ConnectView
        {
            get { return (IConnectView)GetValue(ConnectViewProperty); }
            set { SetValue(ConnectViewProperty, value); }
        }

        public SFTPConnectView()
        {
            InitializeComponent();
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            AddressTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            PortTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            UserTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            PasswordTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            RootTextBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (AddressTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                PortTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                UserTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                PasswordTextBox.GetBindingExpression(TextBox.TextProperty).HasError ||
                RootTextBox.GetBindingExpression(TextBox.TextProperty).HasError)
            {
                return;
            }

            ConnectView.OnBeginConnecting();

            try
            {
                var client = await SFTPClient.Create(Address, Port.Value, User, Password);

                var args = new ConnectViewArgs() { Client = client, InitialPath = Root ?? "/", Name = $"{Address}:{Port}" };
                ConnectView.OnSuccess(args);
            }
            catch (Exception ex)
            {
                ConnectView.OnException(ex);
            }
        }
    }
}
