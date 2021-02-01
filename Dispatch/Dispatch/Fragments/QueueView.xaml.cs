using Dispatch.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Dispatch.View.Fragments
{
    public class IsIndeterminateProgressValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value < 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QueueItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PendingDataTemplate { get; set; }
        public DataTemplate WorkingDataTemplate { get; set; }
        public DataTemplate DoneDataTemplate { get; set; }
        public DataTemplate ErrorDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var queueItem = (QueueViewModel.QueueItem)item;

            switch (queueItem.Status)
            {
                case QueueViewModel.QueueItem.StatusType.Pending:
                    return PendingDataTemplate;

                case QueueViewModel.QueueItem.StatusType.Working:
                    return WorkingDataTemplate;

                case QueueViewModel.QueueItem.StatusType.Done:
                    return DoneDataTemplate;

                case QueueViewModel.QueueItem.StatusType.Error:
                    return ErrorDataTemplate;

                default:
                    return null;
            }
        }
    }

    public partial class QueueView : UserControl
    {
        public QueueView()
        {
            InitializeComponent();
        }
    }
}
