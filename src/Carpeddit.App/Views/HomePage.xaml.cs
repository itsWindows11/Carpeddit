using Carpeddit.App.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.Views
{
    public sealed partial class HomePage : Page
    {
        //private ObservableCollection<SamplePost> SamplePosts { get; } = new();

        private readonly IRedditAuthService _authService = App.Services.GetService<IRedditAuthService>();
        private readonly IRedditService _service = App.Services.GetService<IRedditService>();

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
            var valut = App.Valut.Retrieve("Reddit", "itsWindows11");
            valut.RetrievePassword();
            var password = valut.Password;

            var result = (await _service.GetFrontpagePostsAsync(JsonSerializer.Deserialize<JsonElement>(password).GetProperty("accessToken").GetString())).Data.Children;

            var items = await Task.Run(() => result.Select(o => o.Data));

            MainList.ItemsSource = items;
            HomeRing.IsIndeterminate = false;
            HomeRing.Visibility = Visibility.Collapsed;
        }
    }
}
