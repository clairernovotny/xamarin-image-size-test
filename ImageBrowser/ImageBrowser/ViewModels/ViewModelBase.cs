using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ImageBrowser.Data;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ImageBrowser.ViewModels
{
  

    public abstract class ViewModelBase : BindableBase
    {
       
        public abstract Task LoadItemsAsync(bool forceRefresh = false);

      
    }

    public abstract class ViewModelBase<T> : ViewModelBase where T : BindableSchemaBase
    {
        private const int PREVIEWITEMS_COUNT = 6;
        protected DataSourceBase<T> _dataSource;
        protected ObservableCollection<T> _items = new ObservableCollection<T>();
        protected ObservableCollection<T> _previewItems = new ObservableCollection<T>();
        protected T _selectedItem = null;

        public DataSourceBase<T> DataSource
        {
            get { return _dataSource ?? (_dataSource = CreateDataSource()); }
        }

  
        public virtual bool HasMoreItems
        {
            get { return _items != null ? _items.Count > PREVIEWITEMS_COUNT : false; }
        }

        public bool IsItemSelected
        {
            get { return SelectedItem != null; }
        }

        public ObservableCollection<T> Items
        {
            get { return _items; }
        }

        public ObservableCollection<T> PreviewItems
        {
            get
            {
                if (_items != null)
                {
                    _previewItems.AddRangeUnique(_items.Take(PREVIEWITEMS_COUNT));
                }
                return _previewItems;
            }
        }



        public abstract string PageTitle { get; }

        public ICommand RefreshCommand
        {
            get { return new DelegateCommand(async () => { await LoadItemsAsync(true); }); }
        }

        public T SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        public override async Task LoadItemsAsync(bool forceRefresh = false)
        {
        //    ProgressBarVisibility = true;

            var timeStamp = await DataSource.LoadDataAsync(Items, forceRefresh);

            OnPropertyChanged("PreviewItems");
            OnPropertyChanged("HasMoreItems");

          //  ProgressBarVisibility = false;
        }

        public void SelectItem(object item)
        {
            this.SelectedItem = item as T;
        }

        protected abstract DataSourceBase<T> CreateDataSource();

        protected T GetCurrentItem()
        {
            if (SelectedItem != null)
            {
                return SelectedItem;
            }
            if (Items != null && Items.Count > 0)
            {
                return Items[0];
            }
            return null;
        }

        protected void GoToSource(string linkProperty)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                var url = GetBindingValue(linkProperty);
                if (!String.IsNullOrEmpty(url) && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    Device.OpenUri(new Uri(url, UriKind.Absolute));
                }
            }
        }

        protected virtual void NavigateToSelectedItem()
        {
        }

        protected void PinToStart(string path, string titleToShare, string messageToShare, string imageToShare)
        {
            var currentItem = GetCurrentItem();
            if (currentItem != null)
            {
                if (String.IsNullOrEmpty(path))
                {
                    path = "MainPage";
                }
                // TODO: Not implemented
            }
        }

        protected Uri TryCreateUri(string uriString)
        {
            try
            {
                return new Uri(uriString, UriKind.Absolute);
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("ViewModelBase.TryCreateUri", ex);
            }
            return null;
        }

        private string GetBindingValue(string binding)
        {
            binding = binding ?? String.Empty;
            if (binding.StartsWith("{") && binding.EndsWith("}"))
            {
                var currentItem = GetCurrentItem();
                if (currentItem != null)
                {
                    var propertyName = binding.Substring(1, binding.Length - 2);
                    return currentItem.GetValue(propertyName);
                }
            }
            return binding;
        }
    }
}