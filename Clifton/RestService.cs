using System.Collections.Generic;
using System.Net;

using Newtonsoft.Json;
using RestSharp;

namespace Clifton
{
    public static class RestService
    {
        public static (HttpStatusCode status, string content) Get(string url, Dictionary<string, string> headers = null)
        {
            var response = Execute(url, Method.GET, headers: headers);

            return (response.StatusCode, response.Content);
        }

        public static (T item, HttpStatusCode status, string content) Get<T>(string url, Dictionary<string, string> headers = null) where T : new()
        {
            var response = Execute(url, Method.GET, headers: headers);
            T ret = TryDeserialize<T>(response);

            return (ret, response.StatusCode, response.Content);
        }

        public static (HttpStatusCode status, string content) Post(string url, object data, Dictionary<string, string> headers = null)
        {
            var response = Execute(url, Method.POST, data, headers);

            return (response.StatusCode, response.Content);
        }

        public static (T item, HttpStatusCode status, string content) Post<T>(string url, object data, Dictionary<string, string> headers = null) where T : new()
        {
            var response = Execute(url, Method.POST, data, headers);
            T ret = TryDeserialize<T>(response);

            return (ret, response.StatusCode, response.Content);
        }

        public static (HttpStatusCode status, string content) Patch(string url, object data, Dictionary<string, string> headers = null)
        {
            var response = Execute(url, Method.PATCH, data, headers);

            return (response.StatusCode, response.Content);
        }

        public static (T item, HttpStatusCode status, string content) Patch<T>(string url, object data, Dictionary<string, string> headers = null) where T : new()
        {
            var response = Execute(url, Method.PATCH, data, headers);
            T ret = TryDeserialize<T>(response);

            return (ret, response.StatusCode, response.Content);
        }

        public static (HttpStatusCode status, string content) Delete(string url, Dictionary<string, string> headers = null)
        {
            var response = Execute(url, Method.DELETE, headers: headers);

            return (response.StatusCode, response.Content);
        }

        private static IRestResponse Execute(string url, Method method, object data = null, Dictionary<string, string> headers = null)
        {
            var client = new RestClient(url);
            var request = new RestRequest(method);
            headers?.ForEach(kvp => request.AddHeader(kvp.Key, kvp.Value));

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
