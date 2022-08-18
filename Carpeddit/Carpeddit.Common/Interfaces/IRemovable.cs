using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.Common.Interfaces
{
    public interface IRemovable
    {
        bool Removed { get; set; }

        bool Spammed { get; set; }

        void Remove();

        void Spam();

        Task RemoveAsync();

        Task SpamAsync();
    }
}
