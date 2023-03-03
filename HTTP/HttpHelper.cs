using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebCommon.HTTP
{
    public class HttpsHelper
    {
        private readonly bool _useHttps = false;

        public HttpsHelper(bool useHttps)
        {
            _useHttps = useHttps;
        }

        public async Task<string> LoginAsync_JWT(string username, string password, string token, string requsetUrl)
        {
            string result = null;
            try
            {
                HttpClientHandler clientHandler;

                if (_useHttps)
                {
                    clientHandler = new HttpClientHandler();
                    clientHandler.UseCookies = true;
                    clientHandler.AllowAutoRedirect = true;
                    clientHandler.UseCookies = true;
                    clientHandler.ClientCertificateOptions = ClientCertificateOption.Automatic;
                }
                else
                {
                    clientHandler = new HttpClientHandler();
                }

                KeyValuePair<string, string>[] loginRequsetParam = new KeyValuePair<string, string>[]
                {
                new KeyValuePair<string, string>("_token",token),
                new KeyValuePair<string, string>("user", username),
                new KeyValuePair<string, string>("password", password)
                };

                FormUrlEncodedContent requestContent = new FormUrlEncodedContent(loginRequsetParam);

                using (HttpClient httpClient = new HttpClient(clientHandler))
                {
                    HttpResponseMessage response = await httpClient.PostAsync(requsetUrl, requestContent);
                    //response.EnsureSuccessStatusCode();
                    HttpStatusCode statusCode = response.StatusCode;
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (System.Exception ex)
            {
                Log.Error.Write(false, "error", ex);
            }

            return result;
        }

        public async Task<string> GetAsync(string url)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(url);
            string str = await response.Content.ReadAsStringAsync();
            return str;
        }
    }
}