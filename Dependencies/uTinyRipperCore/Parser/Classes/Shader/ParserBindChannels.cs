using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct ParserBindChannels : IAssetReadable, IYAMLExportable
	{
		public ParserBindChannels(ShaderBindChannel[] channels, int sourceMap)
		{
			Channels = channels;
			SourceMap = sourceMap;
		}

		public void Read(AssetReader reader)
		{
			Channels = reader.ReadAssetArray<ShaderBindChannel>();
			reader.AlignStream();
			SourceMap = reader.ReadInt32();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_Channels", Channels.ExportYAML(container));
			node.Add("m_SourceMap", SourceMap);
			return node;
        }

        public ShaderBindChannel[] Channels { get; set; }
		public int SourceMap { get; set; }
	}
}
