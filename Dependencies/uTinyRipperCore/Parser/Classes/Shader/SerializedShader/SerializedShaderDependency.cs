using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedShaderDependency : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			From = reader.ReadString();
			To = reader.ReadString();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("from", From);
			node.Add("to", To);
			return node;
        }

        public string From { get; set; }
		public string To { get; set; }
	}
}
