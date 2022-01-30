using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

// Source: https://github.com/cliftonm/clifton/blob/master/Clifton.Core/Clifton.Core.ExtensionMethods/ExtensionMethods.cs

namespace Clifton
{
    public static class ExtensionMethods
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> collection, Action<T, int> action, int offset = 0)
        {
            int n = offset;

            foreach (var item in collection)
            {
                action(item, n++);
            }
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> collection, Action<T, int> action, Func<T, bool> qualifier, int offset = 0)
        {
            int n = offset;

            foreach (var item in collection)
            {
                if (qualifier(item))
                {
                    action(item, n);
                }

                ++n;
            }
        }

        public static void ForEach(this int count, Action<int> action, int offset = 0)
        {
            int n = offset;

            for (int i = 0; i < count; i++)
            {
                action(i + n);
            }
        }

        public static string UpperCaseFirstLetter(this string str)
        {
            string ret = str;

            if (!string.IsNullOrEmpty(str))
            {

                char[] a = str.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                ret = new string(a);
            }

            return ret;
        }

        public static int ToInt<T>(this T obj)
        {
            int i = Int32.Parse(obj.ToString());

            return i;
        }

        public static char LastChar(this string s)
        {
            var c = s[s.Length - 1];

            return c;
        }

        public static string RemovePunctuation(this string s)
        {
            var chars = s.Where(c => !Char.IsPunctuation(c)).ToArray();
            var str = new string(chars);

            return str;
        }

        public static string HrRefBodyOrString(this string s)
        {
            var ret = s;

            if (s.StartsWith("<a href"))
            {
                ret = s.Between("\">", "</a>");
            }

            return ret;
        }


