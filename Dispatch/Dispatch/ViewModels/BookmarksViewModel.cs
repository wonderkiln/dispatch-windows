using Dispatch.Service.Models;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace Dispatch.ViewModels
{
    public class BookmarksDropTarget : IDragSource, IDropTarget
    {
        private readonly BookmarksViewModel viewModel;

        public BookmarksDropTarget(BookmarksViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public void DragCancelled()
        {
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
        }

        public void Dropped(IDropInfo dropInfo)
        {
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            var sourceItems = (IEnumerable<object>)dragInfo.SourceItems;

            foreach (var item in sourceItems)
            {
                if (!(item is Bookmark))
                {
                    dragInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dragInfo.Data = sourceItems.Cast<Bookmark>();
            dragInfo.Effects = DragDropEffects.Move;
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<Bookmark>)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
            else if (dropInfo.Data is IEnumerable<Resource>)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Link;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<Bookmark> bookmarks)
            {
                foreach (var bookmark in bookmarks)
                {
                    viewModel.Move(dropInfo.InsertIndex, bookmark);
                }
            }
            else if (dropInfo.Data is IEnumerable<Resource> resources)
            {
                foreach (var resource in resources)
                {
                    viewModel.Insert(dropInfo.InsertIndex, new Bookmark(resource));
                }
            }
        }
    }

    public class BookmarksViewModel : StorageViewModel<Bookmark>
    {
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);


        public BookmarksDropTarget DragDropHandler { get; }

        public BookmarksViewModel() : base("Bookmarks.json")
        {
            DragDropHandler = new BookmarksDropTarget(this);

            // Populate with User Folder, Desktop, and Downloads if there are no bookmarks
            if (Items.Count == 0)
            {
                Items.Add(GetLocalBookmark(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));

                var desktop = GetPath(KnownFolder.Desktop);
                if (desktop != null) Items.Add(GetLocalBookmark(desktop));

                var downloads = GetPath(KnownFolder.Downloads);
                if (downloads != null) Items.Add(GetLocalBookmark(downloads));
            }
        }

        private enum KnownFolder
        {
            Desktop = 0,
            Downloads = 1,
        }

        private readonly string[] knownFolderGuids = new string[]
        {
            "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
            "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
        };

        private string GetPath(KnownFolder knownFolder)
        {
            var result = SHGetKnownFolderPath(new Guid(knownFolderGuids[(int)knownFolder]), 0, IntPtr.Zero, out string outPath);
            if (result >= 0) return outPath;
            return null;
        }

        private Bookmark GetLocalBookmark(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return new Bookmark(directoryInfo.Name, path, true);
        }
    }
}
