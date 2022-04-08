using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.API.Responses;
using Venetian.BW.Framework.V1.API.Responses.TestResponses;
using Venetian.BW.Framework.V1.Domain.Testings;
using Venetian.BW.Framework.V1.Infrastructure.Repositories;

namespace Venetian.BW.Framework.V1.API.Controllers
{
    [Route("api/v{version:apiVersion}/test")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppSetting _setting;
        private readonly ILogger<TestController> _logger;
        public TestController(IOptions<AppSetting> setting, ILogger<TestController> logger)
        {
            _setting = setting.Value;
            _logger = logger;
        }
        [Route("getlist")]
        [HttpGet]
        public async Task<IActionResult> GetTestInfo()
        {
            //Response Format should extend TestResponse
            var response = new BaseResponse<TestResponse>();

            try
            {
                var data = new TestResponse();
              //  var connectionStr = _setting.DbConnection;
                using (var uow = new UnitOfWork(_setting.DbConnection))
                {
                    try
                    {
                        uow.BeginTransaction();   //open unit of work transaction

                        List<Testing> testingListResult = await uow.TestingRepository.GetTestingList();                       

                        data.TestingList = testingListResult;

                        uow.Commit();  //commit transaction
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "db error");

                        uow.Rollback(); //Exception & Roll Back Unit of Work Transaction
                        throw e;
                    }

                }
                response.SetData(data);

            }
            catch (Exception e)
            {
                response.SetError(e);
            }
            return Ok(response);
        }
    }
}
