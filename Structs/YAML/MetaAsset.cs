using System;
using SharpYaml;
using SharpYaml.Serialization;

namespace ImpostersOrdeal
{
    public class MetaAsset
    {
        [YamlMember("fileFormatVersion")]
        public int FileFormatVersion { get; set; }

        [YamlMember("guid")]
        public string GUID { get; set; }

        [YamlMember("timeCreated")]
        public long TimeCreated { get; set; }

        [YamlMember("licenseType")]
        public string LicenseType { get; set; }

        [YamlMember("NativeFormatImporter")]
        public NativeFormatImporter NativeFormatImporter { get; set; }
    }

    public class NativeFormatImporter
    {
        [YamlMember("externalObjects")]
        [YamlStyle(YamlStyle.Flow)]
        public UnityFile ExternalObjects { get; set; }

        [YamlMember("mainObjectFileID")]
        public long MainObjectFileID { get; set; }

        [YamlMember("userData")]
        public string UserData { get; set; }

        [YamlMember("assetBundleName")]
        public string AssetBundleName { get; set; }

        [YamlMember("assetBundleVariant")]
        public string AssetBundleVariant { get; set; }
    }
}
