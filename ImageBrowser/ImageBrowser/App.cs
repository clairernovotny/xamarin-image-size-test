using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageBrowser.Converters;
using ImageBrowser.ViewModels;
using ImageBrowser.Views;
using Xamarin.Forms;

namespace ImageBrowser
{
    public class App : Application
    {
        PicturesViewModel viewModel;
        public App()
        {
            viewModel = new PicturesViewModel();


            Resources = new ResourceDictionary();
            Resources.Add("StringToUriConverter", new StringToUriConverter());

            // This is the default behavior in Xamarin Forms
           // MainPage = new ImageList() { BindingContext = viewModel };
            
            
            // This uses the optimized WP8 renderer
            //MainPage = new ImageListOptimized { BindingContext = viewModel };
        }

        protected override async void OnStart()
        {
            // Handle when your app starts
            await viewModel.LoadItemsAsync();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
