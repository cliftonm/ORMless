using System;
using System.Collections.Generic;
using System.Text;

using Clifton.IntegrationTestWorkflowEngine;

using Models;
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
                .Post<LoginResponse>("account/login", new { username, password })
                .AndOk()
                .Then(wp => token = wp.GetObject<LoginResponse>().access_token)
                .UseHeader("Authorization", $"Bearer {token}");

            return wp;
        }

        public static WorkflowPacket CreateUserAndEntityRoll(this WorkflowPacket wp, string entity, string username, string password, string roleName, Permissions permissions)
        {
            wp
                .Login()
                .Post("account", new { username, password })
                .AndOk()
                .Post<Roll>("entity/roll", new
                {
                    Name = roleName,
                    CanCreate = permissions.CanCreate,
                    CanRead = permissions.CanRead,
                    CanUpdate = permissions.CanUpdate,
                    CanDelete = permissions.CanDelete,
                });

            return wp;
        }
    }
}
