using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Carpeddit.App.Collections
{
    public sealed class MailboxLoadingSource : IIncrementalSource<Message>
    {
        private IRedditService service = Ioc.Default.GetService<IRedditService>();
        private MessageListType type;

        public MailboxLoadingSource(MessageListType type = MessageListType.Inbox)
        {
            this.type = type;
        }

        public async Task<IEnumerable<Message>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return await service.GetMessagesAsync(type);
        }
    }
}
