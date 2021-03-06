using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedProgram : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			SubPrograms = reader.ReadAssetArray<SerializedSubProgram>();
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_SubPrograms", SubPrograms.ExportYAML(container));
			return node;
        }

        /*public void Export(ShaderWriter writer, ShaderType type)
		{
			if (SubPrograms.Length == 0)
			{
				return;
			}

			writer.WriteIndent(3);
			writer.Write("Program \"{0}\" {{\n", type.ToProgramTypeString());
			int tierCount = GetTierCount();
			for (int i = 0; i < SubPrograms.Length; i++)
			{
				SubPrograms[i].Export(writer, type, tierCount > 1);
			}
			writer.WriteIndent(3);
			writer.Write("}\n");
		}

		private int GetTierCount()
		{
			int tierCount = 1;
			int tier = SubPrograms[0].ShaderHardwareTier;
			for (int i = 1; i < SubPrograms.Length; i++)
			{
				if (SubPrograms[i].ShaderHardwareTier <= tier)
				{
					break;
				}

				tierCount++;
			}
			return tierCount;
		}*/

        public SerializedSubProgram[] SubPrograms { get; set; }
	}
}
