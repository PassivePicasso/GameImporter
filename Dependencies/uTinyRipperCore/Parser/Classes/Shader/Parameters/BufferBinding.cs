using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct BufferBinding : IAssetReadable, IYAMLExportable
	{
		public BufferBinding(string name, int index)
		{
			Name = name;
			NameIndex = -1;
			Index = index;
			ArraySize = 0;
		}

		/// <summary>
		/// Greater or equal 2019.3
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public static bool HasArraySize(Version version) => version.IsGreaterEqual(2019, 3);

		public void Read(AssetReader reader)
		{
			NameIndex = reader.ReadInt32();
			Index = reader.ReadInt32();
			if (HasArraySize(reader.Version))
            {
				ArraySize = reader.ReadInt32();
            }
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_NameIndex", NameIndex);
			node.Add("m_Index", Index);
			if (HasArraySize(container.Version))
			{
				node.Add("m_ArraySize", ArraySize);
			}
			return node;
		}

        public string Name { get; set; }
		public int NameIndex { get; set; }
		public int Index { get; set; }
		public int ArraySize { get; set; }
	}
}
