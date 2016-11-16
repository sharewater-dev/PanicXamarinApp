using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class AccidentAssist : ContentPage
    {
        AccidentAssistViewModel _viewModel;
        public AccidentAssist()
        {
            InitializeComponent();
            _viewModel = new AccidentAssistViewModel();
        }
    }
}
