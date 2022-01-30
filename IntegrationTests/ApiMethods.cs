using System;
using System.Collections.Generic;

using FluentAssertions;

using Clifton;
using Clifton.IntegrationTestWorkflowEngine;
using System.Net;

namespace WorkflowTestMethods
{
    public static class ApiMethods
    {
        public static WorkflowPacket Get(this WorkflowPacket wp, string route)
        {
            var resp = RestService.Get($"{wp.BaseUrl}/{route}", wp.Headers);
            wp.LastResponse = resp.status;

            return wp;
        }

        public static WorkflowPacket Get<T>(this WorkflowPacket wp, string route) where T : new()
        {
            return wp.Get<T>(typeof(T).Name, route);
        }

        public static WorkflowPacket Get<T>(this WorkflowPacket wp, string name, string route) where T: new()
        {
            var resp = RestService.Get<T>($"{wp.BaseUrl}/{route}", wp.Headers);
            wp.LastResponse = resp.status;
            wp.SetObject(name, resp.item);

            return wp;
        }

        public static WorkflowPacket Post(this WorkflowPacket wp, string route, object data)
        {
            var resp = RestService.Post($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;

            return wp;
        }

        public static WorkflowPacket Post<T>(this WorkflowPacket wp, string route, object data) where T : new()
        {
            return wp.Post<T>(typeof(T).Name, route, data);
        }

        public static WorkflowPacket Post<T>(this WorkflowPacket wp, string name, string route, object data) where T : new()
        {
            var resp = RestService.Post<T>($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;
            wp.SetObject(name, resp.item);

            return wp;
        }

        public static WorkflowPacket Patch<T>(this WorkflowPacket wp, string route, object data) where T : new()
        {
            return wp.Patch<T>(typeof(T).Name, route, data);
        }

        public static WorkflowPacket Patch<T>(this WorkflowPacket wp, string name, string route, object data) where T : new()
        {
            var resp = RestService.Patch<T>($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;
            wp.SetObject(name, resp.item);

            return wp;
        }

        public static WorkflowPacket Delete(this WorkflowPacket wp, string route)
        {
            var resp = RestService.Delete($"{wp.BaseUrl}/{route}", wp.Headers);
            wp.LastResponse = resp.status;

            return wp;
        }

        public static WorkflowPacket AndOk(this WorkflowPacket wp)
        {
            wp.LastResponse.Should().Be(HttpStatusCode.OK);

            return wp;
        }

        public static WorkflowPacket AndUnauthorized(this WorkflowPacket wp)
        {
            wp.LastResponse.Should().Be(HttpStatusCode.Unauthorized);

            return wp;
        }

        public static WorkflowPacket AndNotFound(this WorkflowPacket wp)
        {
            wp.LastResponse.Should().Be(HttpStatusCode.NotFound);

            return wp;
        }

        public static WorkflowPacket AndNoContent(this WorkflowPacket wp)
        {
            wp.LastResponse.Should().Be(HttpStatusCode.NoContent);

            return wp;
        }

        public static WorkflowPacket AndBadRequest(this WorkflowPacket wp)
        {
            wp.LastResponse.Should().Be(HttpStatusCode.BadRequest);

            return wp;
        }

        public static WorkflowPacket AndInternalServerError(this WorkflowPacket wp)
        {
            wp.LastResponse.Should().Be(HttpStatusCode.InternalServerError);

            return wp;
        }
    }
}
