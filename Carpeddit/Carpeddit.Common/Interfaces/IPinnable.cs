using System.Threading.Tasks;

namespace Carpeddit.Common.Interfaces
{
    public interface IPinnable
    {
        bool Pinned { get; set; }

        void Pin();

        void Unpin();

        Task PinAsync();

        Task UnpinAsync();
    }
}
