using System.IO;
using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedProperties : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			Props = reader.ReadAssetArray<SerializedProperty>();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_Props", Props.ExportYAML(container));
			return node;
        }

        /*public void Export(TextWriter writer)
		{
			writer.WriteIndent(1);
			writer.Write("Properties {\n");
			foreach(SerializedProperty prop in Props)
			{
				prop.Export(writer);
			}
			writer.WriteIndent(1);
			writer.Write("}\n");
		}*/

        public SerializedProperty[] Props { get; set; }
	}
}
