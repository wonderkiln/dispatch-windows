using System.Windows.Controls;

namespace Dispatch.Fragments
{
    public partial class FavoriteNameView : UserControl
    {
        public new string Name
        {
            get
            {
                return NameTextBox.Text;
            }
        }

        public FavoriteNameView()
        {
            InitializeComponent();
        }
    }
}
