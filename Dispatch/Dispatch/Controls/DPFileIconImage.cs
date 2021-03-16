using Dispatch.Service.Theme;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Controls
{
    public class DPFileIconImage : Image
    {
        public static readonly DependencyProperty FullNameProperty = DependencyProperty.Register("FullName", typeof(string), typeof(DPFileIconImage), new PropertyMetadata(OnPropertyChanged));
        public string FullName
        {
            get { return (string)GetValue(FullNameProperty); }
            set { SetValue(FullNameProperty, value); }
        }

        public static readonly DependencyProperty IsFileProperty = DependencyProperty.Register("IsFile", typeof(bool), typeof(DPFileIconImage), new PropertyMetadata(true, OnPropertyChanged));
        public bool IsFile
        {
            get { return (bool)GetValue(IsFileProperty); }
            set { SetValue(IsFileProperty, value); }
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var image = (DPFileIconImage)d;
            image.Invalidate();
        }

        public DPFileIconImage()
        {
            FileIconTheme.OnInvalidate += Theme_OnInvalidate;
        }

        private void Theme_OnInvalidate(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void Invalidate()
        {
            if (IsFile)
            {
                Source = FileIconTheme.GetFileImageSource(FullName);
            }
            else
            {
                Source = FileIconTheme.GetFolderImageSource();
            }
        }
    }
}
