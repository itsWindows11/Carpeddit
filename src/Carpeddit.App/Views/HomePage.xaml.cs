using Carpeddit.App.Services;
using Carpeddit.Common.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page
    {
        //private ObservableCollection<SamplePost> SamplePosts { get; } = new();

        private readonly IRedditAuthService _authService = App.Services.GetService(typeof(IRedditAuthService)) as IRedditAuthService;

        public HomePage()
        {
            InitializeComponent();

            /*foreach (var i in Enumerable.Range(0, 1000))
            {
                var userFriendlyIndex = i + 1;
                SamplePosts.Add(new SamplePost()
                {
                    Title = $"Post {userFriendlyIndex}",
                    Description = $"Description for post {userFriendlyIndex}.",
                    User = "user",
                    Subreddit = "subreddit"
                });
            }*/

            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
        }
    }
}
