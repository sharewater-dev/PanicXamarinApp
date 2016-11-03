using PanicXamarinApp.EntityModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.ViewModel
{
    public class IndexPageViewModel : BaseNavigationViewModel
    {
        public ObservableCollection<Emergency> Emergencys { get; set; }
        public IndexPageViewModel()
        {
            Emergencys = new ObservableCollection<EntityModel.Emergency>();
            Emergencys.Add(new Emergency { ImageUrl = "Home_Emergency.png", Name = "Home Emergency" });
            Emergencys.Add(new Emergency { ImageUrl = "Medical_Emergency.png", Name = "Medical Emergency" });
            Emergencys.Add(new Emergency { ImageUrl = "Car_Jacking.png", Name = "Car Emergency" });

        }
    }
}
