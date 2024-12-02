using SharpYaml.Events;
using SharpYaml.Serialization;
using System;
using System.Globalization;

namespace ImpostersOrdeal
{
    public class FloatSerializer : IYamlSerializable
    {
        public bool Accepts(Type type) => type == typeof(float);

        public object ReadYaml(ref ObjectContext ctx)
        {
            if (!float.TryParse(ctx.Reader.Allow<Scalar>().Value, out float value))
                value = 0.0f;

            return value;
        }

        public void WriteYaml(ref ObjectContext ctx)
        {
            ctx.Writer.Emit(new Scalar(((float)ctx.Instance).ToString(CultureInfo.InvariantCulture)));
        }
    }
}
