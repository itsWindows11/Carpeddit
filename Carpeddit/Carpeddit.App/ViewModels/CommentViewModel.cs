using Carpeddit.App.Collections;
using Carpeddit.Common.Helpers;
using Carpeddit.Common.Interfaces;
using Reddit.Controllers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Carpeddit.App.ViewModels
{
    public partial class CommentViewModel : ViewModel
    {
        public CommentViewModel()
        {
            _replies = new();
        }

        private Comment _originalComment;

        public Comment OriginalComment
        {
            get => _originalComment;
            set
            {
                _originalComment = value;
                _collapsed = _originalComment.Collapsed;
                _voteRatio = (_originalComment.UpVotes - _originalComment.DownVotes).Format();
                _rawVoteRatio = _originalComment.UpVotes - _originalComment.DownVotes;
                _upvoted = _originalComment.IsUpvoted;
                _downvoted = _originalComment.IsDownvoted;
                GetCanLoadMoreAsync().Wait();

                OnPropertyChanged(nameof(OriginalComment));
            }
        }

        private CommentViewModel _parentComment;

        public CommentViewModel ParentComment
        {
            get => _parentComment;
            set => Set(ref _parentComment, value);
        }

        private bool _isCurrentUserMod;

        public bool IsCurrentUserMod
        {
            get => _isCurrentUserMod;
            set => Set(ref _isCurrentUserMod, value);
        }

        public bool IsCurrentUserOP
            => App.RedditClient.Account.Me.Name == OriginalComment.Author;

        public bool IsModDistinguished 
            => OriginalComment.Listing.Distinguished == "moderator";

        public bool IsAdminDistinguished 
            => OriginalComment.Listing.Distinguished == "admin";

        public bool ShouldDisplayUserFlair
            => !string.IsNullOrEmpty(OriginalComment.Listing.AuthorFlairText);

        public bool Expanded => !Collapsed;
        
        public Thickness Thickn
            => Replies.Count > 0 ? new(-12, 0, 0, 0) : (IsTopLevel ? new(-32, 0, 0, 0) : new(-12, 0, 0, 0));

        private bool _showReplyUI;

        public bool ShowReplyUI
        {
            get => _showReplyUI;
            set => Set(ref _showReplyUI, value);
        }

        private bool? _canLoadMore;

        public bool? CanLoadMore
        {
            get
            {
                if (OriginalComment.Listing.Replies != null && OriginalComment.Listing.Replies.MoreData != null)
                {
                    _canLoadMore ??= OriginalComment.Listing.Replies.MoreData.Any();
                    return _canLoadMore;
                }

                _canLoadMore = false;
                return _canLoadMore;
            }
            set => Set(ref _canLoadMore, value);
        }

        private BulkConcurrentObservableCollection<CommentViewModel> _replies;

        public BulkConcurrentObservableCollection<CommentViewModel> Replies
        {
            get => _replies;
            set => Set(ref _replies, value);
        }

        private bool _collapsed = false;

        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                _collapsed = value;
                OriginalComment.Collapsed = value;
                OnPropertyChanged(nameof(Collapsed));
            }
        }

        private bool _isTopLevel;

        public bool IsTopLevel
        {
            get => _isTopLevel;
            set => _isTopLevel = value;
        }

        private string _voteRatio;

        public string VoteRatio
        {
            get => _voteRatio;
            private set => Set(ref _voteRatio, value);
        }

        private int _rawVoteRatio;

        public int RawVoteRatio
        {
            get => _rawVoteRatio;
            set
            {
                _rawVoteRatio = value;
                VoteRatio = value.Format();
                OnPropertyChanged(nameof(VoteRatio));
                OnPropertyChanged(nameof(RawVoteRatio));
            }
        }
    }

    // IVotable implementation
    public partial class CommentViewModel : IVotable
    {
        private bool _upvoted;

        public bool Upvoted
        {
            get => _upvoted;
            set => Set(ref _upvoted, value);
        }

        private bool _downvoted;

        public bool Downvoted
        {
            get => _downvoted;
            set => Set(ref _downvoted, value);
        }

        public void Upvote()
        {
            OriginalComment.Upvote();
            Upvoted = true;
            Downvoted = false;
            RawVoteRatio += 1;
        }

        public void Downvote()
        {
            OriginalComment.Downvote();
            Upvoted = false;
            Downvoted = true;
            RawVoteRatio -= 1;
        }

        public void Unvote()
        {
            OriginalComment.Unvote();
            InvalidateUnvoteCounter();
            Upvoted = false;
            Downvoted = false;
        }

        public async Task UpvoteAsync()
        {
            await OriginalComment.UpvoteAsync();
            Upvoted = true;
            Downvoted = false;
            RawVoteRatio += 1;
        }

        public async Task DownvoteAsync()
        {
            await OriginalComment.DownvoteAsync();
            Upvoted = false;
            Downvoted = true;
            RawVoteRatio -= 1;
        }

        public async Task UnvoteAsync()
        {
            await OriginalComment.UnvoteAsync();
            InvalidateUnvoteCounter();
            Upvoted = false;
            Downvoted = false;
        }

        private void InvalidateUnvoteCounter()
        {
            if (Upvoted)
                RawVoteRatio--;
            else
                RawVoteRatio++;
        }
    }

    // IArchivable implementation
    public partial class CommentViewModel : IArchivable
    {
        public bool Archived
            => OriginalComment.Listing.Archived;

        public bool Locked
            => OriginalComment.Listing.Locked;

        public bool ArchivedOrLocked
            => Archived || Locked;

        public bool NotArchivedOrLocked
            => !ArchivedOrLocked;

        public void Archive()
        {
            throw new NotImplementedException();
        }
        
        public void Unarchive()
        {
            throw new NotImplementedException();
        }

        public Task ArchiveAsync()
        {
            throw new NotImplementedException();
        }

        public Task UnarchiveAsync()
        {
            throw new NotImplementedException();
        }
    }

    // IPinnable, IRemovable and IApprovable (Moderation essentials) implementation
    public partial class CommentViewModel : IPinnable, IRemovable, IApprovable
    {
        private bool _pinned;

        public bool Pinned
        {
            get => _pinned;
            set => Set(ref _pinned, value);
        }

        private bool _removed;

        public bool Removed
        {
            get => _removed;
            set => Set(ref _removed, value);
        }

        private bool _spammed;

        public bool Spammed
        {
            get => _spammed;
            set => Set(ref _spammed, value);
        }

        private bool _approved;

        public bool Approved
        {
            get => _approved;
            set => Set(ref _approved, value);
        }

        public void Pin()
        {
            OriginalComment.Distinguish("yes", true);
        }

        public void Remove()
        {
            OriginalComment.Remove();
            InvalidateModProperties(true, false, false);
        }

        public void Spam()
        {
            OriginalComment.Remove(true);
            InvalidateModProperties(false, true, false);
        }

        public void Unpin()
        {
            throw new NotImplementedException();
        }

        public void Approve()
        {
            App.RedditClient.Account.Dispatch.Moderation.Approve(OriginalComment.Fullname);
            InvalidateModProperties(false, false, true);
        }

        public Task PinAsync()
        {
            return OriginalComment.DistinguishAsync("yes", true);
        }

        public async Task RemoveAsync()
        {
            await OriginalComment.RemoveAsync();
            InvalidateModProperties(true, false, false);
        }

        public async Task SpamAsync()
        {
            await OriginalComment.RemoveAsync(true);
            InvalidateModProperties(false, true, false);
        }

        public Task UnpinAsync()
        {
            throw new NotImplementedException();
        }

        public Task ApproveAsync()
            => App.RedditClient.Account.Dispatch.Moderation.ApproveAsync(OriginalComment.Fullname);

        private void InvalidateModProperties(bool removed, bool spammed, bool approved)
        {
            Removed = removed;
            Spammed = spammed;
            Approved = approved;
        }
    }

    // Helper functions
    public partial class CommentViewModel
    {
        public Task DeleteAsync()
        {
            return OriginalComment.DeleteAsync();
        }
        
        public Task DistinguishAsModeratorAsync()
        {
            return OriginalComment.DistinguishAsync("yes", false);
        }

        public Task RemoveDistinguishAsync()
        {
            return OriginalComment.DistinguishAsync("no", false);
        }

        public Task UnlockCommentsAsync()
        {
            return App.RedditClient.Account.Dispatch.LinksAndComments.UnlockAsync(OriginalComment.Fullname);
        }

        public Task LockCommentsAsync()
        {
            return App.RedditClient.Account.Dispatch.LinksAndComments.LockAsync(OriginalComment.Fullname);
        }

        public Task<bool> GetCanLoadMoreAsync()
            => Task.Run(() =>
            {
                var canLoadMore = false;

                if (OriginalComment.Listing.Replies != null && OriginalComment.Listing.Replies.MoreData != null)
                    canLoadMore = OriginalComment.Listing.Replies.MoreData.Any();

                CanLoadMore = canLoadMore;

                return canLoadMore;
            });

        public ObservableCollection<CommentViewModel> GetReplies(CoreDispatcher dispatcher = null, bool addToRepliesList = false, bool isCurrentUserMod = false)
        {
            ObservableCollection<CommentViewModel> comments = new();

            // Loop to find the replies.
            foreach (Comment comment1 in OriginalComment.Replies)
            {
                CommentViewModel commentVm = new()
                {
                    OriginalComment = comment1,
                    ParentComment = this,
                    IsCurrentUserMod = isCurrentUserMod
                };

                if (addToRepliesList && dispatcher != null)
                {
                    _ = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Replies.Add(commentVm));
                }
                else
                {
                    comments.Add(commentVm);
                }

                _ = commentVm.GetReplies(dispatcher, true, isCurrentUserMod);
            }

            return comments;
        }
    }
}
