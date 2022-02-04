using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using IntegrationTests.Models;
using WorkflowTestMethods;

using Models;

namespace IntegrationTests.PermissionEntityTests
{
    [TestClass]
    public class PermissionEntityTests : Setup
    {
        [TestMethod]
        public void UserCanCreateEntityTest()
        {
            ClearAllTables();

            var wp = new WorkflowPacket(URL)
                .CreateUserAndEntityRoll("Test", "Marc", "fizbin", "CreateEntityRole", new Permissions() { CanCreate = true })
                .Login("Marc", "fizbin")
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0));
        }

        [TestMethod]
        public void UserCannotCreateEntityTest()
        {
            ClearAllTables();

            var wp = new WorkflowPacket(URL)
                .CreateUserAndEntityRoll("Test", "Marc", "fizbin", "CreateEntityRole", new Permissions() { CanRead = true })
                .Login("Marc", "fizbin")
                .Post<Test>("entity/test", testData)
                .AndForbidden();
        }

        [TestMethod]
        public void UserCanReadEntityTest()
        {
            ClearAllTables();

            var wp = new WorkflowPacket(URL)
                .CreateUserAndEntityRoll("Test", "Marc", "fizbin", "CreateEntityRole", new Permissions() { CanRead = true })
                // Post something as SysAdmin
                .Post<Test>("entity/test", testData)
                .Login("Marc", "fizbin")
                .Get<List<Test>>("entity/test")
                .AndOk()
                .IShouldSee<List<Test>>(data => data.Count.Should().Be(1));
        }
    }
}
