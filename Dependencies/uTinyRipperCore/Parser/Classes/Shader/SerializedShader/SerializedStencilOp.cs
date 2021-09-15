using System.IO;
using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedStencilOp : IAssetReadable, IYAMLExportable
	{
		public void Read(AssetReader reader)
		{
			Pass.Read(reader);
			Fail.Read(reader);
			ZFail.Read(reader);
			Comp.Read(reader);
		}

		public void Export(TextWriter writer, StencilType type)
		{
			writer.WriteIndent(4);
			writer.Write("Comp{0} {1}\n", type.ToSuffixString(), CompValue);
			writer.WriteIndent(4);
			writer.Write("Pass{0} {1}\n", type.ToSuffixString(), PassValue);
			writer.WriteIndent(4);
			writer.Write("Fail{0} {1}\n", type.ToSuffixString(), FailValue);
			writer.WriteIndent(4);
			writer.Write("ZFail{0} {1}\n", type.ToSuffixString(), ZFailValue);
		}

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("pass", Pass.ExportYAML(container));
			node.Add("fail", Fail.ExportYAML(container));
			node.Add("zFail", ZFail.ExportYAML(container));
			node.Add("comp", Comp.ExportYAML(container));
			return node;
        }

        public bool IsDefault => PassValue.IsKeep() && FailValue.IsKeep() && ZFailValue.IsKeep() && CompValue.IsAlways();

		public SerializedShaderFloatValue Pass;
		public SerializedShaderFloatValue Fail;
		public SerializedShaderFloatValue ZFail;
		public SerializedShaderFloatValue Comp;

		private StencilOp PassValue => (StencilOp)Pass.Val;
		private StencilOp FailValue => (StencilOp)Fail.Val;
		private StencilOp ZFailValue => (StencilOp)ZFail.Val;
		private StencilComp CompValue => (StencilComp)Comp.Val;
	}
}
