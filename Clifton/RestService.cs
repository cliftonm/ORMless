using System.Diagnostics;
using System.Net;

using Newtonsoft.Json;
using RestSharp;

namespace Clifton
{
    public static class RestService
    {
        public static (HttpStatusCode status, string content) Get(string url)
        {
            var response = Execute(url, Method.GET);

            return (response.StatusCode, response.Content);
        }

        public static (T item, HttpStatusCode status, string content) Get<T>(string url) where T : new()
        {
            var response = Execute(url, Method.GET);
            T ret = TryDeserialize<T>(response);

            return (ret, response.StatusCode, response.Content);
        }

        public static (T item, HttpStatusCode status, string content) Post<T>(string url, object data) where T : new()
        {
            var response = Execute(url, Method.POST, data);
            T ret = TryDeserialize<T>(response);

            return (ret, response.StatusCode, response.Content);
        }

        public static (T item, HttpStatusCode status, string content) Patch<T>(string url, object data) where T : new()
        {
            var response = Execute(url, Method.PATCH, data);
            T ret = TryDeserialize<T>(response);

            return (ret, response.StatusCode, response.Content);
        }

        public static (HttpStatusCode status, string content) Delete(string url)
        {
            var response = Execute(url, Method.DELETE);

            return (response.StatusCode, response.Content);
        }

        private static IRestResponse Execute(string url, Method method, object data = null)
        {
            Debug.WriteLine($"{method}: {url}");
            var client = new RestClient(url);
            var request = new RestRequest(method);

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                request.AddParameter("application/json", json, ParameterType.RequestBody);
            }

            var response = client.Execute(request);

            return response;
        }

        private static T TryDeserialize<T>(IRestResponse response) where T : new()
        {
            T ret = new T();
            int code = (int)response.StatusCode;

            Assertion.SilentTry(() =>
            {
                ret = JsonConvert.DeserializeObject<T>(response.Content);
            });

            return ret;
        }
    }
}
