using System;
using System.Runtime.Caching;

namespace SampleProject.Utils
{
    public class MemoryCacheUtil
    {
        private static MemoryCache _cache = new MemoryCache("SampleCache");

        public static object GetItem(string key)
        {
            return AddOrGetExisting(key, () => InitItem(key));
        }

        // How to call this AddOrGetExisting method:
        // var value = MemoryCacheUtil.AddOrGetExisting(key, () => ActualMethodToGetValuesForCaching(parameters));
        
        public static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = _cache.AddOrGetExisting
                (key, newValue, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) }) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                _cache.Remove(key);
                throw;
            }
        }

        private static object InitItem(string key)
        {
            return new { Value = key.ToUpper() };
        }
    }
}
