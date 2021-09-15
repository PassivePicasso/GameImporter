using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedShaderFloatValue : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			Val = reader.ReadSingle();
			Name = reader.ReadString();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("val", Val);
			node.Add("name", Name);
			return node;
        }

        public bool IsZero => Val == 0.0f;
		public bool IsMax => Val == 255.0f;

		public float Val { get; set; }
		public string Name { get; set; }
	}
}
