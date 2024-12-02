using SharpYaml.Events;
using System;
using SharpYaml.Serialization;
using System.Globalization;

namespace ImpostersOrdeal
{
    public class UnityFileSerializer : IYamlSerializable
    {
        public bool Accepts(Type type) => type == typeof(UnityFile);

        public object ReadYaml(ref ObjectContext ctx)
        {
            ctx.Reader.Allow<MappingStart>();

            var file = new UnityFile();

            while (ctx.Reader.Peek<MappingEnd>() == null)
            {
                var scalar = ctx.Reader.Allow<Scalar>();
                switch (scalar.Value)
                {
                    case "fileID":
                        if (int.TryParse(ctx.Reader.Allow<Scalar>().Value, out int fileID))
                            file.FileID = fileID;
                        else
                            file.FileID = 0;
                        break;

                    case "guid":
                        if (Guid.TryParse(ctx.Reader.Allow<Scalar>().Value, out Guid guid))
                            file.GUID = guid;
                        else
                            file.GUID = Guid.Empty;
                        break;

                    case "type":
                        if (int.TryParse(ctx.Reader.Allow<Scalar>().Value, out int type))
                            file.Type = type;
                        else
                            file.Type = 0;
                        break;
                }
            }

            ctx.Reader.Allow<MappingEnd>();

            return file;
        }

        public void WriteYaml(ref ObjectContext ctx)
        {
            ctx.Writer.Emit(new MappingStartEventInfo(ctx.Instance, ctx.Instance.GetType()) { Style = ctx.Style });

            var file = ctx.Instance as UnityFile;
            
            ctx.Writer.Emit(new Scalar("fileID") );
            ctx.Writer.Emit(new Scalar(file.FileID.ToString(CultureInfo.InvariantCulture)));

            if (file.GUID != Guid.Empty)
            {
                ctx.Writer.Emit(new Scalar("guid"));
                ctx.Writer.Emit(new Scalar(file.GUID.ToString("N")));
            }

            if (file.Type != 0)
            {
                ctx.Writer.Emit(new Scalar("type"));
                ctx.Writer.Emit(new Scalar(file.Type.ToString(CultureInfo.InvariantCulture)));
            }

            ctx.Writer.Emit(new MappingEndEventInfo(ctx.Instance, ctx.Instance.GetType()));
        }
    }
}
