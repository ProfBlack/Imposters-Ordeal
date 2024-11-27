using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using YamlDotNet.Serialization;

namespace ImpostersOrdeal
{
    public class EventCameraData : MonoBehaviour
    {
        [YamlMember(Alias = "baseTime", ApplyNamingConventions = false)]
        public float BaseTime { get; set; }

        [YamlMember(Alias = "timeScale", ApplyNamingConventions = false)]
        public float TimeScale { get; set; }

        [YamlMember(Alias = "length", ApplyNamingConventions = false)]
        public int length { get; set; }

        [YamlMember(Alias = "type", ApplyNamingConventions = false)]
        public List<int> Type { get; set; }

        [YamlMember(Alias = "isEnd", ApplyNamingConventions = false)]
        public List<bool> IsEnd { get; set; }

        [YamlMember(Alias = "startTime", ApplyNamingConventions = false)]
        public List<float> StartTime { get; set; }

        [YamlMember(Alias = "fadeData", ApplyNamingConventions = false)]
        public List<FadeData> FadeData { get; set; }

        [YamlMember(Alias = "pathData", ApplyNamingConventions = false)]
        public List<PathData> PathData { get; set; }

        [YamlMember(Alias = "dofData", ApplyNamingConventions = false)]
        public List<DofData> DofData { get; set; }

        [YamlMember(Alias = "pathData2", ApplyNamingConventions = false)]
        public List<PathData2> PathData2 { get; set; }

        [YamlMember(Alias = "rotationData", ApplyNamingConventions = false)]
        public List<RotationData> RotationData { get; set; }

        [YamlMember(Alias = "returnDefault", ApplyNamingConventions = false)]
        public List<ReturnDefault> ReturnDefault { get; set; }

        [YamlMember(Alias = "returnDefaultRotate", ApplyNamingConventions = false)]
        public List<ReturnDefault> ReturnDefaultRotate { get; set; }

        [YamlMember(Alias = "fovData", ApplyNamingConventions = false)]
        public List<FovData> FovData { get; set; }
    }

    public class ReturnDefault
    {
        [YamlMember(Alias = "curveIndex", ApplyNamingConventions = false)]
        public int CurveIndex { get; set; }

        [YamlMember(Alias = "workTime", ApplyNamingConventions = false)]
        public float WorkTime { get; set; }

        [YamlMember(Alias = "workTimeScale", ApplyNamingConventions = false)]
        public float WorkTimeScale { get; set; }
    }

    public class RotationData
    {
        [YamlMember(Alias = "curveIndex", ApplyNamingConventions = false)]
        public int CurveIndex { get; set; }

        [YamlMember(Alias = "workTime", ApplyNamingConventions = false)]
        public float WorkTime { get; set; }

        [YamlMember(Alias = "workTimeScale", ApplyNamingConventions = false)]
        public float WorkTimeScale { get; set; }

        [YamlMember(Alias = "isDefaultRotate", ApplyNamingConventions = false)]
        public bool IsDefaultRotate { get; set; }

        [YamlMember(Alias = "Angle1", ApplyNamingConventions = false)]
        public Vector3 Angle1 { get; set; }

        [YamlMember(Alias = "Angle2", ApplyNamingConventions = false)]
        public Vector3 Angle2 { get; set; }
    }

    public class PathData2
    {
        [YamlMember(Alias = "curveIndex", ApplyNamingConventions = false)]
        public int CurveIndex { get; set; }

        [YamlMember(Alias = "workTime", ApplyNamingConventions = false)]
        public float WorkTime { get; set; }

        [YamlMember(Alias = "workTimeScale", ApplyNamingConventions = false)]
        public float WorkTimeScale { get; set; }

        [YamlMember(Alias = "vTypeStart", ApplyNamingConventions = false)]
        public int VTypeStart { get; set; }

        [YamlMember(Alias = "vTypeEnd", ApplyNamingConventions = false)]
        public int VTypeEnd { get; set; }

        [YamlMember(Alias = "Pos1", ApplyNamingConventions = false)]
        public Vector3 Pos1 { get; set; }

