using Dispatch.Service.Model;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Dispatch.ViewModel
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
                if (!(item is BookmarkItem))
                {
                    dragInfo.Effects = DragDropEffects.None;
                    return;
                }
            }

            dragInfo.Data = sourceItems.Cast<BookmarkItem>();
            dragInfo.Effects = DragDropEffects.Move;
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IEnumerable<BookmarkItem>)
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
            if (dropInfo.Data is IEnumerable<BookmarkItem> bookmarks)
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
                    viewModel.Insert(dropInfo.InsertIndex, new BookmarkItem(resource));
                }
            }
        }
    }

    public class BookmarksViewModel : StorageViewModel<BookmarkItem>
    {
        public BookmarksDropTarget DragDropHandler { get; }

        public BookmarksViewModel() : base("Bookmarks.json")
        {
            DragDropHandler = new BookmarksDropTarget(this);
        }
    }
}
