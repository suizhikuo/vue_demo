using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Hosting;

namespace Common
{
    public class CacheHelper
    {
        private static Cache _cache;
        public static double SaveTime
        {
            get;
            set;
        }
        static CacheHelper()
        {
            _cache = HostingEnvironment.Cache;
            SaveTime = 5;
        }

        /// <summary>
        /// 获取缓存中的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return _cache.Get(key);
        }

        /// <summary>
        /// 获取缓存中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            object obj = Get(key);
            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key">缓存Key值</param>
        /// <param name="value">缓存数据</param>
        /// <param name="dependency">依赖</param>
        /// <param name="priority">优先级</param>
        /// <param name="callback">缓存清除时回调</param>
        public static void Insert(string key, object value, CacheDependency dependency, CacheItemPriority priority, CacheItemRemovedCallback callback)
        {
            _cache.Insert(key, value, dependency, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(SaveTime), priority, callback);
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key">缓存Key值</param>
        /// <param name="value">缓存数据</param>
        /// <param name="dependency">依赖</param>
        /// <param name="callback">缓存清除时回调</param>
        public static void Insert(string key, object value, CacheDependency dependency, CacheItemRemovedCallback callback)
        {
            Insert(key, value, dependency, CacheItemPriority.Default, callback);
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key">缓存Key值</param>
        /// <param name="value">缓存数据</param>
        /// <param name="dependency">依赖</param>
        public static void Insert(string key, object value, CacheDependency dependency)
        {
            Insert(key, value, dependency, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 添加缓存数据
        /// </summary>
        /// <param name="key">缓存Key值</param>
        /// <param name="value">缓存数据</param>
        public static void Insert(string key, object value)
        {
            Insert(key, value, null, CacheItemPriority.Default, null);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">缓存Key值</param>
        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            _cache.Remove(key);
        }

        /// <summary>
        /// 获取全部缓存Key
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetKeys()
        {
            List<string> keys = new List<string>();
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }
            return keys.AsReadOnly();
        }

        /// <summary>
        /// 删除全部缓存
        /// </summary>
        public static void RemoveAll()
        {
            IList<string> keys = GetKeys();
            foreach (string key in keys)
            {
                _cache.Remove(key);
            }
        }
    }
}
