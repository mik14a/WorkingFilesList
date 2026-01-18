// Working Files List
// Visual Studio extension tool window that shows a selectable list of files
// that are open in the editor
// Copyright © 2016 Anthony Fung

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace WorkingFilesList.ToolWindow.View
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;
    using WorkingFilesList.Core.Model;
    using WorkingFilesList.Core.Service.Locator;
    using WorkingFilesList.ToolWindow.View.Controls;

    /// <summary>
    /// Interaction logic for WorkingFilesWindowControl.
    /// </summary>
    public partial class WorkingFilesWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingFilesWindowControl"/> class.
        /// </summary>
        public WorkingFilesWindowControl()
        {
            this.InitializeComponent();
            this.Loaded += WorkingFilesWindowControlLoaded;

            void WorkingFilesWindowControlLoaded(object sender, RoutedEventArgs e)
            {
                var workingFilesWindowControl = (WorkingFilesWindowControl)sender;
                workingFilesWindowControl.ActiveDocumentsItemsControl.GotFocus += ActiveDocumentsItemsControl_GotFocus;

                // Monitor collection changes to maintain focus after document removal
                var documentMetadataManager = ViewModelService.DocumentMetadataManager;
                if (documentMetadataManager?.ActiveDocumentMetadata != null)
                {
                    ((INotifyCollectionChanged)documentMetadataManager.ActiveDocumentMetadata).CollectionChanged += ActiveDocumentMetadata_CollectionChanged;
                }
            }

            void ActiveDocumentsItemsControl_GotFocus(object sender, RoutedEventArgs e)
            {
                if (sender != e.OriginalSource) return;

                var itemsControl = (ItemsControl)sender;
                var documentMetadataManager = ViewModelService.DocumentMetadataManager;
                if (documentMetadataManager?.ActiveDocumentMetadata == null) return;

                var activeDocumentMetadata = documentMetadataManager.ActiveDocumentMetadata
                    .Cast<DocumentMetadata>()
                    .FirstOrDefault(documentMetadata => documentMetadata.IsActive);
                if (activeDocumentMetadata == null) return;

                var container = itemsControl.ItemContainerGenerator.ContainerFromItem(activeDocumentMetadata);
                if (container == null) return;

                var dragDropButton = FindVisualChild<DragDropButton>(container);
                if (dragDropButton == null) return;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    dragDropButton.Focus();
                }), DispatcherPriority.Input);
            }

            void ActiveDocumentMetadata_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                // When documents are removed (not added), ensure ItemsControl maintains focus
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Focus the ItemsControl itself to maintain keyboard navigation
                        ActiveDocumentsItemsControl.Focus();
                    }), DispatcherPriority.Background);
                }
            }
        }

        static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result) return result;
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null) return childOfChild;
            }
            return null;
        }

    }
}
