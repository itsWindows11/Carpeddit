namespace Carpeddit.Api.Models
{
    public interface IVotable
    {
        int Ups { get; }

        int Downs { get; }

        bool? Likes { get; }
    }
}
