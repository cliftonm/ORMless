using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

using Models;

using IntegrationTests.AccountTests;
using IntegrationTests.Models;
using WorkflowTestMethods;

namespace IntegrationTests
{
    [TestClass]
    public class AuditCrudTests : Setup
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

            new WorkflowPacket(URL)
                .Login()
                .Post<Test>("entity/test", testData)
                .AndOk()
                .Get<List<Audit>>("entity/audit")
                .IShouldSee<List<Audit>>(r => r.Count.Should().Be(1))
                .IShouldSee<List<Audit>>(r => r[0].Action.Should().Be(Constants.AUDIT_INSERT));
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
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Then(() => testData.StringField = "test2")
                .Patch<Test>($"entity/test/{id}", testData)
                .AndOk()
                .Get<List<Audit>>("entity/audit")
                .IShouldSee<List<Audit>>(r => r.Count.Should().Be(2))
                .IShouldSee<List<Audit>>(r => r.OrderBy(q => q.Id).First().Action.Should().Be(Constants.AUDIT_INSERT))
                .IShouldSee<List<Audit>>(r => r.OrderBy(q => q.Id).Skip(1).First().Action.Should().Be(Constants.AUDIT_UPDATE));
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
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Delete($"entity/test/{id}")
                .AndNoContent()
                .Get<List<Audit>>("entity/audit")
                .IShouldSee<List<Audit>>(r => r.Count.Should().Be(2))
                .IShouldSee<List<Audit>>(r => r.OrderBy(q => q.Id).First().Action.Should().Be(Constants.AUDIT_INSERT))
                .IShouldSee<List<Audit>>(r => r.OrderBy(q => q.Id).Skip(1).First().Action.Should().Be(Constants.AUDIT_DELETE));
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
                .Then(wp => wp.IGet<Test>(t => id = t.ID))
                .Delete($"entity/test/{id}/Hard")
                .AndNoContent()
                .Get<List<Audit>>("entity/audit")
                .IShouldSee<List<Audit>>(r => r.Count.Should().Be(2))
                .IShouldSee<List<Audit>>(r => r.OrderBy(q => q.Id).First().Action.Should().Be(Constants.AUDIT_INSERT))
                .IShouldSee<List<Audit>>(r => r.OrderBy(q => q.Id).Skip(1).First().Action.Should().Be(Constants.AUDIT_DELETE));
        }
    }
}
