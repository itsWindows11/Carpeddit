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

        private Comment _originalComment;

        public Comment OriginalComment
        {
            get => _originalComment;
            set
            {
                _originalComment = value;
                _collapsed = OriginalComment.Collapsed;
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

        public Thickness Thickn => Replies.Count > 0 ? new(-10, 0, 0, 0) : (IsTopLevel ? new(-30, 0, 0, 0) : new(-10, 0, 0, 0));

        public string VoteRatio
        {
            get
            {
                return FormatNumber(OriginalComment.UpVotes - OriginalComment.DownVotes);
            }
            private set
            {
                OnPropertyChanged(nameof(VoteRatio));
            }
        }

        public int RawVoteRatio
        {
            get
            {
                return OriginalComment.UpVotes - OriginalComment.DownVotes;
            }
            set
            {
                VoteRatio = FormatNumber(value);
                OnPropertyChanged(nameof(RawVoteRatio));
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

        public Task<ObservableCollection<CommentViewModel>> GetRepliesAsync(bool addToRepliesList = false)
        {
            return Task.Run(() =>
            {
                ObservableCollection<CommentViewModel> comments = new();
                CommentViewModel currentCommentVm = this;
                currentCommentVm.IsTopLevel = true;

                // Loop to find the replies.
                // NOTE: I don't really understand how this just... works, really strange.
                foreach (Comment comment1 in currentCommentVm.OriginalComment.Replies)
                {
                    CommentViewModel commentVm = new()
                    {
                        OriginalComment = comment1
                    };

                    if (addToRepliesList)
                    {
                        Replies.Add(commentVm);
                    }
                    else
                    {
                        comments.Add(commentVm);
                    }

                    currentCommentVm = commentVm;
                }

                return comments;
            });
        }
    }
}
