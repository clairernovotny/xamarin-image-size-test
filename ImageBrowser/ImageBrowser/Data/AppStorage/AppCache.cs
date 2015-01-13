using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageBrowser.Data
{
    public class AppCache
    {
        private static readonly Dictionary<string, string> MemoryCache = new Dictionary<string, string>();

        public static async Task AddItemsAsync<T>(string key, DataSourceContent<T> data) where T : BindableSchemaBase
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                await UserStorage.WriteText(key, json);
                if (MemoryCache.ContainsKey(key))
                {
                    MemoryCache[key] = json;
                }
                else
                {
                    MemoryCache.Add(key, json);
                }
            }
            catch (Exception ex)
            {
                AppLogs.WriteError("AppCache.AddItems", ex);
            }
        }

        public static async Task<DataSourceContent<T>> GetItemsAsync<T>(string key) where T : BindableSchemaBase
        {
            string json = null;
            if (MemoryCache.ContainsKey(key))
            {
                json = MemoryCache[key];
            }
            else
            {
                json = await UserStorage.ReadTextFromFile(key);
                MemoryCache[key] = json;
            }
            if (!String.IsNullOrEmpty(json))
            {
                try
                {
                    var records = JsonConvert.DeserializeObject<DataSourceContent<T>>(json);
                    return records;
                }
                catch (Exception ex)
                {
                    AppLogs.WriteError("AppCache.GetItems", ex);
                }
            }
            return null;
        }
    }
}