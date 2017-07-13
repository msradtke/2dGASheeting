using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2dGASheeting.ViewModels
{
    public class SolutionViewModel
    {
        public List<CanvasViewModel> CanvasViewModels { get; set; }
        public int PatternCount { get; set; }
        public int MasterCount { get; set; }
    }
}
