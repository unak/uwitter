using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Uwitter
{
    class Twitter : IDisposable
    {
        const string REQUEST_TOKEN_URL = @"https://api.twitter.com/oauth/request_token";
        const string AUTHORIZE_URL = @"https://api.twitter.com/oauth/authorize";
        const string AUTHENTICATE_URL = @"https://api.twitter.com/oauth/authenticate";
        const string ACCESS_TOKEN_URL = @"https://api.twitter.com/oauth/access_token";
        const string HOME_TIMELINE_URL = @"https://api.twitter.com/1.1/statuses/home_timeline.json";
        const string USER_TIMELINE_URL = @"https://api.twitter.com/1.1/statuses/user_timeline.json";
        const string UPDATE_STATUS_URL = @"https://api.twitter.com/1.1/statuses/update.json";
        const string RETWEET_URL = @"https://api.twitter.com/1.1/statuses/retweet/{0}.json";
        const string DESTROY_URL = @"https://api.twitter.com/1.1/statuses/destroy/{0}.json";
        const string USERSTREAM_URL = @"https://userstream.twitter.com/1.1/user.json";

        string consumerKey;
        string consumerSecret;
        string requestToken;
        string requestTokenSecret;
        string accessToken;
        string accessTokenSecret;
        string userId;
        string screenName;
        Random rand;

        public bool IsActive
        {
            get
            {
                return !string.IsNullOrEmpty(accessTokenSecret);
            }
        }

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

        public void Dispose()
        {
            if (streamThread != null && streamThread.IsAlive)
            {
                streamSignal = true;
                //streamThread.Join();
            }
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

            return AUTHENTICATE_URL + "?oauth_token=" + Uri.EscapeDataString(requestToken);
        }

        public string GetAccessToken(string pin)
        {
            var parameters = SetupInitialParameters();
            parameters.Add("oauth_token", Uri.EscapeDataString(requestToken));
            parameters.Add("oauth_verifier", pin);
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

        private Thread streamThread = null;
        private Queue<Timeline> timelineQueue = new Queue<Timeline>();
        private bool streamSignal = false;
        public List<Timeline> GetTimeline(decimal? since_id = null, bool old = false)
        {
            List<Timeline> list = null;

            if (since_id == null || old)
            {
                // 30個ほどRESTで取ってくる
                var parameters = SetupInitialParameters();
                if (since_id != null && old)
                {
                    parameters.Add("max_id", since_id.ToString());
                }
                parameters.Add("count", "30");
                parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
                parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", HOME_TIMELINE_URL, parameters, accessTokenSecret)));

                var body = HttpGet(HOME_TIMELINE_URL, parameters);
                if (body == null)
                {
                    return null;
                }
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
                {
                    var serealizer = new DataContractJsonSerializer(typeof(Timeline[]));
                    list = new List<Timeline>((Timeline[])serealizer.ReadObject(ms));
                }
            }

            if (streamThread == null || !streamThread.IsAlive)
            {
                var parameters = SetupInitialParameters();
                parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
                parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", USERSTREAM_URL, parameters, accessTokenSecret)));

                var stream = HttpGetStream(USERSTREAM_URL, parameters);
                if (stream == null)
                {
                    return null;
                }

                streamThread = new Thread(delegate()
                {
                    var serializer = new DataContractJsonSerializer(typeof(Timeline));
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        while (!streamSignal && (line = reader.ReadLine()) != null)
                        {
                            if (line.StartsWith("{\"created_at\""))
                            {
                                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(line)))
                                {
                                    var tweet = (Timeline)serializer.ReadObject(ms);
                                    lock (((ICollection)timelineQueue).SyncRoot)
                                    {
                                        timelineQueue.Enqueue(tweet);
                                    }
                                }
                            }
                        }
                        if (streamSignal)
                        {
                            // XXX:FIXME!!!
                        }
                    }
                    stream.Dispose();
                });
                streamThread.Start();
            }

            lock (((ICollection)timelineQueue).SyncRoot)
            {
                while (timelineQueue.Count > 0)
                {
                    var tweet = timelineQueue.Dequeue();
                    if (since_id == null || tweet.id > since_id)
                    {
                        if (list == null)
                        {
                            list = new List<Timeline>();
                        }
                        list.Add(tweet);
                    }
                }
            }

            return list;
        }

        public Timeline GetUserTimeline(string user_id, decimal id)
        {
            var parameters = SetupInitialParameters();
            parameters.Add("user_id", user_id);
            parameters.Add("count", "1");
            parameters.Add("max_id", id.ToString());
            parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("GET", USER_TIMELINE_URL, parameters, accessTokenSecret)));

            var body = HttpGet(USER_TIMELINE_URL, parameters);
            if (body == null)
            {
                return null;
            }
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(body)))
            {
                var serealizer = new DataContractJsonSerializer(typeof(Timeline[]));
                var list = new List<Timeline>((Timeline[])serealizer.ReadObject(ms));
                if (list.Count == 1)
                {
                    return list[0];
                }
            }
            return null;
        }

        public bool SendTweet(string tweet, decimal? in_reply_to = null)
        {
            var parameters = SetupInitialParameters();
            parameters.Add("status", Uri.EscapeDataString(tweet));
            if (in_reply_to != null)
            {
                parameters.Add("in_reply_to_status_id", in_reply_to.ToString());
            }
            parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("POST", UPDATE_STATUS_URL, parameters, accessTokenSecret)));

            var body = HttpPost(UPDATE_STATUS_URL, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }

            return true;
        }

        public bool Retweet(decimal id)
        {
            var parameters = SetupInitialParameters();
            parameters.Add("id", Uri.EscapeDataString(id.ToString()));
            parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
            var url = string.Format(RETWEET_URL, id.ToString());
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("POST", url, parameters, accessTokenSecret)));

            var body = HttpPost(url, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }

            return true;
        }

        public bool Destroy(decimal id)
        {
            var parameters = SetupInitialParameters();
            parameters.Add("id", Uri.EscapeDataString(id.ToString()));
            parameters.Add("oauth_token", Uri.EscapeDataString(accessToken));
            var url = string.Format(DESTROY_URL, id.ToString());
            parameters.Add("oauth_signature", Uri.EscapeDataString(GenerateSignature("POST", url, parameters, accessTokenSecret)));

            var body = HttpPost(url, parameters);
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }

            return true;
        }

        public static Stream HttpGetStream(string url, SortedDictionary<string, string> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url += '?' + JoinParameters(parameters);
            }
            return HttpRequest<Stream>(url, null, (res) =>
            {
                return res.GetResponseStream();
            });
        }

        public static string HttpGet(string url, SortedDictionary<string, string> parameters)
        {
            using (var stream = HttpGetStream(url, parameters))
            {
                string body;
                using (var reader = new StreamReader(stream))
                {
                    body = reader.ReadToEnd();
                }
                return body;
            }
        }

        public static byte[] HttpGetBinary(string url, SortedDictionary<string, string> parameters)
        {
            byte[] body = new byte[0];
            using (var stream = HttpGetStream(url, parameters))
            {
                using (var reader = new System.IO.BinaryReader(stream))
                {
                    var buf = new byte[4096];
                    int read;
                    while ((read = reader.Read(buf, 0, buf.Length)) > 0)
                    {
                        Array.Resize(ref buf, read);
                        Array.Resize(ref body, body.Length + read);
                        buf.CopyTo(body, body.Length - read);
                    }
                }
            }
            return body;
        }

        public static string HttpPost(string url, SortedDictionary<string, string> parameters)
        {
            return HttpRequest<string>(url, parameters, (res) =>
            {
                string body;
                using (var stream = res.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                return body;
            });
        }

        private delegate T HttpRequestCallback<T>(WebResponse res);
        private static T HttpRequest<T>(string url, SortedDictionary<string, string> parameters, HttpRequestCallback<T> callback)
        {
            var req = WebRequest.Create(url);
            if (Properties.Settings.Default.UseProxy)
            {
                req.Proxy = new WebProxy(Properties.Settings.Default.ProxyHost, Properties.Settings.Default.ProxyPort);
            }
            ((HttpWebRequest)req).UserAgent = Application.ProductName + ' ' + Application.ProductVersion;
            byte[] data = null;
            if (parameters != null)
            {
                data = Encoding.ASCII.GetBytes(JoinParameters(parameters));
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = data.Length;
            }

            try
            {
                if (data != null)
                {
                    using (var stream = req.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                return callback(req.GetResponse());
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    using (var stream = ex.Response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            var result = reader.ReadToEnd();
                            // XXX:FIXME!!! 毎回メッセージボックスはウザイので別の方法が必要
                            MessageBox.Show(ex.Message + "\n" + result, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                return default(T);
            }
            catch (Exception ex)
            {
                // XXX:FIXME!!! 毎回メッセージボックスはウザイので別の方法が必要
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default(T);
            }
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

        private static string JoinParameters(IDictionary<string, string> parameters)
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
