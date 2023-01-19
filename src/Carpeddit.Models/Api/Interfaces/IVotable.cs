namespace Carpeddit.Models.Api
{
    public interface IVotable
    {
        int Ups { get; }

        int Downs { get; }

        bool? Likes { get; }
    }
}
