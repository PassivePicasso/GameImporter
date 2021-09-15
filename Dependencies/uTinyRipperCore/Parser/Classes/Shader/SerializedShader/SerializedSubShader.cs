using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedSubShader : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			Passes = reader.ReadAssetArray<SerializedPass>();
			Tags.Read(reader);
			LOD = reader.ReadInt32();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_Passes", Passes.ExportYAML(container));
			node.Add("m_Tags", Tags.ExportYAML(container));
			node.Add("m_LOD", LOD);
			return node;
        }

        /*public void Export(ShaderWriter writer)
		{
			writer.WriteIndent(1);
			writer.Write("SubShader {\n");
			if(LOD != 0)
			{
				writer.WriteIndent(2);
				writer.Write("LOD {0}\n", LOD);
			}
			Tags.Export(writer, 2);
			for (int i = 0; i < Passes.Length; i++)
			{
				Passes[i].Export(writer);
			}
			writer.WriteIndent(1);
			writer.Write("}\n");
		}*/

        public SerializedPass[] Passes { get; set; }
		public int LOD { get; set; }

		public SerializedTagMap Tags;
	}
}
