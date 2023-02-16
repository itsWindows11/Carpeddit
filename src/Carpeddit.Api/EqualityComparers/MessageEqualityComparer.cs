using Carpeddit.Api.Models;
using System.Collections.Generic;

namespace Carpeddit.Api.EqualityComparers
{
    public sealed class MessageEqualityComparer : EqualityComparer<Message>
    {
        public override bool Equals(Message x, Message y)
            => x.Name == y.Name;

        public override int GetHashCode(Message obj)
            => obj.GetHashCode();
    }
}
