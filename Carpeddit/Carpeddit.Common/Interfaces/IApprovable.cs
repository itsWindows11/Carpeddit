using System.Threading.Tasks;

namespace Carpeddit.Common.Interfaces
{
    public interface IApprovable
    {
        bool Approved { get; set; }

        void Approve();

        Task ApproveAsync();
    }
}
