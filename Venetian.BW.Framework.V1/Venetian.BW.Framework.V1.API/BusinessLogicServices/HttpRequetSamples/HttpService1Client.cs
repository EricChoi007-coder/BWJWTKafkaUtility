using System.Collections.Generic;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.CommonUtility.HttpPollyExtensions;
using Venetian.BW.Framework.V1.Domain.HttpClientRequestModels.Service1s;

namespace Venetian.BW.Framework.V1.API.BusinessLogicServices.HttpRequetSamples
{
    public class HttpService1Client : IService1Client
    {
        private readonly string ServiceBasicUrl = "https://bwservice.com"; //Service Basic URL
        private readonly string ServiceName = "Service1"; //Service Name 
        private readonly string ServiceLink = "/Service1"; //Service Link

        // httpclient bw customize instance
        private readonly BwHttpClient bwHttpClient;
        public HttpService1Client(BwHttpClient  bwHttpClient)
        {
            this.bwHttpClient = bwHttpClient;
        }
        public async Task<List<Service1>> GetService1List()
        {
            List<Service1> service1List = await bwHttpClient.GetAsync<List<Service1>>(ServiceBasicUrl, ServiceName, ServiceLink);
            return service1List;
        }
    }
}
