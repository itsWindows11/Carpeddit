using System.Runtime.Serialization;

namespace Carpeddit.Api.Enums
{
    public enum SortMode
    {
        [EnumMember(Value = "new")]
        New,
        [EnumMember(Value = "best")]
        Best,
        [EnumMember(Value = "controversial")]
        Controversial,
        [EnumMember(Value = "hot")]
        Hot,
        [EnumMember(Value = "rising")]
        Rising,
        [EnumMember(Value = "top")]
        Top,
        // Comments only
        [EnumMember(Value = "random")]
        Random,
        [EnumMember(Value = "old")]
        Old,
        [EnumMember(Value = "qa")]
        QA
    }
}
