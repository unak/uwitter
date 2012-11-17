﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Uwitter
{
    class Twitter
    {
        const string REQUEST_TOKEN_URL = "https://api.twitter.com/oauth/request_token";
        const string AUTHORIZE_URL = "https://api.twitter.com/oauth/authorize";
        const string ACCESS_TOKEN_URL = "https://api.twitter.com/oauth/access_token";
        const string HOME_TIMELINE_URL = "https://api.twitter.com/1.1/statuses/home_timeline.json";

        string consumerKey;
        string consumerSecret;
        string requestToken;
        string requestTokenSecret;
        string accessToken;
        string accessTokenSecret;
        string userId;
        string screenName;
        Random rand;

        public Twitter(string key, string secret)
        {
            initialize(key, secret);
        }

        public Twitter(string key, string secret, string token, string tokenSecret)
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
            var parameters = SetupInitialParameters();
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", REQUEST_TOKEN_URL, parameters)));

            var body = HttpGet(REQUEST_TOKEN_URL, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }
            foreach (string kv in body.Split('&'))
            {
                var kvs = kv.Split(new char[]{'='}, 2);
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
            var parameters = SetupInitialParameters();
            parameters.Add("oauth_token", Uri.EscapeDataString(requestToken));
            parameters.Add("oauth_verifyer", pin);
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", ACCESS_TOKEN_URL, parameters)));

            var body = HttpGet(ACCESS_TOKEN_URL, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return null;
            }
            foreach (string kv in body.Split('&'))
            {
                var kvs = kv.Split(new char[]{'='}, 2);
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

        public Timeline[] GetTimeline(string since_id = null)
        {
            var parameters = SetupInitialParameters();
            if (!string.IsNullOrEmpty(since_id))
            {
                parameters.Add("since_id", since_id);
            }
            parameters.Add("count", "10");
            parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", HOME_TIMELINE_URL, parameters, accessTokenSecret)));

            var body = HttpGet(HOME_TIMELINE_URL, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return null;
            }

            var serializer = new DataContractJsonSerializer(typeof(Timeline[]));
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(body));
            var obj = (Timeline[])serializer.ReadObject(stream);
            stream.Close();

            return obj;
        }

        private string HttpGet(string url, SortedDictionary<string, string> parameters)
        {
            var req = WebRequest.Create(url + '?' + JoinParameters(parameters));
            if (Properties.Settings.Default.UseProxy)
            {
                req.Proxy = new WebProxy(Properties.Settings.Default.ProxyHost, Properties.Settings.Default.ProxyPort);
            }
            ((HttpWebRequest)req).UserAgent = Application.ProductName + ' ' + Application.ProductVersion;

            string body;
            try
            {
                var res = req.GetResponse();
                var stream = res.GetResponseStream();
                var reader = new StreamReader(stream);
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
            var parameters = new SortedDictionary<string, string>();
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
            var buf = new StringBuilder(32);
            for (int i = 0; i < 32; ++i)
            {
                buf.Append(letters[rand.Next(letters.Length)]);
            }
            return buf.ToString();
        }

        private string GenerateSignature(string method, string url, IDictionary<string, string> parameters, string tokenSecret = "")
        {
            var sigBase = method + '&' + Uri.EscapeDataString(url) + '&' + Uri.EscapeDataString(JoinParameters(parameters));
            var digest = new HMACSHA1();
            digest.Key = Encoding.ASCII.GetBytes(consumerSecret + '&' + tokenSecret);
            return Convert.ToBase64String(digest.ComputeHash(Encoding.ASCII.GetBytes(sigBase)));
        }

        private string JoinParameters(IDictionary<string, string> parameters)
        {
            var buf = new StringBuilder();
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