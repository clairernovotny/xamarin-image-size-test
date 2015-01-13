using System;
using System.Windows;
using System.Windows.Input;

using ImageBrowser.Data;
using Xamarin.Forms;

namespace ImageBrowser.ViewModels
{
    public class PicturesViewModel : ViewModelBase<RssSchema>
    {

        override protected DataSourceBase<RssSchema> CreateDataSource()
        {
            return new PicturesDataSource(); // RssDataSource
        }
        
        public override string PageTitle
        {
            get { return "Pictures"; }
        }

    }
}
