using System;
using Venetian.BW.Framework.V1.Domain.Exceptions;

namespace Venetian.BW.Framework.V1.API.Responses.JWTResponses
{
    public class JWTResponse
    {
        public string jwtToken { set; get; }
        public string jwtRefreshToken { set; get; }
    }
}
