using System.Threading.Tasks;

namespace Carpeddit.Common.Interfaces
{
    public interface IVotable
    {
        bool Upvoted { get; set; }
        
        bool Downvoted { get; set; }

        void Upvote();

        void Downvote();

        void Unvote();

        Task UpvoteAsync();

        Task DownvoteAsync();

        Task UnvoteAsync();
    }
}
