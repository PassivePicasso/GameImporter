using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct ShaderBindChannel : IAssetReadable, IYAMLExportable
	{
		public ShaderBindChannel(uint source, VertexComponent target)
		{
			Source = (byte)source;
			Target = target;
		}

		public void Read(AssetReader reader)
		{
			Source = reader.ReadByte();
			Target = (VertexComponent)reader.ReadByte();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("source", Source);
			node.Add("target", (byte)Target);
			return node;
        }

        /// <summary>
        /// ShaderChannel enum
        /// </summary>
        public byte Source { get; set; }
		public VertexComponent Target { get; set; }
	}
}
