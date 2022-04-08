using JWT.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venetian.BW.Framework.V1.API;

namespace Venetian.BW.Framework.V1.CommonUtility.JWTUtility
{
    public class AuthAttribute : Attribute, IAuthorizationFilter, IFilterFactory
    {
        //Const Values
        public const string KeyUserID = "user_id";
        public const string KeyUsername = "username";
        public const string KeyEmployeeId = "employeeId";
        public const string KeyTokenType = "tokenType";
        public const string KeyNBF = "nbf";
        public const string KeyEXP = "exp";
        public const string KeyIAT = "iat";
        public const string KeyISS = "iss";
        public const string KeyAUD = "aud";
        public const string KeyDeviceUniqueId = "deviceUniqueId";
        public const string AccessToken = "access_token";
        public const string RefreshToken = "refresh_token";
        public const string KeyAccessToken = "accessToken";
        public const string pareSystemName = "pareSystemName";

        //value get by attribute constructor
        public string pareSystemValidateName { set; get; } 

        protected IOptions<AppSetting> setting;

        public bool IsReusable => false;

        public AuthAttribute(string pareSystemValidateName)
        {
            this.pareSystemValidateName = pareSystemValidateName;
        }

        public AuthAttribute(string pareSystemValidateName,IOptions<AppSetting> setting)
        {
            this.setting = setting;
           this.pareSystemValidateName = pareSystemValidateName;

        }


        public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new AuthAttribute(this.pareSystemValidateName,
            serviceProvider.GetService<IOptions<AppSetting>>()
            );
        }

        public virtual void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                //setting = context.HttpContext.RequestServices.GetService<IOptions<AppSetting>>();
                string tokenSecret = setting.Value.TokenSecret;
                string tokenAud = setting.Value.TokenAud;
                string tokenIss = setting.Value.TokenIss;
                int accessTokenValidMinute = setting.Value.AccessTokenValidMinute;
                int refreshTokenValidMinute = setting.Value.RefreshTokenValidMinute;
                string connection = setting.Value.Connection;


                StringValues tokens = new StringValues();
                if (!context.HttpContext.Request.Headers.TryGetValue("token", out tokens))
                {
                    throw new SecurityTokenException("missing token");
                }

                if (tokens.Count() <= 0)
                {
                    throw new SecurityTokenException("missing token");
                }

                string token = tokens.FirstOrDefault();
                if (string.IsNullOrEmpty(token))
                {
                    throw new SecurityTokenException("missing token");
                }

                var manager = new JWTManager(tokenSecret, tokenAud, tokenIss,
                accessTokenValidMinute, refreshTokenValidMinute, setting.Value.PareSystemNameList);
                var isValidate = manager.ValidateAccessToken(token);
                var items = manager.GetItems();

                var name = pareSystemValidateName;

                if (isValidate.isValid && isValidate.pareSystemName == this.pareSystemValidateName)
                {
                    var userId = Convert.ToInt64(items[KeyUserID]);

                    for (int i = 0; i < items.Count; i++)
                    {
                        var item = items.ElementAt(i);
                        context.HttpContext.Items[item.Key] = item.Value;
                    }
                    context.HttpContext.Items[KeyAccessToken] = token;
                }
                else 
                {
                    throw new Exception("Application name is not included in the white list");
                }

            }
            catch (SecurityTokenExpiredException e)
            {
                throw new TokenExpiredException("token expired");
            }
            catch (SecurityTokenInvalidAudienceException e)
            {
                //throw new TokenInvalidException("invalid aud");
                throw e;
            }
            catch (Exception e)
            {
                throw ;
            }


        }
    }
}
