using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using Models.Responses;

using WorkflowTestMethods;

namespace IntegrationTests.AccountTests
{
    [TestClass]
    public class AccountTests : Setup
    {
        [TestMethod]
        public void SysAdminLoginTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull());
        }

        [TestMethod]
        public void BadLoginTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Post<LoginResponse>("account/login", new { Username = "baad", Password = "f00d" })
                .AndUnauthorized();
        }

        [TestMethod]
        public void LogoutTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post("account/logout", null)
                .AndOk();
        }

        [TestMethod]
        public void LogoutBadAuthTest()
        {
            ClearAllTables();

            string token = Guid.NewGuid().ToString();

            new WorkflowPacket(URL)
                .UseHeader("Authorization", $"Bearer {token}")
                .Post("account/logout", null)
                .AndUnauthorized();
        }

        [TestMethod]
        public void CreateAccountTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                // I never use fizbin for any of my accounts, if you are wondering.
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                // And the user can log in
                .Login("Marc", "fizbin")
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull());
        }
    }
}
