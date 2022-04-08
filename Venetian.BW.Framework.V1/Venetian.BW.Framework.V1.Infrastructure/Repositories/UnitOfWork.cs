using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.Domain.Services;
using Venetian.BW.Framework.V1.Domain.Testings;

namespace Venetian.BW.Framework.V1.Infrastructure.Repositories
{
    public class UnitOfWork:IUnitOfWork, IDisposable
    {
        private bool disposedValue;
        private SqlConnection _db;
        private SqlTransaction _trans;

        //Business Repository DI
        public ITestingRepository TestingRepository { get; set; }


        public UnitOfWork(string connectionString)
        {
            _db = new SqlConnection(connectionString);
            while (_db.State != ConnectionState.Open)
            {
                try
                {
                    _db.Open();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
        public void BeginTransaction()
        {
            IsolationLevel level = IsolationLevel.ReadCommitted;
            _trans = _db.BeginTransaction(level);

            //initial the repository of all modules
            TestingRepository = new TestingRepository(_db, _trans);
        }

        public void Commit()
        {
            _trans?.Commit();
        }

        public void Rollback()
        {
            _trans?.Rollback();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _trans?.Dispose();
                    _db?.Close();
                    _db?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
