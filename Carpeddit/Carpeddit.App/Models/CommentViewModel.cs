using CommunityToolkit.Mvvm.ComponentModel;
using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Carpeddit.App.Models
{
    public class CommentViewModel : ObservableObject
    {
        public CommentViewModel()
        {
            _replies = new();
        }

        public bool IsCurrentUserOP => App.RedditClient.Account.Me.Name == OriginalComment.Author;

        private bool _isCurrentUserMod;

        public bool IsCurrentUserMod
        {
            get => _isCurrentUserMod;
            set
            {
                _isCurrentUserMod = value;
                OnPropertyChanged(nameof(IsCurrentUserMod));
            }
        }

        public bool IsModDistinguished => OriginalComment.Listing.Distinguished == "moderator";

        public bool IsAdminDistinguished => OriginalComment.Listing.Distinguished == "admin";

        private Comment _originalComment;

        public Comment OriginalComment
        {
            get => _originalComment;
            set
            {
                _originalComment = value;
                _collapsed = _originalComment.Collapsed;
                _voteRatio = FormatNumber(_originalComment.UpVotes - _originalComment.DownVotes);
                _rawVoteRatio = _originalComment.UpVotes - _originalComment.DownVotes;
                _upvoted = _originalComment.IsUpvoted;
                _downvoted = _originalComment.IsDownvoted;

                OnPropertyChanged(nameof(OriginalComment));
            }
        }

        private bool _showReplyUI;

        public bool ShowReplyUI
        {
            get => _showReplyUI;
            set
            {
                _showReplyUI = value;
                OnPropertyChanged(nameof(ShowReplyUI));
            }
        }

        public bool ArchivedOrLocked => OriginalComment.Listing.Archived || OriginalComment.Listing.Locked;

        public bool NotArchivedOrLocked => !ArchivedOrLocked;
        

        private CommentViewModel _parentComment;

        public CommentViewModel ParentComment
        {
            get => _parentComment;
            set
            {
                _parentComment = value;
                OnPropertyChanged(nameof(OriginalComment));
            }
        }

        private ObservableCollection<CommentViewModel> _replies;

        public ObservableCollection<CommentViewModel> Replies
        {
            get => _replies;
            set
            {
                _replies = value;
                OnPropertyChanged(nameof(Replies));
            }
        }

        public bool ShouldDisplayUserFlair => !string.IsNullOrEmpty(OriginalComment.Listing.AuthorFlairText);

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

        public bool Expanded => !Collapsed;

        private bool _isTopLevel;

        public bool IsTopLevel
        {
            get => _isTopLevel;
            set => _isTopLevel = value;
        }

        public Thickness Thickn => Replies.Count > 0 ? new(-12, -2, 0, -4) : (IsTopLevel ? new(-32, -2, 0, -4) : new(-12, -2, 0, -4));

        private string _voteRatio;

        public string VoteRatio
        {
            get => _voteRatio;
            private set
            {
                _voteRatio = value;
                OnPropertyChanged(nameof(VoteRatio));
            }
        }

        private int _rawVoteRatio;

        public int RawVoteRatio
        {
            get => _rawVoteRatio;
            set
            {
                _rawVoteRatio = value;
                VoteRatio = FormatNumber(value);
                OnPropertyChanged(nameof(VoteRatio));
                OnPropertyChanged(nameof(RawVoteRatio));
            }
        }

        private bool _upvoted;

        public bool Upvoted
        {
            get => _upvoted;
            set
            {
                _upvoted = value;

                OnPropertyChanged(nameof(Upvoted));
            }
        }

        private bool _downvoted;

        public bool Downvoted
        {
            get => _downvoted;
            set
            {
                _downvoted = value;

                OnPropertyChanged(nameof(Downvoted));
            }
        }

        public static string FormatNumber(long num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##k");
            }

            return num.ToString("#,0");
        }

        public ObservableCollection<CommentViewModel> GetReplies(bool addToRepliesList = false, bool isCurrentUserMod = false)
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

                if (addToRepliesList)
                {
                    Replies.Add(commentVm);
                }
                else
                {
                    comments.Add(commentVm);
                }

                _ = commentVm.GetReplies(true, isCurrentUserMod);
            }

            return comments;
        }
    }
}
