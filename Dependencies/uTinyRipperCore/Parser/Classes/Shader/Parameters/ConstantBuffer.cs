using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct ConstantBuffer : IAssetReadable, IYAMLExportable
	{
		/// <summary>
		/// 2017.3 and greater
		/// </summary>
		public static bool HasStructParams(Version version) => version.IsGreaterEqual(2017, 3);

		public ConstantBuffer(string name, MatrixParameter[] matrices, VectorParameter[] vectors, StructParameter[] structs, int usedSize)
		{
			Name = name;
			NameIndex = -1;
			MatrixParams = matrices;
			VectorParams = vectors;
			StructParams = structs;
			Size = usedSize;
		}

		public void Read(AssetReader reader)
		{
			NameIndex = reader.ReadInt32();
			MatrixParams = reader.ReadAssetArray<MatrixParameter>();
			VectorParams = reader.ReadAssetArray<VectorParameter>();
			if (HasStructParams(reader.Version))
			{
				StructParams = reader.ReadAssetArray<StructParameter>();
			}
			Size = reader.ReadInt32();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_NameIndex", NameIndex);
			node.Add("m_MatrixParams", MatrixParams.ExportYAML(container));
			node.Add("m_VectorParams", VectorParams.ExportYAML(container));
			if (HasStructParams(container.Version))
            {
				node.Add("m_StructParams", StructParams.ExportYAML(container));
            }
			node.Add("m_Size", Size);
			return node;
        }

        public string Name { get; set; }
		public int NameIndex { get; set; }
		public MatrixParameter[] MatrixParams { get; set; }
		public VectorParameter[] VectorParams { get; set; }
		public StructParameter[] StructParams { get; set; }
		public int Size { get; set; }
	}
}
