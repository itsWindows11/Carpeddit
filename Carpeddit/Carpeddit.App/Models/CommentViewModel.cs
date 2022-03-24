using Reddit.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carpeddit.App.Models
{
    public class CommentViewModel
    {
        public Comment OriginalComment { get; set; }

        public ObservableCollection<CommentViewModel> Replies { get; set; } = new();

        public bool Expanded => !OriginalComment.Collapsed;
    }
}
