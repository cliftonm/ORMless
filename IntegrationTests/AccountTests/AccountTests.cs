using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using Models;
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
                .AndOk()
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
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull())
                .Post("account/logout", null)
                .AndOk()

                // login as SysAdmin to access User table.
                .Login()
                .AndOk()
                .Get<List<User>>($"entity/User")
                .AndOk()
                .IShouldSee<List<User>>(users => users.FirstOrDefault(u => u.UserName == "Marc" && u.AccessToken != null).Should().BeNull())
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
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull());
        }

        [TestMethod]
        public void ChangeUsernameAndPasswordTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull())
                .Patch("account", new { Username = "Thomas", Password = "texasHoldem" })
                .AndOk()
                .Post<LoginResponse>("account/login", new { Username = "Marc", Password = "fizbin" })
                .AndUnauthorized()
                .Login("Thomas", "texasHoldem")
                .AndOk();
        }

        [TestMethod]
        public void ChangePasswordOnlyTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull())
                .Patch("account", new { Password = "texasHoldem" })
                .AndOk()
                .Post<LoginResponse>("account/login", new { Username = "Marc", Password = "fizbin" })
                .AndUnauthorized()
                .Login("Marc", "texasHoldem")
                .AndOk();
        }

        [TestMethod]
        public void DeleteAccountTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull())
                .Delete("account")
                .AndOk()
                .Post<LoginResponse>("account/login", new { Username = "Marc", Password = "fizbin" })
                .AndUnauthorized();
        }

        [TestMethod]
        public void RefreshTokenTest()
        {
            ClearAllTables();
            LoginResponse resp = null;

            new WorkflowPacket(URL)
                .Login()
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull())
                .IGet<LoginResponse>(r => resp = r)
                .Post($"account/refresh/{resp.refresh_token}", null)
                .AndOk();
        }

        [TestMethod]
        public void ExpiredTokenTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .Post("account/expireToken", null)
                .AndOk()
                .Post("account/logout", null)

                // Do something that requires authorization.
                .AndUnauthorized();
        }

        [TestMethod]
        public void ExpiredRefreshTokenTest()
        {
            ClearAllTables();
            LoginResponse resp = null;

            new WorkflowPacket(URL)
                .Login()
                .Post("account", new { Username = "Marc", Password = "fizbin" })
                .AndOk()
                .Login("Marc", "fizbin")
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull())
                .IGet<LoginResponse>(r => resp = r)

                .Post("account/expireRefreshToken", null)
                .AndOk()

                .Post($"account/refresh/{resp.refresh_token}", null)
                .AndUnauthorized();
        }
    }
}
