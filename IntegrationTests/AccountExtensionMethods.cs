using FluentAssertions;

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
                .Then(wp => token = wp.GetObject<LoginResponse>().AccessToken)
                .UseHeader("Authorization", $"Bearer {token}");

            return wp;
        }

        public static WorkflowPacket CreateUserAndEntityRole(this WorkflowPacket wp, string entity, string username, string password, string roleName, Permissions permissions)
        {
            int roleId = -1;
            int entityId = -1;
            int userId = -1;

            wp
                .Login()
                .Post<User>("account", new { username, password })
                .AndOk()
                .IShouldSee<User>(u => u.Id.Should().NotBe(0))
                .IGet<User>(u => userId = u.Id)
                .Log($"User ID = {userId}")

                .Post<Role>("entity/role", new
                {
                    Name = roleName,
                    CanCreate = permissions.CanCreate,
                    CanRead = permissions.CanRead,
                    CanUpdate = permissions.CanUpdate,
                    CanDelete = permissions.CanDelete,
                })
                .AndOk()
                .IShouldSee<Role>(r => r.Id.Should().NotBe(0))
                .IGet<Role>(r => roleId = r.Id)

                .Post<Entity>("entity/entity", new { TableName = entity })
                .AndOk()
                .IShouldSee<Entity>(e => e.Id.Should().NotBe(0))
                .IGet<Entity>(e => entityId = e.Id)

                // Map EntityRole and UserRole.
                .Post<UserRole>("entity/userrole", new { RoleId = roleId, UserId = userId })
                .AndOk()
                .IShouldSee<UserRole>(ur => ur.Id.Should().NotBe(0))
                .Post<EntityRole>("entity/entityrole", new { RoleId = roleId, EntityId = entityId })
                .AndOk()
                .IShouldSee<EntityRole>(er => er.Id.Should().NotBe(0));

            return wp;
        }
    }
}
