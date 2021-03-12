using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Dispatch.Controls
{
    public class DPPasswordBox : Control
    {
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(DPPasswordBox));
        public string Placeholder
        {
            get { return (string)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        private static readonly DependencyPropertyKey IsPlaceholderShownPropertyKey = DependencyProperty.RegisterReadOnly("IsPlaceholderShown", typeof(bool), typeof(DPPasswordBox), new PropertyMetadata(true));
        public static readonly DependencyProperty IsPlaceholderShownProperty = IsPlaceholderShownPropertyKey.DependencyProperty;
        public bool IsPlaceholderShown
        {
            get { return (bool)GetValue(IsPlaceholderShownProperty); }
        }

        public static readonly DependencyProperty LabelSharedSizeGroupProperty = DependencyProperty.Register("LabelSharedSizeGroup", typeof(string), typeof(DPPasswordBox));
        public string LabelSharedSizeGroup
        {
            get { return (string)GetValue(LabelSharedSizeGroupProperty); }
            set { SetValue(LabelSharedSizeGroupProperty, value); }
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(DPPasswordBox), new PropertyMetadata(new PropertyChangedCallback(OnLabelChanged)));
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        private static readonly DependencyPropertyKey IsLabelShownPropertyKey = DependencyProperty.RegisterReadOnly("IsLabelShown", typeof(bool), typeof(DPPasswordBox), new PropertyMetadata(false));
        public static readonly DependencyProperty IsLabelShownProperty = IsLabelShownPropertyKey.DependencyProperty;
        public bool IsLabelShown
        {
            get { return (bool)GetValue(IsLabelShownProperty); }
        }

        protected static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = (string)e.NewValue;
            d.SetValue(IsLabelShownPropertyKey, label.Length != 0);
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DPPasswordBox));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty TrailingProperty = DependencyProperty.Register("Trailing", typeof(object), typeof(DPPasswordBox));
        public object Trailing
        {
            get { return GetValue(TrailingProperty); }
            set { SetValue(TrailingProperty, value); }
        }

        public DPPasswordBox()
        {
            DefaultStyleKey = typeof(DPPasswordBox);
        }

        private static readonly DependencyPropertyKey IsPasswordShownPropertyKey = DependencyProperty.RegisterReadOnly("IsPasswordShown", typeof(bool), typeof(DPPasswordBox), new PropertyMetadata(false));
        public static readonly DependencyProperty IsPasswordShownProperty = IsPasswordShownPropertyKey.DependencyProperty;
        public bool IsPasswordShown
        {
            get { return (bool)GetValue(IsPasswordShownProperty); }
        }

        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(DPPasswordBox), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged) { });
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        protected static void OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (DPPasswordBox)d;
            var password = (string)e.NewValue;

            if (!passwordBox.passwordChangedByMe)
            {
                if (passwordBox.IsPasswordShown)
                {
                    passwordBox.PART_ContentHostVisible.Text = password;
                }
                else
                {
                    passwordBox.PART_ContentHost.Password = password;
                }
            }
            else
            {
                passwordBox.passwordChangedByMe = false;
            }

            passwordBox.SetValue(IsPlaceholderShownPropertyKey, password == null || password.Length == 0);
        }

        private static readonly DependencyPropertyKey IsFocusesdPropertyKey = DependencyProperty.RegisterReadOnly("IsFocused", typeof(bool), typeof(DPPasswordBox), new PropertyMetadata(false));
        public static new readonly DependencyProperty IsFocusedProperty = IsFocusesdPropertyKey.DependencyProperty;
        public new bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
        }

        private PasswordBox PART_ContentHost;
        private TextBox PART_ContentHostVisible;
        private ToggleButton PART_ButtonPassword;

        private bool passwordChangedByMe = false;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ContentHost = GetTemplateChild("PART_ContentHost") as PasswordBox;
            PART_ContentHostVisible = GetTemplateChild("PART_ContentHostVisible") as TextBox;
            PART_ButtonPassword = GetTemplateChild("PART_ButtonPassword") as ToggleButton;

            if (PART_ContentHost != null)
            {
                PART_ContentHost.Padding = new Thickness(0);
                PART_ContentHost.Margin = new Thickness(0);
                PART_ContentHost.PasswordChanged += PART_ContentHost_PasswordChanged;
                PART_ContentHost.GotFocus += PART_ContentHost_GotFocus;
                PART_ContentHost.LostFocus += PART_ContentHost_LostFocus;
            }

            if (PART_ContentHostVisible != null)
            {
                PART_ContentHostVisible.Padding = new Thickness(0);
                PART_ContentHostVisible.Margin = new Thickness(0);
                PART_ContentHostVisible.TextChanged += PART_ContentHostVisible_TextChanged;
                PART_ContentHostVisible.GotFocus += PART_ContentHost_GotFocus;
                PART_ContentHostVisible.LostFocus += PART_ContentHost_LostFocus;
            }

            if (PART_ButtonPassword != null)
            {
                PART_ButtonPassword.Checked += PART_ButtonPassword_Checked;
                PART_ButtonPassword.Unchecked += PART_ButtonPassword_Unchecked;
            }
        }

        private void PART_ContentHost_PasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordChangedByMe = true;
            SetValue(PasswordProperty, PART_ContentHost.Password);
        }

        private void PART_ContentHost_GotFocus(object sender, RoutedEventArgs e)
        {
            SetValue(IsFocusesdPropertyKey, true);
        }

        private void PART_ContentHost_LostFocus(object sender, RoutedEventArgs e)
        {
            SetValue(IsFocusesdPropertyKey, false);
        }

        private void PART_ContentHostVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            passwordChangedByMe = true;
            SetValue(PasswordProperty, PART_ContentHostVisible.Text);
        }

        private void PART_ButtonPassword_Checked(object sender, RoutedEventArgs e)
        {
            PART_ContentHostVisible.Text = Password;
            SetValue(IsPasswordShownPropertyKey, true);
        }

        private void PART_ButtonPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            PART_ContentHost.Password = Password;
            SetValue(IsPasswordShownPropertyKey, false);
        }
    }
}
