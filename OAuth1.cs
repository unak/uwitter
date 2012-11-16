using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Uwitter
{
    class OAuth1
    {
        const string REQUEST_TOKEN_URL = "https://api.twitter.com/oauth/request_token";
        const string AUTHORIZE_URL = "https://api.twitter.com/oauth/authorize";
        const string ACCESS_TOKEN_URL = "https://api.twitter.com/oauth/access_token";

        string consumerKey;
        string consumerSecret;
        string requestToken;
        string requestTokenSecret;
        string accessToken;
        string accessTokenSecret;
        string userId;
        string screenName;
        Random rand;

        public OAuth1(string key, string secret)
        {
            initialize(key, secret);
        }

        public OAuth1(string key, string secret, string token, string tokenSecret)
        {
            initialize(key, secret);
            accessToken = token;
            accessTokenSecret = tokenSecret;
        }

        private void initialize(string key, string secret)
        {
            consumerKey = key;
            consumerSecret = secret;
            rand = new Random();
        }

        public bool GetRequestToken()
        {
            SortedDictionary<string, string> parameters = SetupInitialParameters();
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", REQUEST_TOKEN_URL, parameters)));

            string body = HttpGet(REQUEST_TOKEN_URL, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }
            foreach (string kv in body.Split('&'))
            {
                string[] kvs = kv.Split(new char[]{'='}, 2);
                if (kvs[0].Equals("oauth_token"))
                {
                    requestToken = kvs[1];
                }
                else if (kvs[0].Equals("oauth_token_secret"))
                {
                    requestTokenSecret = kvs[1];
                }
            }

            return true;
        }

        public string GetAuthorizeUrl()
        {
            if (string.IsNullOrEmpty(requestToken))
            {
                return null;
            }

            return AUTHORIZE_URL + "?oauth_token=" + Uri.EscapeDataString(requestToken);
        }

        public string GetAccessToken(string pin)
        {
            SortedDictionary<string, string> parameters = SetupInitialParameters();
            parameters.Add("oauth_token", Uri.EscapeDataString(requestToken));
            parameters.Add("oauth_verifyer", pin);
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", ACCESS_TOKEN_URL, parameters)));

            string body = HttpGet(ACCESS_TOKEN_URL, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return null;
            }
            foreach (string kv in body.Split('&'))
            {
                string[] kvs = kv.Split(new char[]{'='}, 2);
                if (kvs[0].Equals("oauth_token"))
                {
                    accessToken = kvs[1];
                }
                else if (kvs[0].Equals("oauth_token_secret"))
                {
                    accessTokenSecret = kvs[1];
                }
                else if (kvs[0].Equals("user_id"))
                {
                    userId = kvs[1];
                }
                else if (kvs[0].Equals("screen_name"))
                {
                    screenName = kvs[1];
                }
            }

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(screenName))
            {
                return null;
            }

            Properties.Settings.Default.AccessToken = accessToken;
            Properties.Settings.Default.AccessTokenSecret = accessTokenSecret;
            Properties.Settings.Default.UserId = userId;
            Properties.Settings.Default.ScreenName = screenName;
            Properties.Settings.Default.Save();

            return screenName;
        }

        private string HttpGet(string url, SortedDictionary<string, string> parameters)
        {
            WebRequest req = WebRequest.Create(url + '?' + JoinParameters(parameters));
            ((HttpWebRequest)req).UserAgent = Application.ProductName + ' ' + Application.ProductVersion;

            string body;
            try
            {
                WebResponse res = req.GetResponse();
                Stream stream = res.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                body = reader.ReadToEnd();
                reader.Close();
                stream.Close();
            }
            catch (Exception)
            {
                return null;
            }
            return body;
        }

        private SortedDictionary<string, string> SetupInitialParameters()
        {
            SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
            parameters.Add("oauth_consumer_key", OAuthKey.CONSUMER_KEY);
            parameters.Add("oauth_nonce", GenerateNonce());
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_timestamp", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString());
            parameters.Add("oauth_version", "1.0");
            return parameters;
        }

        private string GenerateNonce()
        {
            const string letters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder buf = new StringBuilder(32);
            for (int i = 0; i < 32; ++i)
            {
                buf.Append(letters[rand.Next(letters.Length)]);
            }
            return buf.ToString();
        }

        private string GenerateSignature(string method, string url, IDictionary<string, string> parameters, string tokenSecret = "")
        {
            string sigBase = method + '&' + Uri.EscapeDataString(url) + '&' + Uri.EscapeDataString(JoinParameters(parameters));
            HMACSHA1 digest = new HMACSHA1();
            digest.Key = Encoding.ASCII.GetBytes(consumerSecret + '&' + tokenSecret);
            return Convert.ToBase64String(digest.ComputeHash(Encoding.ASCII.GetBytes(sigBase)));
        }

        private string JoinParameters(IDictionary<string, string> parameters)
        {
            StringBuilder buf = new StringBuilder();
            foreach (var parameter in parameters)
            {
                if (buf.Length > 0)
                {
                    buf.Append('&');
                }
                buf.Append(parameter.Key);
                buf.Append('=');
                buf.Append(parameter.Value);
            }
            return buf.ToString();
        }
    }
}