/// <summary>
/// Left of the first occurance of c.
/// </summary>
/// <param name="src">The source string.</param>
/// <param name="c">Return everything to the left of this character.</param>
/// <returns>String to the left of c, or the entire string.</returns>
public static string LeftOf(this string src, char c)
        {
            string ret = src;
            int idx = src.IndexOf(c);

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

        public static string LeftOf(this String src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

        public static string LeftOf(this String src, int n)
        {
            return src.Length < n ? src : src.Substring(n);
        }

        public static string LeftOfRightmostOf(this String src, char c)
        {
            string ret = src;
            int idx = src.LastIndexOf(c);

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

        public static string LeftOfRightmostOf(this String src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);
            int idx2 = idx;

            while (idx2 != -1)
            {
                idx2 = src.IndexOf(s, idx + s.Length);

                if (idx2 != -1)
                {
                    idx = idx2;
                }
            }

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

        public static string RightOf(this String src, char c)
        {
            string ret = String.Empty;
            int idx = src.IndexOf(c);

            if (idx != -1)
            {
                ret = src.Substring(idx + 1);
            }

            return ret;
        }

        /// <summary>
        /// Returns the portion of a string (src) that is to the right of the first occurance 
        /// of the specified string (s), (i.e. excluding s). Returns an empty string if the 
        /// specified string is not found.
        /// </summary>
        public static string RightOf(this String src, string s, bool caseInsensitive = false)
        {
            string ret = String.Empty;
            
            string src2 = caseInsensitive ? src.ToLower() : src;
            string s2 = caseInsensitive ? s.ToLower() : s;
            
            int idx = src2.IndexOf(s2);

            if (idx != -1)
            {
                ret = src.Substring(idx + s.Length);
            }

            return ret;
        }

        public static string RightOfRightmostOf(this string src, char c)
        {
            string ret = String.Empty;
            int idx = src.LastIndexOf(c);

            if (idx != -1)
            {
                ret = src.Substring(idx + 1);
            }

            return ret;
        }

        public static string RightOfRightmostOf(this String src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);
            int idx2 = idx;

            while (idx2 != -1)
            {
                idx2 = src.IndexOf(s, idx + s.Length);

                if (idx2 != -1)
                {
                    idx = idx2;
                }
            }

            if (idx != -1)
            {
                ret = src.Substring(idx + s.Length, src.Length - (idx + s.Length));
            }

            return ret;
        }

        public static T SerializeTo<T>(this IDictionary<string, object> fieldValues) where T: new()
        {
            var rec = new T();
            Type t = rec.GetType();

            fieldValues.ForEach(kvp =>
            {
                var prop = t.GetProperty(kvp.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                prop?.SetValue(rec, kvp.Value);
            });

            return rec;
        }

        public static List<T> SerializeTo<T>(this List<IDictionary<string, object>> recordList) where T : new()
        {
            var recs = recordList.Select(r => r.SerializeTo<T>()).ToList();
    
            return recs;
        }

        // See Mr.PoorInglish's rework of my article here:
        // https://www.codeproject.com/Articles/5293576/A-Performant-Items-in-List-A-that-are-not-in-List?msg=5782421#xx5782421xx
        public static IEnumerable<T1> In<T1, T2, TKey>(
            this IEnumerable<T1> items1,
            IEnumerable<T2> items2,
            Func<T1, TKey> keySelector1, Func<T2, TKey> keySelector2)
        {
            var dict1 = items1.ToDictionary(keySelector1);
            var k1s = dict1.Keys.Intersect(items2.Select(itm2 => keySelector2(itm2)));
            var notIn = k1s.Select(k1 => dict1[k1]);

            return notIn;
        }

        // See Mr.PoorInglish's rework of my article here:
        // https://www.codeproject.com/Articles/5293576/A-Performant-Items-in-List-A-that-are-not-in-List?msg=5782421#xx5782421xx
        public static IEnumerable<T1> NotIn<T1, T2, TKey>(
            this IEnumerable<T1> items1, 
            IEnumerable<T2> items2,
            Func<T1, TKey> keySelector1, Func<T2, TKey> keySelector2)
        {
            var dict1 = items1.ToDictionary(keySelector1);
            var k1s = dict1.Keys.Except(items2.Select(itm2 => keySelector2(itm2)));
            var notIn = k1s.Select(k1 => dict1[k1]);

            return notIn;
        }

        /// <summary>
        /// Returns everything between the start-character and the first occurence
        /// of the end-character, after the start-character, exclusive.
        /// </summary>
        /// <param name="src">The source string.</param>
        /// <param name="start">The first char to find.</param>
        /// <param name="end">The end char to find.</param>
        /// <returns>The string between the start and end chars, or an empty string if 
        /// the start char or end char is not found.</returns>
        public static string Between(this string src, char start, char end)
        {
            string ret = String.Empty;
            int idxStart = src.IndexOf(start);

            if (idxStart != -1)
            {
                ++idxStart;
                int idxEnd = src.IndexOf(end, idxStart);

                if (idxEnd != -1)
                {
                    ret = src.Substring(idxStart, idxEnd - idxStart);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns everything between the start-string and the first occurence
        /// of the end-string, after the start-string, exclusive.
        /// </summary>
        /// <param name="src">The source string.</param>
        /// <param name="start">The first string to find.</param>
        /// <param name="end">The end string to find.</param>
        /// <returns>The string between the start and end strings, or an empty string if 
        /// the start string or end string is not found.</returns>
        public static string Between(this string src, string start, string end)
        {
            string ret = String.Empty;
            int idxStart = src.IndexOf(start);

            if (idxStart != -1)
            {
                idxStart += start.Length;
                int idxEnd = src.IndexOf(end, idxStart);

                if (idxEnd != -1)
                {
                    ret = src.Substring(idxStart, idxEnd - idxStart);
                }
            }

            return ret;
        }

        public static string Serialize(this IDictionary<string, object> r)
        {
            string ret = null;

            if (r != null)
            {
                ret = JsonConvert.SerializeObject(r);
            }

            return ret;
        }
    }
}
