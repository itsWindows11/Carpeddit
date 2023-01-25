using Carpeddit.App.ViewModels;

namespace Carpeddit.App.Models
{
    public sealed class PostDetailsNavigationInfo
    {
        public bool ShowFullPage { get; set; }

        public PostViewModel ItemData { get; set; }
    }
}
