using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.Domain.Testings
{
    public interface ITestingRepository
    {
        Task<List<Testing>> GetTestingList();
    }
}
