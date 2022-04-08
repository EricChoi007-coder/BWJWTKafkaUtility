namespace Venetian.BW.Framework.V1.API
{
    public class AppSetting
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string DbConnection { get; set; }

        //jwt
        public string TokenSecret { get; set; }

        public string TokenAud { get; set; }
        public string TokenIss { get; set; }
        public int AccessTokenValidMinute { get; set; }
        public int RefreshTokenValidMinute { get; set; }
        public string Connection { get; set; }
        public string PareSystemNameList { get; set; }
    }
}