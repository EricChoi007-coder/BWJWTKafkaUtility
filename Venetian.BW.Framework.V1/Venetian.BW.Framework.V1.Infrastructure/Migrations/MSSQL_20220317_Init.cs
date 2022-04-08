using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.Infrastructure.Migrations
{
    public class MSSQL_20220317_Init
    {
        public void Up()
        {
            string sql = @"
                create table sys_logs(
	                id bigint identity(1,1) primary key,
                    logLevel varchar(10) not null,
                    createDate datetime,
                    functionName varchar(1000),
                    lineNumber varchar(10),
                    message nvarchar(max),
                    exception nvarchar(max),
                    requestMethod varchar(10),
                    requestUrl nvarchar(500),
                    requestQueryString nvarchar(500),
					requestBody nvarchar(max),
                    requestUserAgent varchar(500),
                    requestIp varchar(100),
                    requestHost varchar(100),
                    requestHeaderReferer varchar(100),
                    requestHeaderContentType varchar(500),
                    iisSiteName varchar(100),
                    appDomain varchar(100),
                    machineName varchar(100),
                    boundRequest varchar(100),
                    type varchar(100)
                ); 

create table  bwtest
(
       id                BIGINT PRIMARY KEY IDENTITY(1,1),
       name    VARCHAR(255),
       description       VARCHAR(4000)
      
);
";
        }   }
}
