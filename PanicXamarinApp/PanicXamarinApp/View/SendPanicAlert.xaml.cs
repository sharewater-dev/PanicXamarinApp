using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class SendPanicAlert : ContentPage
    {

        public SendPanicAlert()
        {
            InitializeComponent();
            SendPanicAlertViewModel _sendPanicAlertViewModel = new SendPanicAlertViewModel(this);
            BindingContext = _sendPanicAlertViewModel;
            Utility _utility = new Utility();
            _utility.CreateDatabase();
        }

        #region Events
        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        #endregion
    }
}
