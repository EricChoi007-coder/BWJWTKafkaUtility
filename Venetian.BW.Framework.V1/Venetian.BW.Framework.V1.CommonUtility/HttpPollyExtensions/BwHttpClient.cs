using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.CommonUtility.HttpPollyExtensions
{
    public class BwHttpClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        public BwHttpClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Get方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceBasicUrl">服务名称:(http/https://www.service.com)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string ServiceBasicUrl, string ServiceName, string serviceLink)
        {
            // 故障转移
            string json = "";
            int RestyConut = 0;
            for (int i = 0; i <= 3; i++)
            {
                // 1、是否达到阀值 3
                if (RestyConut == 3)
                {
                    throw new Exception($"Exceed Service Request Retry Max Times");
                }             

                try
                {
                    // 3、建立请求
                    HttpClient httpClient = httpClientFactory.CreateClient(ServiceName);
                    HttpResponseMessage response = await httpClient.GetAsync(ServiceBasicUrl + serviceLink);

                    // 3.1、json -> Object
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        json = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    else
                    {
                        throw new Exception($"{ServiceName}服务调用错误，异常{await response.Content.ReadAsStringAsync()}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Exception {e.Message}");
                    // 存储到集合
                    ++RestyConut;
                    Console.WriteLine($"Service{ServiceName}Error，Begin to Restry to Upper Service, Count:{RestyConut}");
                }
            }

            return JsonConvert.DeserializeObject<T>(json);
        }


        /// <summary>
        /// Post方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// param name="ServiceBasicUrl">服务名称:(http/https://www.service.com)</param>
        /// <param name="ServiceName">服务名称</param>
        /// <param name="serviceLink">服务路径</param>
        /// <param name="paramData">服务参数</param>
        /// <returns></returns>
        public T PostAsync<T>(string ServiceBasicUrl, string ServiceName, string serviceLink, object paramData = null)
        {
            
            // 3、建立请求
            // Console.WriteLine($"请求路径：{ServiceBasicUrl} + {serviceLink}");
            HttpClient httpClient = httpClientFactory.CreateClient(ServiceName);

            // 3.1 转换成json内容
            HttpContent hc = new StringContent(JsonConvert.SerializeObject(paramData), Encoding.UTF8, "application/json");

            // HttpResponseMessage response = await httpClient.GetAsync(serviceUrl.Url + serviceLink);
            HttpResponseMessage response = httpClient.PostAsync(ServiceBasicUrl + serviceLink, hc).Result;

            // 3.1json => object
            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
            {
                string json = response.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<T>(json);
            }
            else
            {
                // 3.2、进行自定义异常处理，进行了降级处理
                throw new Exception($"{ServiceName}service error:{response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
