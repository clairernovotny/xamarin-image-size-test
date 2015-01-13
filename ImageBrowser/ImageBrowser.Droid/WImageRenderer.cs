using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Test;
using Android.Widget;
using ImageBrowser;
using ImageBrowser.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;


[assembly: ExportRenderer(typeof(WImage), typeof(WImageRenderer))]

namespace ImageBrowser.Droid
{
    internal class WImageRenderer : ViewRenderer<Image, ImageView>
    {
        private bool isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            isDisposed = true;
            BitmapDrawable bitmapDrawable;
            if (disposing && Control != null && (bitmapDrawable = Control.Drawable as BitmapDrawable) != null)
            {
                var bitmap = bitmapDrawable.Bitmap;
                if (bitmap != null)
                {
                    bitmap.Recycle();
                    bitmap.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
                SetNativeControl(new FormsImageView(Context));
            UpdateBitmap(e.OldElement);
            UpdateAspect();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                UpdateBitmap(null);
            }
            else
            {
                if (!(e.PropertyName == Image.AspectProperty.PropertyName))
                    return;
                UpdateAspect();
            }
        }

        private void UpdateAspect()
        {
            using (var scaleType = Element.Aspect.ToScaleType())
                Control.SetScaleType(scaleType);
        }

        private async void UpdateBitmap(Image previous = null)
        {
            var bitmap = (Bitmap)null;
            var source = Element.Source as UriImageSource;

            IVisualElementController controller = Element;

            if (previous == null || !Equals(previous.Source, source))
            {
                ((FormsImageView)Control).SkipInvalidate();
                Control.SetImageResource(17170445);
                if (source != null)
                {
                    // Decode based on requested eight
                    // Get the height of the source
                    var options = await GetBitmapOptionsOfImageAsync(source.Uri);

                    var decodePixelHeight = Element.HeightRequest > 0 ? (int)Element.HeightRequest : options.OutHeight;
                    var decodePixelWidth = Element.WidthRequest > 0 ? (int)Element.WidthRequest : options.OutWidth;

                    bitmap = await LoadScaledDownBitmapForDisplayAsync(source.Uri, options, decodePixelWidth, decodePixelHeight);

                }
            }

            if (Element != null && Equals(Element.Source, source) && !isDisposed)
            {
                Control.SetImageBitmap(bitmap);
                if (bitmap != null)
                    bitmap.Dispose();

                controller.NativeSizeChanged();
            }
        }

        private static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            float height = options.OutHeight;
            float width = options.OutWidth;
            double inSampleSize = 1D;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = (int)(height / 2);
                int halfWidth = (int)(width / 2);

                // Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
                while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return (int)inSampleSize;
        }


        async Task<BitmapFactory.Options> GetBitmapOptionsOfImageAsync(Uri uri)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            using (var stream = await GetStream(uri))
            {
                // The result will be null because InJustDecodeBounds == true.
                var result = await BitmapFactory.DecodeStreamAsync(stream, null, options);
            }
            
            return options;
        }

        private static async Task<Stream> GetStream(Uri uri)
        {
            using (var client = new HttpClient())
            {
                return await Task.Run(async () => await client.GetStreamAsync(uri));
            }
        }

        public async Task<Bitmap> LoadScaledDownBitmapForDisplayAsync(Uri uri, BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Calculate inSampleSize
            options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

            // Decode bitmap with inSampleSize set
            options.InJustDecodeBounds = false;


            using (var stream = await GetStream(uri))
            {
                
                return await BitmapFactory.DecodeStreamAsync(stream, null, options);
            }
        }
    }

    internal class FormsImageView : ImageView
    {
        private bool skipInvalidate;

        public FormsImageView(Context context)
            : base(context)
        {
        }

        public override void Invalidate()
        {
            if (skipInvalidate)
                skipInvalidate = false;
            else
                base.Invalidate();
        }

        public void SkipInvalidate()
        {
            skipInvalidate = true;
        }
    }

    internal static class ImageExtensions
    {
        public static ImageView.ScaleType ToScaleType(this Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.AspectFill:
                    return ImageView.ScaleType.CenterCrop;
                case Aspect.Fill:
                    return ImageView.ScaleType.FitXy;
                default:
                    return ImageView.ScaleType.FitCenter;
            }
        }
    }
}