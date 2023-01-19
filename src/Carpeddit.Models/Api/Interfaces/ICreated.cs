using System;

namespace Carpeddit.Models.Api
{
    public interface ICreated
    {
        DateTime Created { get; }

        DateTime CreatedUtc { get; }
    }
}
