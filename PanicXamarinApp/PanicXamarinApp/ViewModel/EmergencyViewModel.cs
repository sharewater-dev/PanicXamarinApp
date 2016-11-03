using MvvmHelpers;
using PanicXamarinApp.EntityModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanicXamarinApp.ViewModel
{
    public class EmergencyViewModel
    {    
        public ObservableCollection<Emergency> Emergency { get; set; }
        public EmergencyViewModel()
        {
            Emergency = new ObservableCollection<EntityModel.Emergency>();
            Emergency.Add(new Emergency {ImageUrl= "Home_Emergency.png", Name= "Home Emergency" });
            Emergency.Add(new Emergency { ImageUrl = "Home_Emergency.png", Name = "Medical Emergency" });
            Emergency.Add(new Emergency { ImageUrl = "Home_Emergency.png", Name = "Car Emergency" });
        }
    }
}
