using Dispatch.Service.Client;
using Dispatch.Service.Model;
using System.Windows.Controls;

namespace Dispatch.View.Fragments
{
    public partial class TabView : UserControl
    {
        public TabView()
        {
            InitializeComponent();
        }

        private void ResourceView_BeginTransfer(object sender, Resource[] e)
        {
            foreach (var resource in e)
            {
                // LocalClient is always on the left side
                if (resource.Client is LocalClient)
                {
                    //ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Upload, resource, new Resource(ViewModel.RightViewModel.Client, ViewModel.RightViewModel.CurrentPath, ""), ViewModel.RightViewModel));
                }
                else
                {
                    //ResourceQueue.Shared.Enqueue(new ResourceQueue.Item(ResourceQueue.Item.ActionType.Download, resource, new Resource(ViewModel.LeftViewModel.Client, ViewModel.LeftViewModel.CurrentPath, ""), ViewModel.LeftViewModel));
                }
            }
        }
    }
}
