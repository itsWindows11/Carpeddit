using System;

namespace Carpeddit.Api.Models
{
    public interface ICreated
    {
        DateTime Created { get; }

        DateTime CreatedUtc { get; }
    }
}
