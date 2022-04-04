using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.App.Models
{
    public class CommentViewModel
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
            }
        }

        private ObservableCollection<CommentViewModel> _replies;

        public ObservableCollection<CommentViewModel> Replies
        {
            get => _replies;
            set => _replies = value;
        }

        private bool _collapsed = false;

        public bool Collapsed
        {
            get => _collapsed;
            set
            {
                _collapsed = value;
                OriginalComment.Collapsed = value;
            }
        }

        public bool Expanded => !Collapsed;

        private bool _isTopLevel;

        public bool IsTopLevel
        {
            get => _isTopLevel;
            set => _isTopLevel = value;
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

        public Task<ObservableCollection<CommentViewModel>> GetRepliesAsync(CommentViewModel comment)
        {
            return Task.Run(() =>
            {
                ObservableCollection<CommentViewModel> comments = new();

                // Loop 4 levels deep to find the replies.
                foreach (Comment comment1 in comment.OriginalComment.Replies)
                {
                    CommentViewModel comment1Vm = new()
                    {
                        OriginalComment = comment1
                    };

                    comments.Add(comment1Vm);

                    foreach (Comment comment2 in comment1.Replies)
                    {
                        CommentViewModel comment2Vm = new()
                        {
                            OriginalComment = comment2
                        };

                        comment1Vm.Replies.Add(comment2Vm);

                        foreach (Comment comment3 in comment2.Replies)
                        {
                            CommentViewModel comment3Vm = new()
                            {
                                OriginalComment = comment3
                            };

                            comment2Vm.Replies.Add(comment2Vm);

                            foreach (Comment comment4 in comment3.Replies)
                            {
                                CommentViewModel comment4Vm = new()
                                {
                                    OriginalComment = comment4
                                };

                                comment3Vm.Replies.Add(comment2Vm);
                            }
                        }
                    }
                }

                return comments;
            });
        }
    }
}
