#define DEVELOPMENT

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bot {
    class HttpHelper {
#if DEVELOPMENT
        public static string DOMAIN = "https://devplay.texasholdem.com/";
        string uri = "https://devlogin.texasholdem.com/oauth/token";
#else
        public static string DOMAIN = "https://play.texasholdem.com/";
        string uri = "https://login.texasholdem.com/oauth/token";
#endif

        private static HttpHelper instance;

        static public HttpHelper Instance {
            get {
                if (instance == null) {
                    instance = new HttpHelper();
                }
                return instance;
            }//get
        }//Instance

        string token = "";

        private HttpHelper() {

        }

        public string GetAccessToken(string username, string pw) {
            string post_data = "grant_type=password&username=" + username + "&password=" + pw + "&client_id=s6BhdRkqt3&client_secret=123";

            byte[] postBytes = Encoding.ASCII.GetBytes(post_data);

            HttpWebRequest request = setupPostRequest(uri);
            request.ContentLength = postBytes.Length;
            var requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string jsonMessage = new StreamReader(response.GetResponseStream()).ReadToEnd();
                TokenInfo t = JsonConvert.DeserializeObject<TokenInfo>(jsonMessage);
                token = t.access_token;

                return token;
            }
            catch (WebException) {
                return "";
            }//catch
        }//getAccesToken

        HttpWebRequest setupPostRequest(string uri) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = (10000);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.Headers.Add("Authorization", "Bearer " + token);
            request.ContentType = "application/x-www-form-urlencoded";
            return request;
        }
    }//HttpHelper
}//Bot
