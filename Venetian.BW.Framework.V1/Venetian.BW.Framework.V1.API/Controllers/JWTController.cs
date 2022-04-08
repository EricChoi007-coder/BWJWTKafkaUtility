using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.API.Requests.JWTRequests;
using Venetian.BW.Framework.V1.API.Responses;
using Venetian.BW.Framework.V1.API.Responses.JWTResponses;
using Venetian.BW.Framework.V1.CommonUtility.JWTUtility;
using Venetian.BW.Framework.V1.Infrastructure.Repositories;

namespace Venetian.BW.Framework.V1.API.Controllers
{
    [ApiController]
    [Route("jwt")]
    public class JWTController : ControllerBase
    {

        private readonly ILogger<JWTController> _logger;
        private readonly AppSetting _setting;
        public JWTController(IOptions<AppSetting> setting, ILogger<JWTController> logger)
        {
            _logger = logger;
            _setting = setting.Value;
        }

        [HttpGet("jwt-test")]
        [Auth("venetian")]
        public OkResult Get()
        {
            return Ok();
        }

        [HttpGet("refresh-token")]

        //public async Task<IActionResult> GetAccessTokenByRefreshToken()
        //{
        //    var response = new BaseResponse<JWTResponse>();
        //    try
        //    {

        //        //reponse data
        //        var data = new JWTResponse();

        //        //db operations
        //        using (var uow = new UnitOfWork(_setting.DbConnection))
        //        {
        //            try
        //            {
        //                //jwt token generation
        //                JWTManager jWTManager = new JWTManager(_setting.TokenSecret, _setting.TokenAud, _setting.TokenIss, _setting.AccessTokenValidMinute, _setting.RefreshTokenValidMinute,_setting.PareSystemNameList);
        //                string jwtToken = jWTManager.GenerateAccessToken();

        //                string jwtRefreshToken = jWTManager.GenerateRefreshToken();
        //                data.jwtToken = jwtToken;
        //                data.jwtRefreshToken = jwtRefreshToken;
        //                //update DB JWT Token & Refresh token
        //                //commit trans
        //                uow.Commit();
        //            }
        //            catch (Exception e)
        //            {
        //                //rollback
        //                uow.Rollback();
        //                throw e;
        //            }

        //        }
        //        response.SetData(data);
        //    }
        //    catch (Exception e)
        //    {

        //        response.SetError(e);
        //    }
        //    return Ok(response);
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest loginRequest)
        {


            var response = new BaseResponse<JWTResponse>();
            try
            {
                //reponse data
                var data = new JWTResponse();

                
                    try
                    {

                        //validation if RegisterRequest.appName is included in the Config[](_setting.PareSystemNameList)

                        //jwt token generation
                        JWTManager jWTManager = new JWTManager(_setting.TokenSecret, _setting.TokenAud, _setting.TokenIss, _setting.AccessTokenValidMinute, _setting.RefreshTokenValidMinute, _setting.PareSystemNameList);
                        jWTManager.AddKeyValue("pareSystemName", loginRequest.appName);
                        string jwtToken = jWTManager.GenerateAccessToken();
                        string jwtRefreshToken = jWTManager.GenerateRefreshToken();
                        data.jwtToken = jwtToken;
                        data.jwtRefreshToken = jwtRefreshToken;
               
                    }
                    catch (Exception e)
                    {
    
                        throw e;
                    }
                
           

                //set the response data
                response.SetData(data);
            }
            catch (Exception e)
            {
                //set the error
                response.SetError(e);
            }
            return Ok(response);
        }
    }
}
