﻿using PanicXamarinApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class AccidentDetail : ContentPage
    {
        AccidentDetailViewModel _viewModel;
        public AccidentDetail()
        {
            InitializeComponent();
            _viewModel = new AccidentDetailViewModel();
            BindingContext = _viewModel;
        
         
        }
       
    }
}