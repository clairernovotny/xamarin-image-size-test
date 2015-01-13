using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImageBrowser.Data
{
    public abstract class DataSourceBase<T> where T : BindableSchemaBase
    {
        static readonly bool isListItem;
        static DataSourceBase()
        {
            // see if the T is an IListItem
            if (typeof(IListItem).GetTypeInfo()
                                 .IsAssignableFrom(typeof(T).GetTypeInfo()))
            {
                isListItem = true;
            }
        }


        private const int ContentExpirationHours = 2;
        

        protected abstract string CacheKey { get; }

        public abstract Task<IEnumerable<T>> LoadDataAsync();

        

        public abstract bool HasStaticData { get; }

        public async Task<DateTime> LoadDataAsync(ObservableCollection<T> viewItems, bool forceRefresh)
        {
            var timeStamp = DateTime.Now;

            if (HasStaticData)
            {
                viewItems.AddRangeUnique(await LoadDataAsync());

                // Update the index values
                if (isListItem)
                {
                    for (var i = 0; i < viewItems.Count; i++)
                    {
                        ((IListItem)viewItems[i]).Index = i;
                    }
                }
            }
            else
            {
                var dataInCache = await AppCache.GetItemsAsync<T>(CacheKey);
                if (dataInCache != null)
                {
                    timeStamp = dataInCache.TimeStamp;

                    // Update the index values
                    if (isListItem)
                    {
                        for (var i = 0; i < dataInCache.Items.Count; i++)
                        {
                            ((IListItem)dataInCache.Items[i]).Index = i;
                        }
                    }

                    viewItems.AddRangeUnique(dataInCache.Items);
                }

                if (NetworkInterface.GetIsNetworkAvailable() && DataNeedToBeUpdated(forceRefresh, dataInCache))
                {
                    var freshData = new DataSourceContent<T>()
                    {
                        TimeStamp = DateTime.Now,
                        Items = (await LoadDataAsync()).ToList()
                    };

                    timeStamp = freshData.TimeStamp;

                    // Update the index values
                    if (isListItem)
                    {
                        for (var i = 0; i < freshData.Items.Count; i++)
                        {
                            ((IListItem)freshData.Items[i]).Index = i;
                        }
                    }

                    viewItems.AddRangeUnique(freshData.Items);
                    
                    await AppCache.AddItemsAsync(CacheKey, freshData);
                }
            }
            return timeStamp;
        }

        private static bool DataNeedToBeUpdated(bool forceRefresh, DataSourceContent<T> dataInCache)
        {
            return dataInCache == null || forceRefresh || IsContentExpirated(dataInCache.TimeStamp);
        }

        private static bool IsContentExpirated(DateTime timeStamp)
        {
            return (DateTime.Now - timeStamp) > new TimeSpan(ContentExpirationHours, 0, 0);
        }
    }
}
