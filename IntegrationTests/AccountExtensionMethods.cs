using System;
using System.Collections.Generic;
using System.Text;

using Clifton.IntegrationTestWorkflowEngine;

using Models.Responses;
using WorkflowTestMethods;

using IntegrationTests.Models;

namespace IntegrationTests
{
    public static class AccountExtensionMethods
    {
        public static WorkflowPacket Login(this WorkflowPacket wp, string username = "SysAdmin", string password = "SysAdmin")
        {
            string token = null;

            wp
                .Post<LoginResponse>("account/login", new { Username = username, Password = password })
                .AndOk()
                .Then(wp => token = wp.GetObject<LoginResponse>().access_token)
                .UseHeader("Authorization", $"Bearer {token}");

            return wp;
        }

        public static WorkflowPacket CreateUserAndEntityRoll(this WorkflowPacket wp, string entity, string username, string password, Permissions permissions)
        {
            return wp;
        }
    }
}
