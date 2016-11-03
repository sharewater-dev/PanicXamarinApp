using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using PanicXamarinApp.CustomControls;
using PanicXamarinApp.Droid.CustomControls;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

[assembly: ExportRenderer(typeof(NativeButton), typeof(NativeButtonRenderer))]
namespace PanicXamarinApp.Droid.CustomControls
{
    public class NativeButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (this.Control == null) return;
                var btn = Control;

                var fileName = e.NewElement?.StyleId;
                Typeface font = null;
            
                font = Typeface.CreateFromAsset(Forms.Context.Assets, "fontawesome-webfont.ttf");
                //}
                btn.Typeface = font;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
    }
}