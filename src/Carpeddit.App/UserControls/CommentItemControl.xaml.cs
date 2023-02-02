using Carpeddit.App.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App.UserControls
{
    public sealed partial class CommentItemControl : UserControl
    {
        public static DependencyProperty ModelProperty
            = DependencyProperty.Register(nameof(Model), typeof(CommentViewModel), typeof(CommentItemControl), new(null));

        public static DependencyProperty FooterContentProperty
            = DependencyProperty.Register(nameof(FooterContent), typeof(object), typeof(CommentItemControl), new(null));

        public CommentViewModel Model
        {
            get => (CommentViewModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public object FooterContent
        {
            get => GetValue(FooterContentProperty);
            set => SetValue(FooterContentProperty, value);
        }

        public CommentItemControl()
        {
            InitializeComponent();
        }
    }
}
