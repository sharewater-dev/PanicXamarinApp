using System;
using Xamarin.Forms;
using PanicXamarinApp.CustomControls;
using PanicXamarinApp.iOS.CustomControls;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;

[assembly: ExportRenderer(typeof(NativeButton), typeof(NativeButtonRenderer))]
namespace PanicXamarinApp.iOS.CustomControls
{
    public class NativeButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (this.Control == null) return;
                var btn = Control as UIButton;

                var fileName = e.NewElement?.StyleId;
                NSMutableAttributedString obj = new NSMutableAttributedString();
                //if (!string.IsNullOrEmpty(fileName))
                //{
                //    font = Typeface.CreateFromAsset(Forms.Context.Assets, fileName);
                //}
                //else
                //{
                obj.AddAttribute(new NSString("NSFont"), UIFont.FromName("fontawesome-webfont.ttf", 12), new NSRange(0, "".Length));
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

    }
}