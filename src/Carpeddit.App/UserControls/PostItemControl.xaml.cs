using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace Carpeddit.App.UserControls
{
    public sealed partial class PostItemControl : UserControl
    {
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }

        public string SubredditName
        {
            get => (string)GetValue(SubredditNameProperty);
            set => SetValue(SubredditNameProperty, value);
        }

        public int VoteRatio
        {
            get => (int)GetValue(VoteRatioProperty);
            set => SetValue(VoteRatioProperty, value);
        }

        public bool ShowDescription
        {
            get => (bool)GetValue(ShowDescriptionProperty);
            set => SetValue(ShowDescriptionProperty, value);
        }

        public bool IsDownvoted
        {
            get => (bool)GetValue(IsDownvotedProperty);
            set => SetValue(IsDownvotedProperty, value);
        }

        public bool IsUpvoted
        {
            get => (bool)GetValue(IsUpvotedProperty);
            set => SetValue(IsUpvotedProperty, value);
        }

        public DateTime CreationDate
        {
            get => (DateTime)GetValue(CreationDateProperty);
            set => SetValue(CreationDateProperty, value);
        }

        public ICommand UserClickCommand
        {
            get => (ICommand)GetValue(UserClickCommandProperty);
            set => SetValue(UserClickCommandProperty, value);
        }

        public ICommand SubredditClickCommand
        {
            get => (ICommand)GetValue(SubredditClickCommandProperty);
            set => SetValue(SubredditClickCommandProperty, value);
        }

        public object FooterContent
        {
            get => GetValue(FooterContentProperty);
            set => SetValue(FooterContentProperty, value);
        }

        public PostItemControl()
        {
            InitializeComponent();
        }
    }

    public partial class PostItemControl
    {
        public static DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title), typeof(string), typeof(PostItemControl), new(string.Empty));

        public static DependencyProperty DescriptionProperty
            = DependencyProperty.Register(nameof(Description), typeof(string), typeof(PostItemControl), new(string.Empty));

        public static DependencyProperty UserNameProperty
            = DependencyProperty.Register(nameof(UserName), typeof(string), typeof(PostItemControl), new(string.Empty));

        public static DependencyProperty SubredditNameProperty
            = DependencyProperty.Register(nameof(SubredditName), typeof(string), typeof(PostItemControl), new(string.Empty));

        public static DependencyProperty ShowDescriptionProperty
            = DependencyProperty.Register(nameof(ShowDescription), typeof(bool), typeof(PostItemControl), new(true));

        public static DependencyProperty VoteRatioProperty
            = DependencyProperty.Register(nameof(VoteRatio), typeof(int), typeof(PostItemControl), new(0));

        public static DependencyProperty IsDownvotedProperty
            = DependencyProperty.Register(nameof(IsDownvoted), typeof(bool), typeof(PostItemControl), new(false));

        public static DependencyProperty IsUpvotedProperty
            = DependencyProperty.Register(nameof(IsUpvoted), typeof(bool), typeof(PostItemControl), new(false));

        public static DependencyProperty CreationDateProperty
            = DependencyProperty.Register(nameof(CreationDate), typeof(DateTime), typeof(PostItemControl), new(default(DateTime)));

        public static DependencyProperty UserClickCommandProperty
            = DependencyProperty.Register(nameof(UserClickCommand), typeof(ICommand), typeof(PostItemControl), new(null));

        public static DependencyProperty SubredditClickCommandProperty
            = DependencyProperty.Register(nameof(SubredditClickCommand), typeof(ICommand), typeof(PostItemControl), new(null));

        public static DependencyProperty FooterContentProperty
            = DependencyProperty.Register(nameof(FooterContent), typeof(object), typeof(PostItemControl), new(null));
    }

    public partial class PostItemControl
    {
        private void OnUserHyperlinkClick(Hyperlink _, HyperlinkClickEventArgs _1)
            => UserClickCommand?.Execute(UserName);

        private void OnSubredditHyperlinkClick(Hyperlink _, HyperlinkClickEventArgs _1)
            => SubredditClickCommand?.Execute(SubredditName);
    }
}
