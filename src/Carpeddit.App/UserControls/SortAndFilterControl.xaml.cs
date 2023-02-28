using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Carpeddit.App.UserControls
{
    public sealed partial class SortAndFilterControl : UserControl
    {
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public object SelectedContent
        {
            get => GetValue(SelectedContentProperty);
            set => SetValue(SelectedContentProperty, value);
        }

        public FlyoutBase Flyout
        {
            get => (FlyoutBase)GetValue(FlyoutProperty);
            set
            {
                SetValue(FlyoutProperty, value);
                RegisterRadioFlyouts(value);
            }
        }

        public ICommand SelectionChangedCommand
        {
            get => (ICommand)GetValue(SelectionChangedCommandProperty);
            set => SetValue(SelectionChangedCommandProperty, value);
        }

        public SortAndFilterControl()
        {
            InitializeComponent();
        }

        private void RegisterRadioFlyouts(FlyoutBase flyout)
        {
            if (flyout is not MenuFlyout menu)
                return;

            FindRadioFlyouts(menu.Items, (radioItem, subItemText) =>
            {
                if (!string.IsNullOrWhiteSpace(subItemText))
                    radioItem.Tag = $"{subItemText} ({radioItem.Text})";
                else
                    radioItem.Tag = radioItem.Text;

                radioItem.Click += OnRadioItemClick;
            });
        }

        private void FindRadioFlyouts(IList<MenuFlyoutItemBase> items, Action<RadioMenuFlyoutItem, string> action)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case RadioMenuFlyoutItem radioItem:
                        action(radioItem, string.Empty);
                        break;
                    case MenuFlyoutSubItem subItems:
                        foreach (var subItem in subItems.Items)
                        {
                            if (subItem is RadioMenuFlyoutItem item1)
                                action(item1, subItems.Text);
                        }
                        break;
                    default:
                        continue;
                }
            }
        }

        private void OnRadioItemClick(object sender, RoutedEventArgs e)
        {
            var item = (RadioMenuFlyoutItem)sender;
            SelectedContent = item.Tag;

            SelectionChangedCommand?.Execute(item.Tag);
        }
    }

    public partial class SortAndFilterControl
    {
        public static DependencyProperty LabelProperty
            = DependencyProperty.Register(nameof(Label), typeof(string), typeof(SortAndFilterControl), new(string.Empty));

        public static DependencyProperty SelectedContentProperty
            = DependencyProperty.Register(nameof(SelectedContent), typeof(object), typeof(SortAndFilterControl), new(null));

        public static DependencyProperty FlyoutProperty
            = DependencyProperty.Register(nameof(Flyout), typeof(FlyoutBase), typeof(SortAndFilterControl), new(null));

        public static DependencyProperty SelectionChangedCommandProperty
            = DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(ICommand), typeof(SortAndFilterControl), new(null));
    }
}
