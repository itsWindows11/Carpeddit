using Reddit.Things;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Carpeddit.App.ViewModels
{
    public partial class ImageViewModel : ViewModel<Image>
    {
        public ImageViewModel(Image image)
        {
            Model ??= image;
        }

        private List<Image> _images;

        public List<Image> Images
        {
            get => _images;
            set => Set(ref _images, value);
        }
    }

    // Property change handling
    public partial class ImageViewModel : INotifyPropertyChanged
    {
        private readonly List<PropertyChangedEventHandler> _handlers = new();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _handlers.Add(value);
            }
            remove
            {
                _handlers.Remove(value);
            }
        }

        protected void OnPropertyChanged(object sender, [CallerMemberName] string name = "")
        {
            foreach (var handler in _handlers)
            {
                handler?.Invoke(sender, new(name));
            }
        }

        protected bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
