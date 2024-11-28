using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class EventCameraData : MonoBehaviour
    {
        [YamlMember("baseTime")]
        public float BaseTime { get; set; }

        [YamlMember("timeScale")]
        public float TimeScale { get; set; }

        [YamlMember("length")]
        public int length { get; set; }

        [YamlMember("type")]
        public List<int> Type { get; set; }

        [YamlMember("isEnd")]
        public List<bool> IsEnd { get; set; }

        [YamlMember("startTime")]
        public List<float> StartTime { get; set; }

        [YamlMember("fadeData")]
        public List<FadeData> FadeData { get; set; }

        [YamlMember("pathData")]
        public List<PathData> PathData { get; set; }

        [YamlMember("dofData")]
        public List<DofData> DofData { get; set; }

        [YamlMember("pathData2")]
        public List<PathData2> PathData2 { get; set; }

        [YamlMember("rotationData")]
        public List<RotationData> RotationData { get; set; }

        [YamlMember("returnDefault")]
        public List<ReturnDefault> ReturnDefault { get; set; }

        [YamlMember("returnDefaultRotate")]
        public List<ReturnDefault> ReturnDefaultRotate { get; set; }

        [YamlMember("fovData")]
        public List<FovData> FovData { get; set; }
    }

    public class ReturnDefault
    {
        [YamlMember("curveIndex")]
        public int CurveIndex { get; set; }

        [YamlMember("workTime")]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale")]
        public float WorkTimeScale { get; set; }
    }

    public class RotationData
    {
        [YamlMember("curveIndex")]
        public int CurveIndex { get; set; }

        [YamlMember("workTime")]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale")]
        public float WorkTimeScale { get; set; }

        [YamlMember("isDefaultRotate")]
        public bool IsDefaultRotate { get; set; }

        [YamlMember("Angle1")]
        public Vector3 Angle1 { get; set; }

        [YamlMember("Angle2")]
        public Vector3 Angle2 { get; set; }
    }

    public class PathData2
    {
        [YamlMember("curveIndex")]
        public int CurveIndex { get; set; }

        [YamlMember("workTime")]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale")]
        public float WorkTimeScale { get; set; }

        [YamlMember("vTypeStart")]
        public int VTypeStart { get; set; }

        [YamlMember("vTypeEnd")]
        public int VTypeEnd { get; set; }

        [YamlMember("Pos1")]
        public Vector3 Pos1 { get; set; }

        [YamlMember("Pos2")]
        public Vector3 Pos2 { get; set; }

        [YamlMember("Pos3")]
        public Vector3 Pos3 { get; set; }
    }

    public class PathData
    {
        [YamlMember("workTime")]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale")]
        public float WorkTimeScale { get; set; }

        [YamlMember("vTypeStart")]
        public int VTypeStart { get; set; }

        [YamlMember("vTypeEnd")]
        public int VTypeEnd { get; set; }

        [YamlMember("startPosition")]
        public Vector3 StartPosition { get; set; }

        [YamlMember("Vectol")]
        public Vector3 Vectol { get; set; }

        [YamlMember("endPosition")]
        public Vector3 EndPosition { get; set; }

        [YamlMember("isDefaultRotate")]
        public bool IsDefaultRotate { get; set; }

        [YamlMember("startRotation")]
        public Vector3 StartRotation { get; set; }

        [YamlMember("endRotation")]
        public Vector3 EndRotation { get; set; }
    }

    public class FadeData
    {
        [YamlMember("type")]
        public int Type { get; set; }

        [YamlMember("color")]
        public Color Color { get; set; }

        [YamlMember("duration")]
        public float Duration { get; set; }
    }

    public class DofData
    {
        [YamlMember("workTime")]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale")]
        public float WorkTimeScale { get; set; }

        [YamlMember("use")]
        public bool[] Use { get; set; }

        [YamlMember("typeStart")]
        public int[] TypeStart { get; set; }

        [YamlMember("typeEnd")]
        public int[] TypeEnd { get; set; }

        [YamlMember("valStart")]
        public float[] ValStart { get; set; }

        [YamlMember("valEnd")]
        public float[] ValEnd { get; set; }

        [YamlMember("targetOffset")]
        public Vector3 TargetOffset { get; set; }
    }

    public class FovData
    {
        [YamlMember("curveIndex")]
        public int CurveIndex { get; set; }

        [YamlMember("workTime")]
        public float WorkTime { get; set; }

        [YamlMember("workTimeScale")]
        public float WorkTimeScale { get; set; }

        [YamlMember("field_of_view_start")]
        public float FieldOfViewStart { get; set; }

        [YamlMember("field_of_view")]
        public float FieldOfView { get; set; }

        [YamlMember("is_default_start")]
        public bool IsDefaultStart { get; set; }

        [YamlMember("is_default_end")]
        public bool IsDefaultEnd { get; set; }
    }
}
