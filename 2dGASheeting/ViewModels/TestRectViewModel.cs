using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using _2dGASheeting.Models;
using _2dGASheeting.Services;
namespace _2dGASheeting.ViewModels
{
    public class TestRectViewModel
    {

        public TestRectViewModel()
        {
            _2dGeneticAlg alg = new _2dGeneticAlg();
            alg.UseSampleData();
            var list = alg.CreateInitialSolutions();


            //BottomLeftBestFitHeuristic test = new BottomLeftBestFitHeuristic();
            //var pattern = test.Process();
            var master = list.First().Pattern.Master;
            var service = new DrawPattern2dService(list,master,5);
            Canvas = service.GetCanvas();
            
        }
        //public Canvas Canvas { get; set; }
        public List<Canvas> Canvas { get; set; }

    }
}
