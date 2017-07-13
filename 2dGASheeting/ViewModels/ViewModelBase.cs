using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
using Prism.Events;
using System.ComponentModel;
namespace _2dGASheeting.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ViewModelBase :  INotifyPropertyChanged
    {
        List<object> _handlers; //keep reference to handlers alive
        public ViewModelBase()
        {
            _handlers = new List<object>();
        }
        /// <summary>
        /// Alerts a listener to change view based on event of other viewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="FactoryParameter">Parameter to pass to the viewmodel factory</param>
        public delegate void ChangeViewEventHandler(object sender, object FactoryParameter = null);

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void AddTabItem(WorkspaceViewModel viewModel)
        {

        }
        
        public IEventAggregator EventAggregator { get; set; }
        
    }
}
