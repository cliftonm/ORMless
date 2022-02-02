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
            int roleId = -1;
            int entityId = -1;
            int userId = -1;

            wp
                .Login()
                .Post<User>("account", new { username, password })
                .AndOk()
                .IGet<User>(u => userId = u.Id)
                .Log($"User ID = {userId}")

                .Post<Role>("entity/roll", new
                {
                    Name = roleName,
                    CanCreate = permissions.CanCreate,
                    CanRead = permissions.CanRead,
                    CanUpdate = permissions.CanUpdate,
                    CanDelete = permissions.CanDelete,
                })
                .AndOk()
                .IGet<Role>(r => roleId = r.Id)

                .Post<Entity>("entity/entity", new { TableName = "Test" })
                .AndOk()
                .IGet<Entity>(e => entityId = e.Id);

                // map EntityRole and UserRole

            return wp;
        }
    }
}
