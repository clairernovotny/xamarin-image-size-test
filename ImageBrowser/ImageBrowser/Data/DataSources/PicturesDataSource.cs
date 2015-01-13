using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageBrowser.Data
{
    public class PicturesDataSource : DataSourceBase<RssSchema>
    {
        private const string Url = @"http://photos.novotny.org/hack/feed.mg?Type=nicknameRecentPhotos&Data=orennovotny&format=rss200&Size=XLarge";

        protected override string CacheKey
        {
            get { return "PicturesDataSource"; }
        }

        public override bool HasStaticData
        {
            get { return false; }
        }

        public async override Task<IEnumerable<RssSchema>> LoadDataAsync()
        {
            try
            {
                var rssDataProvider = new RssDataProvider(Url);
                return await rssDataProvider.Load();
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("PicturesDataSource.LoadData", ex.ToString());
                return new RssSchema[0];
            }
        }
    }
}
