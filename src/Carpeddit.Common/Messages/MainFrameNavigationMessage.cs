using System;
using Windows.UI.Xaml.Media.Animation;

namespace Carpeddit.Common.Messages
{
    public sealed class MainFrameNavigationMessage
    {
        public Type Page { get; set; }
        
        public object Parameter { get; set; }

        public NavigationTransitionInfo TransitionInfo { get; set; } = new EntranceNavigationTransitionInfo();
    }
}
