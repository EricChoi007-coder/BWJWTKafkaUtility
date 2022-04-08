using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.CommonUtility.HttpPollyExtensions
{
    public static class PollyHttpClientServiceCollectionExtensions
    {
        public static IServiceCollection AddPollyHttpClient(this IServiceCollection services, string name, Action<PollyHttpClientOptions> action)
        {
            // 1、创建选项配置类
            PollyHttpClientOptions options = new PollyHttpClientOptions();
            action(options);

            // 1、自定义异常处理(用缓存处理)
            var fallbackResponse = new HttpResponseMessage
            {
                Content = new StringContent("System is Busy, Please try later"),// 内容，自定义内容
                StatusCode = HttpStatusCode.BadGateway // 504
            };

            services.AddHttpClient(name) // 请求连接复用  name->服务隔离； name -> name+uuid service instance 隔离
                .AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<ExecutionRejectedException>() // 捕获所有的Polly异常
                .FallbackAsync(fallbackResponse))
                .AddPolicyHandler(
                Policy<HttpResponseMessage>
                .Handle<Exception>().
                CircuitBreakerAsync(options.CircuitBreakerOpenFallCount, TimeSpan.FromSeconds(options.CircuitBreakerDownTime))) // 断路器
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(options.TimeoutTime)) // 超时
                .AddPolicyHandler(Policy<HttpResponseMessage>
              .Handle<Exception>()
              .RetryAsync(options.RetryCount))
                .AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(10, 100));// 资源隔离（保证每一个服务都是固定的线程）

            return services;
        }
    }
}
