using Carpeddit.App.Dialogs;
using System;
using Windows.UI.Xaml.Controls;

namespace Carpeddit.App
{
    public sealed partial class FirstRunPage : Page
    {
        public FirstRunPage()
        {
            InitializeComponent();

            Loaded += async (s, e) => await new FirstRunDialog().ShowAsync();
        }
    }
}
