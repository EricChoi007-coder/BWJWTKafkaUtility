using System.Collections.Generic;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.Domain.HttpClientRequestModels.Service1s;

namespace Venetian.BW.Framework.V1.API.BusinessLogicServices.HttpRequetSamples
{
    public interface IService1Client
    {
        Task<List<Service1>> GetService1List();
    }
}
