using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.Domain.Services
{
    public interface IUnitOfWork: IDisposable
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
