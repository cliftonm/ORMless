using System.Collections.Generic;
using System.Net;

using FluentAssertions;

namespace Clifton.IntegrationTestWorkflowEngine
{
    public class WorkflowPacket
    {
        public HttpStatusCode LastResponse { get; set; }
        public string LastContent { get; set; }
        public string BaseUrl { get; protected set; }
        public Dictionary<string, object> Container = new Dictionary<string, object>();
        public Dictionary<string, string> Headers = new Dictionary<string, string>();

        public WorkflowPacket(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public dynamic GetObject(string containerName)
        {
            Container.Should().ContainKey(containerName);
            var ret = Container[containerName];

            return ret;
        }

        public T GetObject<T>() where T : class
        {
            T obj = GetObject<T>(typeof(T).Name);

            return obj;
        }

        public T GetObject<T>(string containerName) where T : class
        {
            Container.Should().ContainKey(containerName);
            T ret = Container[containerName] as T;

            return ret;
        }

        public void SetObject(string containerName, object obj)
        {
            Container[containerName] = obj;
        }
    }
}
