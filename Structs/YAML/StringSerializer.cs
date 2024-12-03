using SharpYaml.Events;
using SharpYaml.Serialization;
using System;

namespace ImpostersOrdeal
{
    public class StringSerializer : IYamlSerializable
    {
        public bool Accepts(Type type) => type == typeof(string);

        public object ReadYaml(ref ObjectContext ctx)
        {
            return ctx.Reader.Allow<Scalar>().Value;
        }

        public void WriteYaml(ref ObjectContext ctx)
        {
            var value = (string)ctx.Instance;

            if (value.StartsWith("\'"))
            {
                value = value.Replace("\'", "\'\'");
                value = "\'" + value + "\'";
            }

            ctx.Writer.Emit(new Scalar(value));
        }
    }
}
