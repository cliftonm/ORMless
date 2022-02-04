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
            wp.LastContent = resp.content;

            return wp;
        }

        public static WorkflowPacket Get<T>(this WorkflowPacket wp, string route) where T : new()
        {
            return wp.Get<T>(typeof(T).Name, route);
        }

        public static WorkflowPacket Get<T>(this WorkflowPacket wp, string name, string route) where T: new()
        {
            wp.Log($"GET: {typeof(T).Name} {route}");
            var resp = RestService.Get<T>($"{wp.BaseUrl}/{route}", wp.Headers);
            wp.LastResponse = resp.status;
            wp.LastContent = resp.content;
            wp.SetObject(name, resp.item);

            return wp;
        }

        public static WorkflowPacket Post(this WorkflowPacket wp, string route, object data)
        {
            wp.Log($"POST: {route}");
            var resp = RestService.Post($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;
            wp.LastContent = resp.content;

            return wp;
        }

        public static WorkflowPacket Post<T>(this WorkflowPacket wp, string route, object data) where T : new()
        {
            return wp.Post<T>(typeof(T).Name, route, data);
        }

        public static WorkflowPacket Post<T>(this WorkflowPacket wp, string name, string route, object data) where T : new()
        {
            wp.Log($"POST: {typeof(T).Name} {route}");
            var resp = RestService.Post<T>($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;
            wp.LastContent = resp.content;
            wp.SetObject(name, resp.item);

            return wp;
        }

        public static WorkflowPacket Patch(this WorkflowPacket wp, string route, object data)
        {
            wp.Log($"PATCH: {route}");
            var resp = RestService.Patch($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;
            wp.LastContent = resp.content;

            return wp;
        }

        public static WorkflowPacket Patch<T>(this WorkflowPacket wp, string route, object data) where T : new()
        {
            return wp.Patch<T>(typeof(T).Name, route, data);
        }

        public static WorkflowPacket Patch<T>(this WorkflowPacket wp, string name, string route, object data) where T : new()
        {
            wp.Log($"PATCH: {typeof(T).Name} {route}");
            var resp = RestService.Patch<T>($"{wp.BaseUrl}/{route}", data, wp.Headers);
            wp.LastResponse = resp.status;
            wp.LastContent = resp.content;
            wp.SetObject(name, resp.item);

            return wp;
        }

        public static WorkflowPacket Delete(this WorkflowPacket wp, string route)
        {
            wp.Log($"DELETE: {route}");
            var resp = RestService.Delete($"{wp.BaseUrl}/{route}", wp.Headers);
            wp.LastResponse = resp.status;
            wp.LastContent = resp.content;

            return wp;
        }

        public static WorkflowPacket AndOk(this WorkflowPacket wp)
        {
            wp.Log("AndOk");
            
            if (wp.LastResponse != HttpStatusCode.OK)
            {
                wp.Log(wp.LastContent);
            }

            wp.LastResponse.Should().Be(HttpStatusCode.OK);

            return wp;
        }

        public static WorkflowPacket AndUnauthorized(this WorkflowPacket wp)
        {
            wp.Log("AndUnauthorized");
            wp.LastResponse.Should().Be(HttpStatusCode.Unauthorized);

            return wp;
        }

        public static WorkflowPacket AndForbidden(this WorkflowPacket wp)
        {
            wp.Log("AndForbidden");
            wp.LastResponse.Should().Be(HttpStatusCode.Forbidden);

            return wp;
        }

        public static WorkflowPacket AndNotFound(this WorkflowPacket wp)
        {
            wp.Log("AndNotFound");
            wp.LastResponse.Should().Be(HttpStatusCode.NotFound);

            return wp;
        }

        public static WorkflowPacket AndNoContent(this WorkflowPacket wp)
        {
            wp.Log("AndNoContent");
            wp.LastResponse.Should().Be(HttpStatusCode.NoContent);

            return wp;
        }

        public static WorkflowPacket AndBadRequest(this WorkflowPacket wp)
        {
            wp.Log("AndBadRequest");
            wp.LastResponse.Should().Be(HttpStatusCode.BadRequest);

            return wp;
        }

        public static WorkflowPacket AndInternalServerError(this WorkflowPacket wp)
        {
            wp.Log("AndInternalServerError");
            wp.LastResponse.Should().Be(HttpStatusCode.InternalServerError);

            return wp;
        }
    }
}
