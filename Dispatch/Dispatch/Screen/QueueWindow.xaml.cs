﻿using Dispatch.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dispatch.Screen
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Queued { get; set; }
        public DataTemplate IntermitentProgress { get; set; }
        public DataTemplate InProgress { get; set; }
        public DataTemplate Finished { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var status = (TransferObject.TransferStatus)item;

            switch(status)
            {
                case TransferObject.TransferStatus.Queued:
                    return Queued;
                case TransferObject.TransferStatus.InProgress:
                    return IntermitentProgress;
                default:
                    return Finished;
            }
        }
    }

    /// <summary>
    /// Interaction logic for QueueWindow.xaml
    /// </summary>
    public partial class QueueWindow : Window
    {
        public QueueViewModel QueueViewModel { get; set; }

        public QueueWindow()
        {
            InitializeComponent();
        }
    }
}
