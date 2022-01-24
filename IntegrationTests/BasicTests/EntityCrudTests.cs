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
        public static Test testData = new Test()
        {
            IntField = 1,
            StringField = "test",
            DateField = DateTime.Parse("8/19/1962"),
            DateTimeField = DateTime.Parse("3/21/1991 7:47 pm"),
            TimeField = DateTime.Parse("12:05 am"),
            BitField = true
        };

        public static Test testData2 = new Test()
        {
            IntField = 2,
            StringField = "test2",
            DateField = DateTime.Parse("8/20/1962"),
            DateTimeField = DateTime.Parse("3/22/1991 7:47 pm"),
            TimeField = DateTime.Parse("12:06 am"),
            BitField = true
        };

        [TestMethod]
        public void CreateEntityTest()
        {
            ClearAllTables();

            var wp = new WorkflowPacket(URL)
                .Post<Test>("entity/test", testData)
                .AndOk()
                .IShouldSee<Test>(t => t.ID.Should().NotBe(0));
        }

        [TestMethod]
        public void UpdateEntityTest()
        {
            int id = -1;

            ClearAllTables();

            var wp = new WorkflowPacket(URL)
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

            var wp = new WorkflowPacket(URL)
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

            var wp = new WorkflowPacket(URL)
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

            var wp = new WorkflowPacket(URL)
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

            var wp = new WorkflowPacket(URL)
                .Post<Test>("entity/test", testData)
                .AndOk()
                .Post<Test>("entity/test", testData2)
                .AndOk()
                .Get<List<Test>>($"entity/test")
                .IShouldSee<List<Test>>(t => t.Count.Should().Be(2));
        }

        [TestMethod]
        public void GetByIdTest()
        {
            int id = -1;

            ClearAllTables();
            var wp = new WorkflowPacket(URL)
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
