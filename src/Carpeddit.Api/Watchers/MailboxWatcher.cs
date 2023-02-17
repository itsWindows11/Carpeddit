using Carpeddit.Api.Enums;
using Carpeddit.Api.EqualityComparers;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Carpeddit.Api.Watchers
{
    public sealed class MailboxWatcher
    {
        public List<Message> KnownItems { get; private set; } = new();

        public event EventHandler<MailboxUpdateEventArgs> MailboxUpdated;

        private IRedditService _service;
        private MessageListType _type;

        private MailboxWatcher(IRedditService service, MessageListType type)
        {
            _service = service;
            _type = type;
        }

        public static async Task<MailboxWatcher> WatchMailboxAsync(IRedditService service, TimeSpan intervalDuration, MessageListType listType = MessageListType.Inbox)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            var mailboxWatcher = new MailboxWatcher(service, listType);

            var messages = await service.GetMessagesAsync(listType);

            mailboxWatcher.KnownItems.AddRange(messages);

            var timer = new DispatcherTimer()
            {
                Interval = intervalDuration
            };

            timer.Tick += mailboxWatcher.OnTimerTick;
            timer.Start();

            return mailboxWatcher;
        }

        private async void OnTimerTick(object sender, object e)
        {
            var messages = await _service.GetMessagesAsync(_type);

            var newMessages = messages.Where(m => m.New && !KnownItems.Any(m1 => m1.Name.Equals(m.Name))).ToList();

            KnownItems.AddRange(newMessages);
            MailboxUpdated?.Invoke(this, new(newMessages));
        }

        public sealed class MailboxUpdateEventArgs
        {
            public IReadOnlyList<Message> Messages { get; }

            public MailboxUpdateEventArgs(IReadOnlyList<Message> messages)
                => Messages = messages;
        }
    }
}
