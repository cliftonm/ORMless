using System;

namespace Clifton
{
    public static class Assertion
    {
        public static void Try<T>(Action action, string msg) where T : Exception, new()
        {
            try
            {
                action();
            }
            catch
            {
                var ex = Activator.CreateInstance(typeof(T), new object[] { msg }) as T;
                throw ex;
            }
        }

        public static void SilentTry(Action action)
        {
            try
            {
                action();
            }
            catch
            {
            }
        }

        public static T BreakOnException<Q, T>(Q item, Func<T> fnc)
        {
            T ret = default;

            try
            {
                ret = fnc();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Diagnostics.Debugger.Break();
            }

            return ret;
        }

        public static void IsTrue(bool cond, string message)
        {
            if (!cond)
            {
                throw new Exception(message);
            }
        }

        public static void That(bool cond, string message)
        {
            if (!cond)
            {
                throw new Exception(message);
            }
        }

        public static void IsNull(object obj, string message)
        {
            if (obj != null)
            {
                throw new Exception(message);
            }
        }

        public static void IsNotNull(object obj, string message)
        {
            if (obj == null)
            {
                throw new Exception(message);
            }
        }

        public static void That<T>(bool condition, string msg) where T : Exception, new()
        {
            if (!condition)
            {
                var ex = Activator.CreateInstance(typeof(T), new object[] { msg }) as T;
                throw ex;
            }
        }

        public static void Null<T>(object obj, string msg) where T : Exception, new()
        {
            if (obj != null)
            {
                var ex = Activator.CreateInstance(typeof(T), new object[] { msg }) as T;
                throw ex;
            }
        }

        public static void NotNull<T>(object obj, string msg) where T : Exception, new()
        {
            if (obj == null)
            {
                var ex = Activator.CreateInstance(typeof(T), new object[] { msg }) as T;
                throw ex;
            }
        }
    }
}
