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
        #region Instance Variable
        RegisterProfileViewModel _parentViewModel;
        AddEmergencyContactViewModel _viewModel;
        Dictionary<string, string> _salutation = new Dictionary<string, string>
        {
            { "SELECT","SELECT" },
            { "MR","MR" },
            { "MRS", "MRS" },
            { "Miss", "Miss" },
        };

        Dictionary<string, string> _relationShip = new Dictionary<string, string>
        {
            { "SELECT","SELECT" },
            { "Father","Father" },
            { "Mother", "Mother" },
            { "Son", "Son" },
            { "Daughter","Daughter" },
            { "Elder Brother", "Elder Brother" },
            { "Younger Brother", "Younger Brother" },
            { "Elder Sister","Elder Sister" },
            { "Younger Sister", "Younger Sister" },
            { "Grand Father", "Grand Father" },
            { "Grand Mother", "Grand Mother" }
        };
        #endregion

        #region AddEmergencyContact
        public AddEmergencyContact(RegisterProfileViewModel parentViewModel)
        {
            try
            {
                InitializeComponent();
                _parentViewModel = parentViewModel;
                _viewModel = new AddEmergencyContactViewModel(this);
                _viewModel._emergencyContactsList = parentViewModel._EmergencyContactsList;
                _viewModel.LoadData();
                foreach (string key in _salutation.Keys)
                {
                    ddlSaturation.Items.Add(key);
                }
                ddlSaturation.SelectedIndexChanged += DdlSaturation_SelectedIndexChanged;

                foreach (string key in _relationShip.Keys)
                {
                    ddlRelationShip.Items.Add(key);
                }
                ddlRelationShip.SelectedIndexChanged += DdlRelationShip_SelectedIndexChanged;

                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {

            }       
        }
        #endregion

        #region Events
        private void DdlRelationShip_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewModel.RelationShip = ddlRelationShip.Items[ddlRelationShip.SelectedIndex];
        }

        private void DdlSaturation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _viewModel.Salutation = ddlSaturation.Items[ddlSaturation.SelectedIndex];
        }
        #endregion
    }
}
