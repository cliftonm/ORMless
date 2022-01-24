using FieldAliases = System.Collections.Generic.Dictionary<string, string>;

namespace Lib
{
    public class Join
    {
        public enum JoinType
        {
            Inner,
            Left,
            Right,
            Full,
            LeftOuter,
            RightOuter,
            FullOuter,
        }

        public string Table { get; }
        public string TableField { get; }
        public string WithTable { get; }
        public FieldAliases WithTableFields { get; }
        public JoinType Type { get; }

        public Join(string table, string withTable, FieldAliases withTableFields = null, JoinType type = JoinType.Inner, string tableField = null)
        {
            Table = table;
            WithTable = withTable;
            TableField = tableField;
            WithTableFields = withTableFields;
            Type = type;
        }
    }
}
