using Carpeddit.Api.Enums;
using Carpeddit.Api.Models;
using Carpeddit.Api.Services;
using Carpeddit.App.ViewModels;
using Carpeddit.Common.Collections;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Carpeddit.App.Collections
{
    public sealed class PostLoadingSource : IIncrementalSource<PostViewModel>
    {
        public List<PostViewModel> Items { get; } = new();

        private IRedditService service;
        private string subreddit;

        public SortMode CurrentSort { get; set; }

        public PostLoadingSource(string subreddit = null, SortMode sort = SortMode.Best)
        {
            service = Ioc.Default.GetService<IRedditService>();
            CurrentSort = sort;
            this.subreddit = subreddit;
        }

        public async Task<IEnumerable<PostViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            IEnumerable<Post> postListings;
            IEnumerable<PostViewModel> posts;

            var listingInput = new ListingInput(limit: pageSize);
            var lastItem = Items.LastOrDefault();

            if (pageIndex > 0 && lastItem != null)
                listingInput.After = lastItem.Post.Name;
            else if (pageIndex <= 0 && lastItem != null)
                Items.Clear();

            try
            {
                if (string.IsNullOrWhiteSpace(subreddit))
                {
                    postListings = await service.GetFrontpagePostsAsync(CurrentSort, listingInput);
                }
                else if (subreddit.StartsWith("u/") || subreddit.StartsWith("/u/"))
                {
                    postListings =
                        await service.GetUserPostsAsync(subreddit.Replace("/u/", string.Empty).Replace("u/", string.Empty), CurrentSort, listingInput);
                }
                else
                {
                    postListings =
                        await service.GetSubredditPostsAsync(subreddit.Replace("/r/", string.Empty).Replace("r/", string.Empty), CurrentSort, listingInput);
                }

                posts = postListings.Select(p => new PostViewModel()
                {
                    Post = p
                });

                Items.AddRange(posts);

                return posts;
            } catch { }

            return Enumerable.Empty<PostViewModel>();
        }
    }
}
