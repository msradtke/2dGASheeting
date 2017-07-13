using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using _2dGASheeting.Models;
using _2dGASheeting.Services;
using System.Collections.ObjectModel;
namespace _2dGASheeting.ViewModels
{
    public class TestRectViewModel
    {

        public TestRectViewModel()
        {
            _2dGeneticAlg alg = new _2dGeneticAlg();
            alg.UseSampleData();
            var solutions = alg.Process();
            SolutionContainerViewModel = new SolutionContainerViewModel();
            //BottomLeftBestFitHeuristic test = new BottomLeftBestFitHeuristic();
            //var pattern = test.Process();
            var master = solutions.First().Patterns.First().Master;
            var solsVm = new ObservableCollection<SolutionViewModel>();
            foreach (var solution in solutions)
            {
                var solVm = new SolutionViewModel();
                solVm.PatternCount = solution.Patterns.Count();
                solVm.MasterCount = solution.PatternDemands.Sum(x => x.Demand);
                var canvasVms = new List<CanvasViewModel>();
                foreach (var pd in solution.PatternDemands)
                {
                    var service = new DrawPattern2dService(pd.Pattern, master, 5);

                    var canvas = service.GetCanvas();
                    
                    canvasVms.Add(new CanvasViewModel { Canvas = canvas, Count = pd.Demand });
                                        
                }
                solVm.CanvasViewModels = canvasVms;
                solsVm.Add(solVm);
            }
            SolutionContainerViewModel.SolutionViewModels = solsVm;
        }
        //public Canvas Canvas { get; set; }
        public List<Canvas> Canvas { get; set; }
        //public List<CanvasViewModel> CanvasViewModels { get; set; }
        public SolutionContainerViewModel SolutionContainerViewModel { get; set; }
    }

    public class CanvasViewModel
    {
        public int Count { get; set; }
        public Canvas Canvas { get; set; }
    }

}
