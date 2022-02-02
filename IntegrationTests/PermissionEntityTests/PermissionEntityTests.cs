using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using IntegrationTests.Models;
using WorkflowTestMethods;

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
                .CreateUserAndEntityRoll("Test", "Marc", "fizbin", "CreateEntityRoll", new Permissions() { CanCreate = true })
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0));
        }
    }
}
