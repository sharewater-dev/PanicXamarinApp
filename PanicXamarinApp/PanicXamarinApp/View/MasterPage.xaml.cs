using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PanicXamarinApp.View
{
    public partial class MasterPage : ContentPage
    {        
        public MasterPage()
        {
            InitializeComponent();

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "FLASHLIGHT",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "MESSAGES",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "FAQ's",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "T&C's",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "CONTACT THE AA",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)

            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "RATE OUR SERVICE",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "ABOUT APP",
                IconSource = "Messages.png",
                TargetType = typeof(IndexPage)
            });

            listView.ItemsSource = masterPageItems;
        }
    }
}
