using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using Models.Responses;

using WorkflowTestMethods;

namespace IntegrationTests.AccountTests
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
    }

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
