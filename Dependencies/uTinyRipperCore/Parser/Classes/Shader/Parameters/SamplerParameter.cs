using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SamplerParameter : IAssetReadable, IYAMLExportable
	{
		public SamplerParameter(uint sampler, int bindPoint)
		{
			Sampler = sampler;
			BindPoint = bindPoint;
		}

		public void Read(AssetReader reader)
		{
			Sampler = reader.ReadUInt32();
			BindPoint = reader.ReadInt32();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("sampler", Sampler);
			node.Add("bindPoint", BindPoint);
			return node;
        }

        public uint Sampler { get; set; }
		public int BindPoint { get; set; }
	}
}
