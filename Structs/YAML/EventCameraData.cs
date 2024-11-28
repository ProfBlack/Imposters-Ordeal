using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class EventCameraData : MonoBehaviour
    {
        [YamlMember("baseTime", 10)]
        public float BaseTime { get; set; }

        [YamlMember("timeScale", 11)]
        public float TimeScale { get; set; }

        [YamlMember("length", 12)]
        public int length { get; set; }

        [YamlMember("type", 13)]
        public List<int> Type { get; set; }

        [YamlMember("isEnd", 14)]
        public List<bool> IsEnd { get; set; }

        [YamlMember("startTime", 15)]
        public List<float> StartTime { get; set; }

        [YamlMember("fadeData", 16)]
        public List<FadeData> FadeData { get; set; }

        [YamlMember("pathData", 17)]
        public List<PathData> PathData { get; set; }

        [YamlMember("dofData", 18)]
        public List<DofData> DofData { get; set; }

        [YamlMember("pathData2", 19)]
        public List<PathData2> PathData2 { get; set; }

        [YamlMember("rotationData", 20)]
        public List<RotationData> RotationData { get; set; }

        [YamlMember("returnDefault", 21)]
        public List<ReturnDefault> ReturnDefault { get; set; }

        [YamlMember("returnDefaultRotate", 22)]
        public List<ReturnDefault> ReturnDefaultRotate { get; set; }

        [YamlMember("fovData", 23)]
        public List<FovData> FovData { get; set; }
    }

    public class ReturnDefault
    {
        [YamlMember("curveIndex", 0)]
        public int CurveIndex { get; set; }

        [YamlMember("workTime", 1)]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale", 2)]
        public float WorkTimeScale { get; set; }
    }

    public class RotationData
    {
        [YamlMember("curveIndex", 0)]
        public int CurveIndex { get; set; }

        [YamlMember("workTime", 1)]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale", 2)]
        public float WorkTimeScale { get; set; }

        [YamlMember("isDefaultRotate", 3)]
        public bool IsDefaultRotate { get; set; }

        [YamlMember("Angle1", 4)]
        public Vector3 Angle1 { get; set; }

        [YamlMember("Angle2", 5)]
        public Vector3 Angle2 { get; set; }
    }

    public class PathData2
    {
        [YamlMember("curveIndex", 0)]
        public int CurveIndex { get; set; }

        [YamlMember("workTime", 1)]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale", 2)]
        public float WorkTimeScale { get; set; }

        [YamlMember("vTypeStart", 3)]
        public int VTypeStart { get; set; }

        [YamlMember("vTypeEnd", 4)]
        public int VTypeEnd { get; set; }

        [YamlMember("Pos1", 5)]
        public Vector3 Pos1 { get; set; }

        [YamlMember("Pos2", 6)]
        public Vector3 Pos2 { get; set; }

        [YamlMember("Pos3", 7)]
        public Vector3 Pos3 { get; set; }
    }

    public class PathData
    {
        [YamlMember("workTime", 0)]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale", 1)]
        public float WorkTimeScale { get; set; }

        [YamlMember("vTypeStart", 2)]
        public int VTypeStart { get; set; }

        [YamlMember("vTypeEnd", 3)]
        public int VTypeEnd { get; set; }

        [YamlMember("startPosition", 4)]
        public Vector3 StartPosition { get; set; }

        [YamlMember("Vectol", 5)]
        public Vector3 Vectol { get; set; }

        [YamlMember("endPosition", 6)]
        public Vector3 EndPosition { get; set; }

        [YamlMember("isDefaultRotate", 7)]
        public bool IsDefaultRotate { get; set; }

        [YamlMember("startRotation", 8)]
        public Vector3 StartRotation { get; set; }

        [YamlMember("endRotation", 9)]
        public Vector3 EndRotation { get; set; }
    }

    public class FadeData
    {
        [YamlMember("type", 0)]
        public int Type { get; set; }

        [YamlMember("color", 1)]
        public Color Color { get; set; }

        [YamlMember("duration", 2)]
        public float Duration { get; set; }
    }

    public class DofData
    {
        [YamlMember("workTime", 0)]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale", 1)]
        public float WorkTimeScale { get; set; }

        [YamlMember("use", 2)]
        public bool[] Use { get; set; }

        [YamlMember("typeStart", 3)]
        public int[] TypeStart { get; set; }

        [YamlMember("typeEnd", 4)]
        public int[] TypeEnd { get; set; }

        [YamlMember("valStart", 5)]
        public float[] ValStart { get; set; }

        [YamlMember("valEnd", 6)]
        public float[] ValEnd { get; set; }

        [YamlMember("targetOffset", 7)]
        public Vector3 TargetOffset { get; set; }
    }

    public class FovData
    {
        [YamlMember("curveIndex", 0)]
        public int CurveIndex { get; set; }

        [YamlMember("workTime", 1)]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale", 2)]
        public float WorkTimeScale { get; set; }

        [YamlMember("field_of_view_start", 3)]
        public float FieldOfViewStart { get; set; }

        [YamlMember("field_of_view", 4)]
        public float FieldOfView { get; set; }

        [YamlMember("is_default_start", 5)]
        public bool IsDefaultStart { get; set; }

        [YamlMember("is_default_end", 6)]
        public bool IsDefaultEnd { get; set; }
    }
}
