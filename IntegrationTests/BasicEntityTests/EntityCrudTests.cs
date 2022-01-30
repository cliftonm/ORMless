using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using IntegrationTests.Models;
using WorkflowTestMethods;

namespace IntegrationTests
{
    [TestClass]
    public class EntityCrudTests : Setup
    {
        [TestMethod]
        public void CreateEntityTest()
        {
            ClearAllTables();

            var wp = new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0));
        }

        [TestMethod]
        public void UpdateEntityTest()
        {
            int id = -1;

            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0))
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Then(() => testData.StringField = "test2")
                .Patch<Test>($"entity/test/{id}", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.StringField.Should().Be("test2"));
        }

        [TestMethod]
        public void SoftDeleteEntityTest()
        {
            int id = -1;

            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0))
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Delete($"entity/test/{id}")
                .AndNoContent()
                .Get<Test>($"entity/test/{id}")
                .AndNotFound();
        }

        [TestMethod]
        public void HardDeleteEntityTest()
        {
            int id = -1;

            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0))
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Delete($"entity/test/{id}/Hard")
                .AndNoContent()
                .Get<Test>($"entity/test/{id}")
                .AndNotFound();
        }

        [TestMethod]
        public void ShouldNotSeeDeletedEntityTest()
        {
            int id = -1;

            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0))
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Delete($"entity/test/{id}")
                .AndNoContent()
                .Get<List<Test>>($"entity/test")
                .AndOk()
                .IShouldSee<List<Test>>(t => t.Count.Should().Be(0));
        }

        [TestMethod]
        public void GetAllTest()
        {
            ClearAllTables();

            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .Post<Test>("entity/test", testData2)
                .AndOk()
                .Break()
                .Get<List<Test>>($"entity/test")
                .IShouldSee<List<Test>>(t => t.Count.Should().Be(2));
        }

        [TestMethod]
        public void GetByIdTest()
        {
            int id = -1;

            ClearAllTables();
            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .AndOk()
                .Get<Test>($"entity/test/{id}")
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().Be(id));
        }
    }
}
