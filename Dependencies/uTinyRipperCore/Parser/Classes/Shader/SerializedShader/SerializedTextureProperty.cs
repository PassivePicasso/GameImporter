using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedTextureProperty : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			DefaultName = reader.ReadString();
			TexDim = reader.ReadInt32();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_DefaultName", DefaultName);
			node.Add("m_TexDim", TexDim);
			return node;
        }

        public string DefaultName { get; set; }
		public int TexDim { get; set; }
	}
}
