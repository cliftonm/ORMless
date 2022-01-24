using System;
using System.Collections.Generic;
using System.Linq;

using Clifton;

namespace Lib
{
    public class Joins : List<Join>
    {
        private static Dictionary<Join.JoinType, string> joinTemplate = new Dictionary<Join.JoinType, string>()
        {
            {Join.JoinType.Inner, "JOIN" },
            {Join.JoinType.Left, "LEFT JOIN" },
            {Join.JoinType.Right, "RIGHT JOIN" },
            {Join.JoinType.Full, "FULL JOIN" },
            {Join.JoinType.LeftOuter, "LEFT OUTER JOIN" },
            {Join.JoinType.RightOuter, "RIGHT OUTER JOIN" },
            {Join.JoinType.FullOuter, "FULL OUTER JOIN" },
        };

        public string GetJoins()
        {
            string ret = String.Empty;

            if (Count > 0)
            {
                List<string> joins = new List<string>();
                var joinAliases = GetJoinAliases();

                this.ForEachWithIndex((j, idx) =>
                {
                    // Use the override if it exists, otherwise the default is the "with" table name + ID
                    var tableFieldName = j.TableField ?? $"_{j.WithTable}ID";
                    var alias = joinAliases[idx].alias;
                    var joinType = joinTemplate[j.Type];

                    // Find the join table alias.
                    // Currently this functionality is limited to one table with which we join other tables.
                    // To fix this, the dictionary needs to have some "key" (such as the qualifier) that determines which of multiple joins
                    // to continue joining with.  Too complicated to set up right now.

                    var joinTableAliases = joinAliases.Where(a => a.Value.table == j.Table).ToList();
                    var joinTableAlias = j.Table;

                    Assertion.IsTrue(joinTableAliases.Count <= 1, $"Unsupported: Cannot join multiple instances of {j.Table} with other joins.");

                    if (joinTableAliases.Count == 1)
                    {
                        joinTableAlias = joinTableAliases[0].Value.alias;
                    }

                    var join = $"{joinType} {j.WithTable} {alias} on {alias}.{Constants.ID} = {joinTableAlias}.{tableFieldName} and {alias}.{Constants.DELETED} = 0";
                    joins.Add(join);
                });

                ret = String.Join(Constants.CRLF, joins);
            }

            return ret;
        }

        public string GetJoinFields(string prepend = "")
        {
            string ret = String.Empty;

            if (Count > 0)
            {
                List<string> joinFields = new List<string>();
                var joinAliases = GetJoinAliases();

                this.ForEachWithIndex((j, idx) =>
                {
                    if (j.WithTableFields != null)
                    {
                        var joinTableAlias = joinAliases[idx].alias;

                        j.WithTableFields.ForEach(f =>
                        {
                            joinFields.Add($"{joinTableAlias}.{f.Key} {f.Value}");
                        });
                    }
                });

                if (joinFields.Count > 0)
                {
                    ret = prepend + String.Join(",", joinFields);
                }
            }

            return ret;
        }

        private Dictionary<int, (string alias, string table)> GetJoinAliases()
        {
            Dictionary<int, (string alias, string table)> joinAliases = new Dictionary<int, (string alias, string table)>();

            this.ForEachWithIndex((j, idx) =>
            {
                var alias = $"{j.WithTable}{idx}";

                // If we have an alias for the WithTable because it's part of a join, use it.
                joinAliases.TryGetValue(idx, out (string alias, string table) tableAlias);
                tableAlias.alias ??= alias;
                tableAlias.table ??= j.WithTable;

                joinAliases[idx] = tableAlias;
            });

            return joinAliases;
        }
    }
}
