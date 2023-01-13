using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page
    {
        private ObservableCollection<SamplePost> SamplePosts { get; } = new();

        public HomePage()
        {
            InitializeComponent();

            foreach (var i in Enumerable.Range(0, 1000))
            {
                var userFriendlyIndex = i + 1;
                SamplePosts.Add(new SamplePost()
                {
                    Title = $"Post {userFriendlyIndex}",
                    Description = $"Description for post {userFriendlyIndex}.",
                    User = "user",
                    Subreddit = "subreddit"
                });
            }
        }
    }

    public partial class SamplePost : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string user;

        [ObservableProperty]
        private string subreddit;
    }
}
