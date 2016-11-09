using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.ViewModel;
using System;
using Xamarin.Forms;


namespace PanicXamarinApp.View
{
    public partial class SendPanicAlert : ContentPage
    {
        SendPanicAlertViewModel _sendPanicAlertViewModel;
        public SendPanicAlert()
        {
            InitializeComponent();
            _sendPanicAlertViewModel = new SendPanicAlertViewModel(this);
            BindingContext = _sendPanicAlertViewModel;
            Utility _utility = new Utility();
            _utility.CreateDatabase();

        }

        #region Events
        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            _sendPanicAlertViewModel.IsRequestCancel = true;
            Navigation.PopAsync();
        }
        #endregion
    }
}
