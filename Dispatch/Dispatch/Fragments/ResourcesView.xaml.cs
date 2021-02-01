using System.Windows.Controls;
using System.Windows.Input;

namespace Dispatch.Fragments
{
    public partial class ResourcesView : UserControl
    {
        public ResourcesView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                textBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();

                Keyboard.ClearFocus();
            }
        }
    }
}
