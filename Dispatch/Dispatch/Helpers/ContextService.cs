using Dispatch.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dispatch.Helpers
{
    public interface IContextServiceActions
    {
        void ContextServiceOpen(Resource resource);

        void ContextServiceTransfer(Resource[] resources);

        void ContextServiceDelete(Resource[] resources);
    }

    public class ContextService
    {
        public static void Show(Resource[] resources, UIElement target, IContextServiceActions action)
        {
            if (resources == null || resources.Length == 0) return;

            var context = new ContextMenu();

            if (resources.Length == 1) context.Items.Add(CreateMenu("\uE838", "Open", "Enter", () => action.ContextServiceOpen(resources[0])));
            context.Items.Add(CreateMenu("\uE896", "Transfer", "Ctrl+T", () => action.ContextServiceTransfer(resources)));
            context.Items.Add(new Separator());
            if (resources.Length == 1 && resources[0].Type != ResourceType.Drive) context.Items.Add(CreateMenu("\uE8AC", "Rename", "Ctrl+R"));
            if (resources.Count(e => e.Type == ResourceType.Drive) == 0) context.Items.Add(CreateMenu("\uE74D", "Delete", "Del", () => action.ContextServiceDelete(resources)));
            context.Items.Add(new Separator());
            if (resources.Length == 1 && resources[0].Type == ResourceType.File) context.Items.Add(CreateMenu("\uE946", "File Info", "Ctrl+I"));

            var separatorsToRemove = new List<object>();

            for (int i = 0; i < context.Items.Count; i++)
            {
                var prev = i == 0 ? null : context.Items[i - 1];
                var curr = context.Items[i];
                var next = i == context.Items.Count - 1 ? null : context.Items[i + 1];

                if (curr is Separator)
                {
                    if (!(prev is MenuItem && next is MenuItem))
                    {
                        separatorsToRemove.Add(curr);
                    }
                }
            }

            foreach (var item in separatorsToRemove)
            {
                context.Items.Remove(item);
            }

            context.PlacementTarget = target;
            context.IsOpen = true;
        }

        private static UIElement CreateIcon(string icon)
        {
            var textBlox = new TextBlock();
            textBlox.FontFamily = new FontFamily("Segoe MDL2 Assets");
            textBlox.Text = icon;
            return textBlox;
        }

        private static UIElement CreateMenu(string icon, string name, string shortcut, Action action = null)
        {
            var menu = new MenuItem();
            menu.Icon = CreateIcon(icon);
            menu.Header = name;
            menu.InputGestureText = shortcut;
            menu.Click += (sender, e) => action?.Invoke();
            return menu;
        }
    }
}
