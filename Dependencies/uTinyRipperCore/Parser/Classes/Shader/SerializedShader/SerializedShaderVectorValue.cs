using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedShaderVectorValue : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			X.Read(reader);
			Y.Read(reader);
			Z.Read(reader);
			W.Read(reader);
			Name = reader.ReadString();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("x", X.ExportYAML(container));
			node.Add("y", Y.ExportYAML(container));
			node.Add("z", Z.ExportYAML(container));
			node.Add("w", W.ExportYAML(container));
			node.Add("name", Name);
			return node;
        }

        public bool IsZero => X.IsZero && Y.IsZero && Z.IsZero && W.IsZero;

		public string Name { get; set; }

		public SerializedShaderFloatValue X;
		public SerializedShaderFloatValue Y;
		public SerializedShaderFloatValue Z;
		public SerializedShaderFloatValue W;
	}
}
