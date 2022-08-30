using System;
using Carpeddit.App.Pages;
using Carpeddit.Common.Helpers;
using Carpeddit.Common.Interfaces;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Reddit.Controllers;
using Reddit.Things;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.ViewModels
{
    // Properties
    public partial class PostViewModel : ViewModel
    {
        private Reddit.Controllers.Post _post;

        public Reddit.Controllers.Post Post
        {
            get => _post;
            set
            {
                _post = value;

                _upvoted = value.IsUpvoted;
                _downvoted = value.IsDownvoted;
            }
        }

        private string _title;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _author;

        public string Author
        {
            get => _author;
            set => Set(ref _author, value);
        }

        private DateTime _created;

        public DateTime Created
        {
            get => _created;
            set => Set(ref _created, value);
        }

        private string _subreddit;

        public string Subreddit
        {
            get => _subreddit;
            set => Set(ref _subreddit, value);
        }

        private string _description;
        
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public bool ShouldDisplayUserFlair
            => !string.IsNullOrEmpty(Post.Listing.AuthorFlairText);

        public bool ShouldDisplayPostFlair
            => !string.IsNullOrEmpty(Post.Listing.LinkFlairText);

        public bool IsCurrentUserOP
            => App.RedditClient.Account.Me.Name == Author;

        public bool IsModDistinguished
            => Post.Listing.Distinguished == "moderator";

        public bool IsAdminDistinguished
            => Post.Listing.Distinguished == "admin";

        private string _shortDescription;

        public string ShortDescription
        {
            get
            {
                _shortDescription ??= Description.Length >= 350 ? Description.Substring(0, 350) + "..." : Description;
                return _shortDescription;
            }
        }

        public Uri ImageUri
            => HasImage ? new Uri(Description, UriKind.Absolute) : null;

        public bool IsGallery
            => Post.Listing.IsGallery ?? false;

        private List<Reddit.Things.Image> _images;

        public List<Reddit.Things.Image> Images
        {
            get
            {
                _images ??= new();

                if (Post.Listing.MediaMetadata != null && !_images.Any())
                {
                    foreach (var image in Post.Listing.MediaMetadata)
                    {
                        var image1 = image.Value.OriginalImage;

                        if (image1 != null && image1.Url != null)
                        {
                            image1.Url = image1.Url.Replace("preview.redd.it", "i.redd.it");

                            _images.Add(image1);
                        }
                    }
                }

                return _images;
            }
        }

        private int _commentsCount;

        public int CommentsCount
        {
            get => _commentsCount;
            set => Set(ref _commentsCount, value);
        }

        public string CommentsCountInUI
            => $"{CommentsCount} comment{(CommentsCount == 1 ? string.Empty : "s")}";

        public bool HasImage
            => CheckHasImage();

        private MediaSource _videoSource;

        public MediaSource VideoSource
        {
            get
            {
                if (Post is LinkPost post &&
                    Uri.TryCreate(post.URL + "/DASHPlaylist.mpd", UriKind.Absolute, out Uri uri))
                {
                    _videoSource ??= MediaSource.CreateFromUri(uri);
                }
                
                return _videoSource;
            }
        }

        public string VoteRatio
        {
            get => (Post.UpVotes - Post.DownVotes).Format();
            private set => OnPropertyChanged(nameof(VoteRatio));
        }

        public int RawVoteRatio
        {
            get => Post.UpVotes - Post.DownVotes;
            set
            {
                VoteRatio = value.Format();
                OnPropertyChanged(nameof(RawVoteRatio));
            }
        }
    }

    // IVotable implementation
    public partial class PostViewModel : IVotable
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
            Post.Upvote();
            Upvoted = true;
            Downvoted = false;
            RawVoteRatio += 1;
        }

        public void Downvote()
        {
            Post.Downvote();
            Upvoted = false;
            Downvoted = true;
            RawVoteRatio -= 1;
        }

        public void Unvote()
        {
            Post.Unvote();
            InvalidateUnvoteCounter();
            Upvoted = false;
            Downvoted = false;
        }

        public async Task UpvoteAsync()
        {
            await Post.UpvoteAsync();
            Upvoted = true;
            Downvoted = false;
            RawVoteRatio += 1;
        }
        
        public async Task DownvoteAsync()
        {
            await Post.DownvoteAsync();
            Upvoted = false;
            Downvoted = true;
            RawVoteRatio -= 1;
        }

        public async Task UnvoteAsync()
        {
            await Post.UnvoteAsync();
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
    public partial class PostViewModel : IArchivable
    {
        public bool Archived
            => Post.Listing.Archived;

        public bool Locked
            => Post.Listing.Locked;

        public bool ArchivedOrLocked
            => Post.Listing.Archived || Post.Listing.Locked;

        public bool NotArchivedOrLocked
            => !ArchivedOrLocked;

        public void Archive()
        {
            throw new NotImplementedException();
        }

        public Task ArchiveAsync()
        {
            throw new NotImplementedException();
        }

        public void Unarchive()
        {
            throw new NotImplementedException();
        }

        public Task UnarchiveAsync()
        {
            throw new NotImplementedException();
        }
    }

    // IPinnable, IRemovable and IApprovable (Moderation essentials) implementation
    public partial class PostViewModel : IPinnable, IRemovable, IApprovable
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
            // Retrieve first 3 posts of the subreddit, the first 2 are pinned (if not then there's at least one empty slot).
            var pinned = App.RedditClient.Subreddit(name: Post.Subreddit).Posts.GetHot(limit: 3);
            int indexToInsert = 1;

            if (!pinned[0].Listing.Stickied || (pinned[0].Listing.Stickied && pinned[1].Listing.Stickied))
            {
                indexToInsert = 1;
            }
            else if (!pinned[1].Listing.Stickied || pinned[0].Listing.Stickied)
            {
                indexToInsert = 2;
            }

            // Sticky the post, finally.
            // This function has misleading documentation and I have no idea how to fix it, so the above
            // if statements are used here as a workaround.
            Post.SetSubredditSticky(indexToInsert, false);
        }

        public void Remove()
        {
            Post.Remove();
            InvalidateModProperties(true, false, false);
        }

        public void Spam()
        {
            Post.Remove(true);
            InvalidateModProperties(false, true, false);
        }

        public void Unpin()
        {
            throw new NotImplementedException();
        }

        public void Approve()
        {
            Post.Approve();
            InvalidateModProperties(false, false, true);
        }

        public async Task PinAsync()
        {
            // Retrieve first 3 posts of the subreddit, the first 2 are pinned (if not then there's at least one empty slot).
            var pinned = await Task.Run(() => App.RedditClient.Subreddit(name: Post.Subreddit).Posts.GetHot(limit: 3));
            int indexToInsert = 1;

            if (!pinned[0].Listing.Stickied || (pinned[0].Listing.Stickied && pinned[1].Listing.Stickied))
            {
                indexToInsert = 1;
            }
            else if (!pinned[1].Listing.Stickied || pinned[0].Listing.Stickied)
            {
                indexToInsert = 2;
            }

            // Sticky the post, finally.
            // This function has misleading documentation and I have no idea how to fix it, so the above
            // if statements are used here as a workaround.
            await Post.SetSubredditStickyAsync(indexToInsert, false);
        }

        public async Task RemoveAsync()
        {
            await Post.RemoveAsync();
            InvalidateModProperties(true, false, false);
        }

        public async Task SpamAsync()
        {
            await Post.RemoveAsync(true);
            InvalidateModProperties(false, true, false);
        }

        public Task UnpinAsync()
        {
            throw new NotImplementedException();
        }

        public Task ApproveAsync()
            => Task.Run(() => Post.Approve());

        private void InvalidateModProperties(bool removed, bool spammed, bool approved)
        {
            Removed = removed;
            Spammed = spammed;
            Approved = approved;
        }
    }

    // Helper functions
    public partial class PostViewModel
    {
        public Task DeleteAsync()
            => Post.DeleteAsync();

        public async IAsyncEnumerable<CommentViewModel> GetCommentsAsync(string sortType = "top", CoreDispatcher dispatcher = null)
        {
            List<Task> commentLoadingTasks = new();
            bool isCurrentUserMod = App.RedditClient.Subreddit(Post.Subreddit).About().SubredditData.UserIsModerator ?? false;

            var comments = await Task.Run(() => Post.Comments.GetComments(sort: sortType));

            foreach (Reddit.Controllers.Comment comment in comments)
            {
                CommentViewModel comment1 = new()
                {
                    OriginalComment = comment,
                    IsTopLevel = true,
                    IsCurrentUserMod = isCurrentUserMod
                };

                yield return comment1;

                _ = Task.Run(() => _ = comment1.GetReplies(dispatcher, true, isCurrentUserMod));
            }
        }

        private bool CheckHasImage()
        {
            return Uri.IsWellFormedUriString(Description, UriKind.Absolute)
                && (Description.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || Description.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                && (Description.Contains("i.redd.it") || Description.Contains("i.imgur"));
        }

        // Markdown link clicked event handler
        public async void OnMarkdownLinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.Absolute, out Uri link))
            {
                _ = await Launcher.LaunchUriAsync(link);
                return;
            }

            if (e.Link.StartsWith("/r/"))
            {
                (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: e.Link.Substring(3)).About());
                return;
            }

            if (e.Link.StartsWith("r/"))
            {
                (Window.Current.Content as Frame).Navigate(typeof(SubredditPage), App.RedditClient.Subreddit(name: e.Link.Substring(2)).About());
                return;
            }

            if (e.Link.StartsWith("/u/"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(name: e.Link.Substring(3)).About());
                return;
            }

            if (e.Link.StartsWith("u/"))
            {
                MainPage.Current.ContentFrame.Navigate(typeof(YourProfilePage), App.RedditClient.User(name: e.Link.Substring(2)).About());
                return;
            }
        }
    }
}
