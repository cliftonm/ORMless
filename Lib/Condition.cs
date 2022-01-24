using System.Collections.Generic;

namespace Lib
{
    public class Condition
    {
        public enum Op
        {
            Equals,
            NotEqual,
            IsNull,
            IsNotNull,
            Like,
        }

        // TODO: Ability to group conditions into sets of and/ors.
        public enum AndOr
        {
            And,
            Or,
        }

        public string _Field { get; set; }
        public Op Operation { get; set; }
        public AndOr _AndOr { get; set; }
        public object Value { get; set; }

        public string Comparitor => opTemplate[Operation];
        public string Combinator => combinatorTemplate[_AndOr];

        private static Dictionary<Op, string> opTemplate = new Dictionary<Op, string>()
        {
            {Op.Equals, "=" },
            {Op.NotEqual, "!=" },
            {Op.IsNull, "is null" },
            {Op.IsNotNull, "is not null" },
            {Op.Like, "like" },
        };

        private static Dictionary<AndOr, string> combinatorTemplate = new Dictionary<AndOr, string>()
        {
            {AndOr.And, "and" },
            {AndOr.Or, "or" },
        };

        private Conditions conditions;

        public Condition()
        {
        }

        public Condition(Conditions conditions)
        {
            this.conditions = conditions;
        }

        public Condition(string field, Op operation)
        {
            _Field = field;
            Operation = operation;
        }

        public Condition(string field, Op operation, object val)
        {
            _Field = field;
            Operation = operation;
            Value = val;
        }

        public Condition Field(string field)
        {
            _Field = field;

            return this;
        }

        public Conditions Is(object val)
        {
            Operation = Op.Equals;
            Value = val;

            return conditions;
        }

        public Conditions IsLike(object val)
        {
            Operation = Op.Like;
            Value = val;

            return conditions;
        }

        public Conditions IsNot(object val)
        {
            Operation = Op.NotEqual;
            Value = val;

            return conditions;
        }

        public string FixField(string f, string size)
        {
            var ret = $"cast({f} as nvarchar({size}))";

            return ret;
        }
    }
}
