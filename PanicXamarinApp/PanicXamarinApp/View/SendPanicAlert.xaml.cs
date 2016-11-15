using PanicXamarinApp.SQLite.SQLiteEntityLayer;
using PanicXamarinApp.ViewModel;
using System;
using Xamarin.Forms;


namespace PanicXamarinApp.View
{
    public partial class SendPanicAlert : ContentPage
    {
        #region Instance Variable
        SendPanicAlertViewModel _sendPanicAlertViewModel;
        #endregion

        #region SendPanicAlert
        public SendPanicAlert()
        {
            InitializeComponent();
            _sendPanicAlertViewModel = new SendPanicAlertViewModel(this);
            BindingContext = _sendPanicAlertViewModel;         
        }
        #endregion

        #region Events
        private void BtnCancel_Clicked(object sender, EventArgs e)
        {
            _sendPanicAlertViewModel.IsRequestCancel = true;
            Navigation.PopAsync();
        }
        #endregion
    }
}
