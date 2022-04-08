using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Venetian.BW.Framework.V1.CommonUtility.JWTUtility
{
    public class JWTManager
    {
        private int accessTokenValidMinute;
        private string tokenSecret;
        private string tokenAud;
        private string tokenIss;
        private int refreshTokenValidMinute;
        private string pareSystemNameList;

        private IDictionary<string, string> itemsKV;


        public JWTManager(string tokenSecret, string tokenAud, string tokenIss,
        int accessTokenValidMinute, int refreshTokenValidMinute, string pareSystemNameList)
        {
            this.tokenSecret = tokenSecret;
            this.tokenAud = tokenAud;
            this.tokenIss = tokenIss;
            this.accessTokenValidMinute = accessTokenValidMinute;
            this.refreshTokenValidMinute = refreshTokenValidMinute;
            this.pareSystemNameList = pareSystemNameList;

            itemsKV = new Dictionary<string, string>();

        } 

        public void AddKeyValue(string key, string value)
        {
            itemsKV.Add(key, value);
        }

        public void AddKeyValue(IDictionary<string, string> dict)
        {
            for (int i = 0; i < dict.Count; i++)
            {
                itemsKV.Add(dict.ElementAt(i));
            }
        }

        public string GenerateAccessToken()
        {
            var handler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;

            var claims = new List<Claim>();
            for (int i = 0; i < itemsKV.Count; i++)
            {
                var item = itemsKV.ElementAt(i);
                var claim = new Claim(item.Key, item.Value);
                claims.Add(claim);
            }

            var tokenType = new Claim(AuthAttribute.KeyTokenType, AuthAttribute.AccessToken);
            claims.Add(tokenType);

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = tokenAud,
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(accessTokenValidMinute),
                SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret)),
            SecurityAlgorithms.HmacSha256),
                Issuer = tokenIss
            };

            //var token = handler.CreateToken(descriptor);
            var token = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(token);

        }

        public string GenerateRefreshToken()
        {
            var handler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;

            var claims = new List<Claim>();
            for (int i = 0; i < itemsKV.Count; i++)
            {
                var item = itemsKV.ElementAt(i);
                var claim = new Claim(item.Key, item.Value);
                claims.Add(claim);
            }

            var tokenType = new Claim(AuthAttribute.KeyTokenType, AuthAttribute.RefreshToken);
            claims.Add(tokenType);

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = tokenAud,
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(refreshTokenValidMinute),
                SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret)),
            SecurityAlgorithms.HmacSha256),
                Issuer = tokenIss
            };

            var token = handler.CreateToken(descriptor);
            //var token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        public JWTValidationResult ValidateAccessToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(accessToken))
            {
                SecurityToken validatedToken;

                var validator = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret)),
                    ValidAudience = tokenAud,
                    ValidIssuer = tokenIss
                };

                var claimsPrincipal = handler.ValidateToken(accessToken, validator, out validatedToken);
                var claims = claimsPrincipal.Claims;

                var validationResult = new JWTValidationResult { isValid = false, pareSystemName = String.Empty };

                for (int i = 0; i < claims.Count(); i++)
                {
                    var claim = claims.ElementAt(i);

                    if (claim.Type == AuthAttribute.KeyTokenType && claim.Value == AuthAttribute.AccessToken)
                    {
                        validationResult.isValid = true;
                    }
                    switch (claim.Type)
                    {
                        case AuthAttribute.KeyNBF:
                        case AuthAttribute.KeyEXP:
                        case AuthAttribute.KeyIAT:
                        case AuthAttribute.KeyISS:
                        case AuthAttribute.KeyAUD:
                        case AuthAttribute.KeyTokenType:
                        case AuthAttribute.pareSystemName: //custome claim 
                            continue;
                        default:
                            itemsKV.Add(claim.Type, claim.Value);
                            break;
                    }
                }

                //validate the claim("pareSystemName") in Config
                if (null != claims.Where(claim => claim.Type == AuthAttribute.pareSystemName).FirstOrDefault())
                    {
                        //get pareSystemName from JWT claim List
                        var pareSystemName = claims.Where(claim => claim.Type == AuthAttribute.pareSystemName).FirstOrDefault().Value;

                        //get paring system name from Config['pareSystemNameList'] for validation
                        List<string> listPareSystemNameElements = pareSystemNameList.Split(',').ToList();   // Trim() ?

                        if (listPareSystemNameElements.Contains(pareSystemName))
                        {
                            validationResult.isValid = true;
                            validationResult.pareSystemName = pareSystemName;
                            return validationResult;
                        }
                        else
                        {
                            validationResult.isValid = false;
                            return validationResult;
                        }
                    }
                    else
                    {
                        validationResult.isValid = false;
                        return validationResult;
                    }


            

                return validationResult;
            }

            return new JWTValidationResult { isValid = false, pareSystemName = String.Empty }; 
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(refreshToken))
            {
                SecurityToken validatedToken;

                var validator = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSecret)),
                    ValidAudience = tokenAud,
                    ValidIssuer = tokenIss
                };

                var claimsPrincipal = handler.ValidateToken(refreshToken, validator, out validatedToken);
                var claims = claimsPrincipal.Claims;

                var isValid = false;
                for (int i = 0; i < claims.Count(); i++)
                {
                    var claim = claims.ElementAt(i);

                    if (claim.Type == AuthAttribute.KeyTokenType && claim.Value == AuthAttribute.RefreshToken)
                    {
                        isValid = true;
                    }

                    switch (claim.Type)
                    {
                        case AuthAttribute.KeyNBF:
                        case AuthAttribute.KeyEXP:
                        case AuthAttribute.KeyIAT:
                        case AuthAttribute.KeyISS:
                        case AuthAttribute.KeyAUD:
                        case AuthAttribute.KeyTokenType:
                            continue;
                        default:
                            itemsKV.Add(claim.Type, claim.Value);
                            break;
                    }

                }

                return isValid;
            }

            return false;
        }

        public IDictionary<string, string> GetItems()
        {
            itemsKV.Add("user_id", "1234");  //temp solution
            return itemsKV;
        }

        public string GetValue(string key)
        {
            string value = "";
            itemsKV.TryGetValue(key, out value);
            return value;
        }

        public int GetValidMinute()
        {
            return accessTokenValidMinute;
        }
    }
}
