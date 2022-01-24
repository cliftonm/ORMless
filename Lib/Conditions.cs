using System.Collections.Generic;
using System.Text;

using Parameters = System.Collections.Generic.Dictionary<string, object>;

namespace Lib
{
    public class Conditions : List<Condition>
    {
        private static Dictionary<Condition.Op, string> opTemplate = new Dictionary<Condition.Op, string>()
        {
            {Condition.Op.Equals, "[Field] = [FieldParam]" },
            {Condition.Op.NotEqual, "[Field] != [FieldParam]" },
            {Condition.Op.IsNull, "[Field] is null" },
            {Condition.Op.IsNotNull, "[Field] is not null" },
            {Condition.Op.Like, "[Field] like [FieldParam]" },
        };

        public void AddConditions(StringBuilder sb, Parameters parms)
        {
            if (Count > 0)
            {
                List<string> conditions = new List<string>();

                ForEach(c =>
                {
                    var parmName = $"p{c._Field.Replace(".", "")}";
                    var template = opTemplate[c.Operation];

                    template = template
                        .Replace("[Field]", c._Field)
                        .Replace("[FieldParam]", $"@{parmName}");

                    // Add the parameter regardless of whether we use it or not.
                    parms[parmName] = c.Value;

                    conditions.Add(template);
                });

                var anders = string.Join(" and ", conditions);
                sb.Append($" and {anders}");
            }
        }

        public static Condition Where()
        {
            var conditions = new Conditions();
            var condition = new Condition(conditions);
            conditions.Add(condition);

            return condition;
        }

        public Condition And()
        {
            var condition = new Condition(this) { _AndOr = Condition.AndOr.And };
            Add(condition);

            return condition;
        }

        public Condition Or()
        {
            var condition = new Condition(this) { _AndOr = Condition.AndOr.Or };
            Add(condition);

            return condition;
        }
    }
}
