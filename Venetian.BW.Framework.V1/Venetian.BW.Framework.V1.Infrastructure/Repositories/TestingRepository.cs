using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.Domain.Testings;

namespace Venetian.BW.Framework.V1.Infrastructure.Repositories
{
    public class TestingRepository : ITestingRepository
    {
        private SqlConnection _db;
        private SqlTransaction _trans;

        public TestingRepository(SqlConnection db, SqlTransaction trans)
        {
            _db = db;
            _trans = trans;
        }
        public async Task<List<Testing>> GetTestingList()
        {
            string sql = "select id,name,description from dbo.bwtest";

            return (await _db.QueryAsync<Testing>(sql, null, _trans)).ToList();
        }
    }
}
