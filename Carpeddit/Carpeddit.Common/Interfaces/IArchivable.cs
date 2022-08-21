using System.Threading.Tasks;

namespace Carpeddit.Common.Interfaces
{
    public interface IArchivable
    {
        bool Archived { get; }
        
        bool Locked { get; }

        void Archive();

        void Unarchive();

        Task ArchiveAsync();

        Task UnarchiveAsync();
    }
}
