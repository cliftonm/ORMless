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
    [TestClass]
    public class AccountTests : Setup
    {
        [TestMethod]
        public void SysAdminLoginTest()
        {
            new WorkflowPacket(URL)
                .Post<LoginResponse>("account/login", new { Username = "SysAdmin", Password = "SysAdmin" })
                .AndOk()
                .IShouldSee<LoginResponse>(r => r.access_token.Should().NotBeNull());
        }

        [TestMethod]
        public void BadLoginTest()
        {
            new WorkflowPacket(URL)
                .Post<LoginResponse>("account/login", new { Username = "baad", Password = "f00d" })
                .AndUnauthorized();
        }
    }
}
