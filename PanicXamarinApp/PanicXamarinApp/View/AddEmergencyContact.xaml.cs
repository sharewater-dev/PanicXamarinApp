using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class AddEmergencyContact : ContentPage
    {
        AddEmergencyContactViewModel viewModel;
        public AddEmergencyContact()
        {
            InitializeComponent();
            viewModel = new AddEmergencyContactViewModel(this);
            BindingContext = viewModel;
        }
    }
}