        [YamlMember(Alias = "Pos2", ApplyNamingConventions = false)]
        public Vector3 Pos2 { get; set; }

        [YamlMember(Alias = "Pos3", ApplyNamingConventions = false)]
        public Vector3 Pos3 { get; set; }
    }

    public class PathData
    {
        [YamlMember(Alias = "workTime", ApplyNamingConventions = false)]
        public float WorkTime { get; set; }

        [YamlMember(Alias = "workTimeScale", ApplyNamingConventions = false)]
        public float WorkTimeScale { get; set; }

        [YamlMember(Alias = "vTypeStart", ApplyNamingConventions = false)]
        public int VTypeStart { get; set; }

        [YamlMember(Alias = "vTypeEnd", ApplyNamingConventions = false)]
        public int VTypeEnd { get; set; }

        [YamlMember(Alias = "startPosition", ApplyNamingConventions = false)]
        public Vector3 StartPosition { get; set; }

        [YamlMember(Alias = "Vectol", ApplyNamingConventions = false)]
        public Vector3 Vectol { get; set; }

        [YamlMember(Alias = "endPosition", ApplyNamingConventions = false)]
        public Vector3 EndPosition { get; set; }

        [YamlMember(Alias = "isDefaultRotate", ApplyNamingConventions = false)]
        public bool IsDefaultRotate { get; set; }

        [YamlMember(Alias = "startRotation", ApplyNamingConventions = false)]
        public Vector3 StartRotation { get; set; }

        [YamlMember(Alias = "endRotation", ApplyNamingConventions = false)]
        public Vector3 EndRotation { get; set; }
    }

    public class FadeData
    {
        [YamlMember(Alias = "type", ApplyNamingConventions = false)]
        public int Type { get; set; }

        [YamlMember(Alias = "color", ApplyNamingConventions = false)]
        public Color Color { get; set; }

        [YamlMember(Alias = "duration", ApplyNamingConventions = false)]
        public float Duration { get; set; }
    }

    public class DofData
    {
        [YamlMember(Alias = "workTime", ApplyNamingConventions = false)]
        public float WorkTime { get; set; }

        [YamlMember(Alias = "workTimeScale", ApplyNamingConventions = false)]
        public float WorkTimeScale { get; set; }

        [YamlMember(Alias = "use", ApplyNamingConventions = false)]
        public bool[] Use { get; set; }

        [YamlMember(Alias = "typeStart", ApplyNamingConventions = false)]
        public int[] TypeStart { get; set; }

        [YamlMember(Alias = "typeEnd", ApplyNamingConventions = false)]
        public int[] TypeEnd { get; set; }

        [YamlMember(Alias = "valStart", ApplyNamingConventions = false)]
        public float[] ValStart { get; set; }

        [YamlMember(Alias = "valEnd", ApplyNamingConventions = false)]
        public float[] ValEnd { get; set; }

        [YamlMember(Alias = "targetOffset", ApplyNamingConventions = false)]
        public Vector3 TargetOffset { get; set; }
    }

    public class FovData
    {
        [YamlMember(Alias = "curveIndex", ApplyNamingConventions = false)]
        public int CurveIndex { get; set; }

        [YamlMember(Alias = "workTime", ApplyNamingConventions = false)]
        public float WorkTime { get; set; }

        [YamlMember(Alias = "workTimeScale", ApplyNamingConventions = false)]
        public float WorkTimeScale { get; set; }

        [YamlMember(Alias = "field_of_view_start", ApplyNamingConventions = false)]
        public float FieldOfViewStart { get; set; }

        [YamlMember(Alias = "field_of_view", ApplyNamingConventions = false)]
        public float FieldOfView { get; set; }

        [YamlMember(Alias = "is_default_start", ApplyNamingConventions = false)]
        public bool IsDefaultStart { get; set; }

        [YamlMember(Alias = "is_default_end", ApplyNamingConventions = false)]
        public bool IsDefaultEnd { get; set; }
    }
}
