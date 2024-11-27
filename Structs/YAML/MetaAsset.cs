using System;
using YamlDotNet.Serialization;

namespace ImpostersOrdeal
{
    public class MetaAsset
    {
        [YamlMember(Alias = "fileFormatVersion", ApplyNamingConventions = false)]
        public int FileFormatVersion { get; set; }

        [YamlMember(Alias = "guid", ApplyNamingConventions = false)]
        public Guid GUID { get; set; }

        [YamlMember(Alias = "timeCreated", ApplyNamingConventions = false)]
        public long TimeCreated { get; set; }

        [YamlMember(Alias = "licenseType", ApplyNamingConventions = false)]
        public string LicenseType { get; set; }

        [YamlMember(Alias = "NativeFormatImporter", ApplyNamingConventions = false)]
        public NativeFormatImporter NativeFormatImporter { get; set; }
    }

    public class NativeFormatImporter
    {
        [YamlMember(Alias = "externalObjects", ApplyNamingConventions = false)]
        public UnityFile ExternalObjects { get; set; }

        [YamlMember(Alias = "mainObjectFileID", ApplyNamingConventions = false)]
        public long MainObjectFileID { get; set; }

        [YamlMember(Alias = "userData", ApplyNamingConventions = false)]
        public string UserData { get; set; }

        [YamlMember(Alias = "assetBundleName", ApplyNamingConventions = false)]
        public string AssetBundleName { get; set; }

        [YamlMember(Alias = "assetBundleVariant", ApplyNamingConventions = false)]
        public string AssetBundleVariant { get; set; }
    }
}
