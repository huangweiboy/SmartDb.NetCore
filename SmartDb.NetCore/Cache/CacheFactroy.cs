using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDb.NetCore
{
    public class CacheFactroy
    {
        private static CacheFactroy instance;
        private static readonly object objLock = new object();
        private static Dictionary<string, object> cacheDictionary;

        private CacheFactroy() { }

        public static CacheFactroy GetInstance()
        {
            if (instance == null)
            {
                lock (objLock)
                {
                    if (instance == null)
                    {
                        instance = new CacheFactroy();
                        cacheDictionary = new Dictionary<string, object>();
                    }
                }
            }
            return instance;
        }

        public void SetCache(string cacheKey, object cacheValue)
        {
            var item = cacheDictionary.FirstOrDefault(a=>a.Key.Equals(cacheKey));
            if (!string.IsNullOrEmpty(item.Key)|| cacheValue==null)
            {
                return;
            }
            cacheDictionary.Add(cacheKey, cacheValue);
        }

        public object GetCache(string cacheKey)
        {
            object cacheValue=null;
            var item = cacheDictionary.FirstOrDefault(a => a.Key.Equals(cacheKey));
            if (string.IsNullOrEmpty(item.Key))
            {
                return cacheValue;
            }
            cacheValue = item.Value;
            return cacheValue;
        }

        public void RemoveCache(string cacheKey)
        {
            object cacheValue = GetCache(cacheKey);
            if (cacheValue==null)
            {
                return;
            }
            cacheDictionary.Remove(cacheKey);
        }
    }
}
