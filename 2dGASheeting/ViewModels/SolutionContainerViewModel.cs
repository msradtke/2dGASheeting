using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace _2dGASheeting.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class SolutionContainerViewModel : WorkspaceViewModel
    {
        public SolutionContainerViewModel()
        {
            
            SolutionViewModels = new ObservableCollection<SolutionViewModel>();
            BackCommand = new ActionCommand(Back);
            ForwardCommand = new ActionCommand(Forward);
            //SelectedSolutionNum = 1;
        }
        public ICommand BackCommand { get; set; }
        public ICommand ForwardCommand { get; set; }

        void Back()
        {
            if (SelectedSolutionNum > 1)
                SelectedSolutionNum--;
        }
        void Forward()
        {
            if(SelectedSolutionNum < SolutionCount)
                SelectedSolutionNum++;
        }
        public SolutionViewModel SelectedSolution { get; set; }

        ObservableCollection<SolutionViewModel> _solutionViewModels;
        public ObservableCollection<SolutionViewModel> SolutionViewModels
        {
            get {return _solutionViewModels; }
            set
            {
                if(_solutionViewModels != value)
                {
                    _solutionViewModels = value;
                    SolutionCount = _solutionViewModels.Count();
                    SelectedSolutionNum = 1;
                }
            }
        }
        
        public int SolutionCount { get; set; }

        int _selectedSolutionNum;
        public int SelectedSolutionNum
        {
            get { return _selectedSolutionNum; }
            set
            {
                if (SolutionViewModels != null)
                {

                    if (value > 0 && value <= SolutionViewModels.Count())
                    {
                        _selectedSolutionNum = value;
                        SelectedSolution = SolutionViewModels[_selectedSolutionNum - 1];
                    }
                    else
                    {
                        SelectedSolutionNum = _selectedSolutionNum;
                        if (SolutionViewModels.Count() > 0)
                            SelectedSolution = SolutionViewModels[0];
                    }
                }
            }
        }
    }
}
