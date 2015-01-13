using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageBrowser;
using ImageBrowser.WinPhone.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;
using Size = Xamarin.Forms.Size;

[assembly: ExportRenderer(typeof(WImage), typeof(WImageRenderer))]

namespace ImageBrowser.WinPhone.Renderers
{
    class WImageRenderer : ViewRenderer<Xamarin.Forms.Image, System.Windows.Controls.Image>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            var image = new System.Windows.Controls.Image();
            image.Unloaded += ImageOnUnloaded;
            SetAspect(image);
            SetSource(image);
            SetNativeControl(image);
        }

        private void ImageOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Control.Unloaded -= ImageOnUnloaded;
            var img = Control.Source as BitmapImage;
            if (img != null)
                DisposeImage(img);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                SetSource(Control);
            }
            else
            {
                if (!(e.PropertyName == Xamarin.Forms.Image.AspectProperty.PropertyName))
                    return;
                SetAspect(Control);
            }
        }

        private void SetSource(System.Windows.Controls.Image image)
        {
            var src = Element.Source as UriImageSource;
            IVisualElementController controller = Element;

            // see if there's ane existing bitmap source
            var oldSource = image.Source as BitmapImage;

            if (src != null && src.Uri != null)
            {
                var img = new BitmapImage(src.Uri)
                {
                    CreateOptions = BitmapCreateOptions.BackgroundCreation
                };
                if (Element.HeightRequest > 0)
                {
                    img.DecodePixelHeight = (int)Element.HeightRequest;
                }
                if (Element.WidthRequest > 0)
                {
                    img.DecodePixelWidth = (int)Element.WidthRequest;
                }

                image.Source = img;
                controller.NativeSizeChanged();
            }
            else
            {
                image.Source = null;
            }

            if (oldSource != null)
            {
                DisposeImage(oldSource);
            }
        }

        private void SetAspect(System.Windows.Controls.Image image)
        {
            var aspect = this.Element.Aspect;
            image.Stretch = ToStretch(aspect);
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            if (Control.Source == null)
                return new SizeRequest();
            return new SizeRequest(new Size()
            {
                Width = ((BitmapSource)Control.Source).PixelWidth,
                Height = ((BitmapSource)Control.Source).PixelHeight
            });
        }

        private static Stretch ToStretch(Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.AspectFill:
                    return Stretch.UniformToFill;
                case Aspect.Fill:
                    return Stretch.Fill;
                default:
                    return Stretch.Uniform;
            }
        }

        /// <summary>
        /// Force a <see cref="BitmapImage"/> to free up the image resources.
        /// This is a "hack" used because setting a null Source URI does not feee-up
        /// the bitmap raw data memory.
        /// 
        /// http://stackoverflow.com/questions/13816569/windows-phone-listbox-with-images-out-of-memory
        /// </summary>
        /// <param name="image"></param>
        private static void DisposeImage(BitmapImage image)
        {
            var uri = new Uri("ImageBrowser.WinPhone;component/Assets/11.png", UriKind.RelativeOrAbsolute);
            var sr = System.Windows.Application.GetResourceStream(uri);
            try
            {
                using (var stream = sr.Stream)
                {
                    image.DecodePixelWidth = 1; // This is essential!
                    image.DecodePixelHeight = 1; // This is essential!
                    image.SetSource(stream);
                }
            }
            catch
            { }
        }

    }
}

