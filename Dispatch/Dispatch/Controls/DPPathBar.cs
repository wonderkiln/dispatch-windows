using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Dispatch.Controls
{
    public class DPPathBarItem : Control
    {
        public static readonly DependencyProperty IsLastProperty = DependencyProperty.Register("IsLast", typeof(bool), typeof(DPPathBarItem));
        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }

        public static new readonly DependencyProperty NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(DPPathBarItem));
        public new string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(DPPathBarItem));
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public DPPathBarItem()
        {
            DefaultStyleKey = typeof(DPPathBarItem);
        }

        public Action<DPPathBarItem> OnClick;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var PART_Button = GetTemplateChild("PART_Button") as Button;
            PART_Button.Click += PART_Button_Click;
        }

        private void PART_Button_Click(object sender, RoutedEventArgs e)
        {
            OnClick?.Invoke(this);
        }
    }

    public class DPPathBar : ItemsControl
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(DPPathBar), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPathChanged));
        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        protected static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pathBar = (DPPathBar)d;

            var path = (string)e.NewValue;
            var delimiters = new char[] { '\\', '/' };

            foreach (var delimiter in delimiters)
            {
                if (path.Contains(delimiter.ToString()))
                {
                    var items = new List<DPPathBarItem>();
                    var components = path.Split(delimiter).Where(c => !string.IsNullOrEmpty(c)).ToList();

                    for (int i = 0; i < components.Count; i++)
                    {
                        items.Add(new DPPathBarItem()
                        {
                            Name = components[i],
                            Path = string.Join(delimiter.ToString(), components.GetRange(0, i + 1)) + delimiter,
                            IsLast = i == components.Count - 1,
                        });
                    }

                    pathBar.ItemsSource = items;

                    break;
                }
            }
        }

        private void CalculateScrollViewerOffset()
        {
            PART_ScrollViewer.ScrollToHorizontalOffset(PART_ScrollViewer.ExtentWidth - PART_ScrollViewer.ViewportWidth);
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            CalculateScrollViewerOffset();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CalculateScrollViewerOffset();
        }

        public DPPathBar()
        {
            DefaultStyleKey = typeof(DPPathBar);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DPPathBarItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DPPathBarItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            var pathBarItem = (DPPathBarItem)element;
            pathBarItem.OnClick = (e) =>
            {
                SetValue(PathProperty, e.Path);
            };
        }

        private ScrollViewer PART_ScrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ScrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;
        }
    }
}
