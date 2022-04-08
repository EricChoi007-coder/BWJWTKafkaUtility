using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.CommonUtility.CustomizeCacheUtility
{
    public class CustomCache
    {
        /// <summary>
        /// private 保护数据   
        /// static  全局唯一  不释放
        /// Dictionary  保存多项数据
        /// </summary>
        private static Dictionary<string, KeyValuePair<object, DateTime>> CustomCacheDictionary;

        //private static System.Collections.Concurrent.ConcurrentDictionary
        /// <summary>
        /// 主动清理：只要是过期，最多超过10分钟，一定会被清理
        /// </summary>
        static CustomCache()
        {
            CustomCacheDictionary = new Dictionary<string, KeyValuePair<object, DateTime>>();
            Console.WriteLine($"{DateTime.Now.ToString("MM-dd HH:mm:ss fff")}初始化缓存");
            //缓存依托内存，系统重启后，缓存会重启，日志
            Task.Run(() =>
            {
                while (true)
                {
                    LockAction(new Action(() =>
                    {
                        List<string> list = new List<string>();
                        foreach (var key in CustomCacheDictionary.Keys)
                        {
                            var valueTime = CustomCacheDictionary[key];
                            if (valueTime.Value > DateTime.Now)//没有过期
                            {
                                //没过期
                            }
                            else
                            {
                                list.Add(key);
                            }
                        }
                        list.ForEach(key => CustomCacheDictionary.Remove(key));
                    }));
                    Thread.Sleep(1000 * 60 * 10);//10分钟来一遍  CPU影响很小
                }
            });
        }

        /// <summary>
        /// 添加数据  key重复会异常的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(string key, object value, int second = 1800)
        {
            LockAction(new Action(() =>
            {
                CustomCacheDictionary.Add(key, new KeyValuePair<object, DateTime>(value, DateTime.Now.AddSeconds(second)));
            }));
        }

        private static readonly object CustomCache_Lock = new object();
        private static void LockAction(Action action)
        {
            lock (CustomCache_Lock)
            {
                action.Invoke();
            }
        }


        /// <summary>
        /// 保存数据，有就覆盖 没有就新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SaveOrUpdate(string key, object value, int second = 1800)
        {
            LockAction(new Action(() =>
            {
                CustomCacheDictionary[key] = new KeyValuePair<object, DateTime>(value, DateTime.Now.AddSeconds(second));
            }));
        }

        /// <summary>
        /// 获取数据 没有会异常的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            return (T)CustomCacheDictionary[key].Key;
        }

        /// <summary>
        /// 检查是否存在
        /// 
        /// 清理一下，除非我们去访问这条缓存，才会去清理  被动清理，任何过期的数据，都不可以被查到
        /// 可能有垃圾留在缓存里面
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Exsit(string key)
        {
            if (CustomCacheDictionary.ContainsKey(key))
            {
                var valueTime = CustomCacheDictionary[key];
                if (valueTime.Value > DateTime.Now)//没有过期
                {
                    return true;
                }
                else
                {
                    LockAction(new Action(() =>
                    {
                        CustomCacheDictionary.Remove(key);//清理一下
                    }));
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static void Remove(string key)
        {
            LockAction(new Action(() =>
            {
                CustomCacheDictionary.Remove(key);
            }));
        }

        public static void RemoveAll()
        {
            LockAction(new Action(() =>
            {
                CustomCacheDictionary.Clear();
            }));
        }

        public static void RemoveCondition(Func<string, bool> func)
        {
            LockAction(new Action(() =>
            {
                List<string> list = new List<string>();
                foreach (var key in CustomCacheDictionary.Keys)
                {
                    if (func.Invoke(key))
                    {
                        list.Add(key);
                    }
                }
                list.ForEach(key => CustomCacheDictionary.Remove(key));
            }));
        }





        public static T Find<T>(string key, Func<T> func, int second = 1800)
        {
            T t = default(T);
            if (!Exsit(key))
            {
                t = func.Invoke();
                CustomCache.Add(key, t, second);
            }
            else
            {
                t = Get<T>(key);
            }
            return t;
        }
    }
}
