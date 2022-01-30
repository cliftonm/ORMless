﻿using System;
using System.Collections.Generic;

using FluentAssertions;

using Clifton.IntegrationTestWorkflowEngine;

namespace WorkflowTestMethods
{
    public static class WorkflowMethods
    {
        public static WorkflowPacket UseHeader(this WorkflowPacket wp, string key, string val)
        {
            wp.Headers = new Dictionary<string, string>() { { key, val } };

            return wp;
        }

        public static WorkflowPacket UseHeader(this WorkflowPacket wp, Dictionary<string, string> headers)
        {
            wp.Headers = headers;

            return wp;
        }

        public static WorkflowPacket Break(this WorkflowPacket wp)
        {
            System.Diagnostics.Debugger.Break();

            return wp;
        }

        public static WorkflowPacket IShouldSee<T>(this WorkflowPacket wp, Action<T> test) where T : class
        {
            return wp.IShouldSee(typeof(T).Name, test);
        }

        public static WorkflowPacket IShouldSee<T>(this WorkflowPacket wp, string containerName, Action<T> test) where T : class
        {
            T obj = wp.GetObject<T>(containerName);
            test(obj);

            return wp;
        }

        public static WorkflowPacket IShouldSee(this WorkflowPacket wp, string containerName, Func<dynamic, bool> test)
        {
            var obj = wp.GetObject(containerName);
            bool b = test(obj);
            b.Should().BeTrue();

            return wp;
        }

        public static WorkflowPacket IShouldSee(this WorkflowPacket wp, Func<bool> test)
        {
            bool b = test();
            b.Should().BeTrue();

            return wp;
        }


        public static WorkflowPacket IShouldSee(this WorkflowPacket wp, Action test)
        {
            test();

            return wp;
        }

        public static WorkflowPacket Then(this WorkflowPacket wp, Action action)
        {
            action();

            return wp;
        }

        public static WorkflowPacket Then(this WorkflowPacket wp, Action<WorkflowPacket> action)
        {
            action(wp);

            return wp;
        }

        public static WorkflowPacket IGet<T>(this WorkflowPacket wp, Action<T> getter) where T : class
        {
            return wp.IGet(typeof(T).Name, getter);
        }

        public static WorkflowPacket IGet<T>(this WorkflowPacket wp, string containerName, Action<T> getter) where T : class
        {
            T obj = wp.GetObject<T>(containerName);
            getter(obj);

            return wp;
        }
    }
}
