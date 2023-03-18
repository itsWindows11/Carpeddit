using Carpeddit.Api.Models;
using Carpeddit.App.Collections;
using Carpeddit.App.Views;
using Carpeddit.Common.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Carpeddit.App.ViewModels.Pages
{
    public sealed partial class MailboxPageViewModel : ObservableObject
    {
        public BulkIncrementalLoadingCollection<MailboxLoadingSource, Message> Items { get; }

        private readonly MailboxLoadingSource source;
        private bool loadedInitialItems;

        [ObservableProperty]
        private bool isLoadingMore;

        [ObservableProperty]
        private bool isLoading;

        public MailboxPageViewModel()
        {
            source = new();

            Items = new(source, 50, () =>
            {
                if (loadedInitialItems)
                    IsLoadingMore = true;
                else
                    IsLoading = true;
            }, () =>
            {
                loadedInitialItems = true;
                IsLoading = false;
                IsLoadingMore = false;
            });
        }

        public void GoToDetails(object parameter)
            => WeakReferenceMessenger.Default.Send(new MainFrameNavigationMessage()
            {
                Page = typeof(MailboxDetailsPage),
                Parameter = parameter
            });
    }
}
