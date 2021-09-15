using System.Globalization;
using System.IO;
using uTinyRipper.Converters;
using uTinyRipper.YAML;

namespace uTinyRipper.Classes.Shaders
{
	public struct SerializedShaderState : IAssetReadable, IYAMLExportable
	{
		/// <summary>
		/// 2017.2 and greater
		/// </summary>
		public static bool HasZClip(Version version) => version.IsGreaterEqual(2017, 2);

		public void Read(AssetReader reader)
		{
			Name = reader.ReadString();
			RtBlend0.Read(reader);
			RtBlend1.Read(reader);
			RtBlend2.Read(reader);
			RtBlend3.Read(reader);
			RtBlend4.Read(reader);
			RtBlend5.Read(reader);
			RtBlend6.Read(reader);
			RtBlend7.Read(reader);
			RtSeparateBlend = reader.ReadBoolean();
			reader.AlignStream();

			if (HasZClip(reader.Version))
			{
				ZClip.Read(reader);
			}
			ZTest.Read(reader);
			ZWrite.Read(reader);
			Culling.Read(reader);
			OffsetFactor.Read(reader);
			OffsetUnits.Read(reader);
			AlphaToMask.Read(reader);
			StencilOp.Read(reader);
			StencilOpFront.Read(reader);
			StencilOpBack.Read(reader);
			StencilReadMask.Read(reader);
			StencilWriteMask.Read(reader);
			StencilRef.Read(reader);
			FogStart.Read(reader);
			FogEnd.Read(reader);
			FogDensity.Read(reader);
			FogColor.Read(reader);

			FogMode = (FogMode)reader.ReadInt32();
			GpuProgramID = reader.ReadInt32();
			Tags.Read(reader);
			LOD = reader.ReadInt32();
			Lighting = reader.ReadBoolean();
			reader.AlignStream();
		}

		/*public void Export(TextWriter writer)
		{
			if (Name != string.Empty)
			{
				writer.WriteIndent(3);
				writer.Write("Name \"{0}\"\n", Name);
			}
			if (LOD != 0)
			{
				writer.WriteIndent(3);
				writer.Write("LOD {0}\n", LOD);
			}
			Tags.Export(writer, 3);
			
			RtBlend0.Export(writer, RtSeparateBlend ? 0 : -1);
			RtBlend1.Export(writer, 1);
			RtBlend2.Export(writer, 2);
			RtBlend3.Export(writer, 3);
			RtBlend4.Export(writer, 4);
			RtBlend5.Export(writer, 5);
			RtBlend6.Export(writer, 6);
			RtBlend7.Export(writer, 7);

			if (AlphaToMaskValue)
			{
				writer.WriteIndent(3);
				writer.Write("AlphaToMask On\n");
			}

			if (!ZClipValue.IsOn())
			{
				writer.WriteIndent(3);
				writer.Write("ZClip {0}\n", ZClipValue);
			}
			if (!ZTestValue.IsLEqual() && !ZTestValue.IsNone())
			{
				writer.WriteIndent(3);
				writer.Write("ZTest {0}\n", ZTestValue);
			}
			if (!ZWriteValue.IsOn())
			{
				writer.WriteIndent(3);
				writer.Write("ZWrite {0}\n", ZWriteValue);
			}
			if (!CullingValue.IsBack())
			{
				writer.WriteIndent(3);
				writer.Write("Cull {0}\n", CullingValue);
			}
			if (!OffsetFactor.IsZero || !OffsetUnits.IsZero)
			{
				writer.WriteIndent(3);
				writer.Write("Offset {0}, {1}\n", OffsetFactor.Val, OffsetUnits.Val);
			}

			if (!StencilRef.IsZero || !StencilReadMask.IsMax || !StencilWriteMask.IsMax || !StencilOp.IsDefault || !StencilOpFront.IsDefault || !StencilOpBack.IsDefault)
			{
				writer.WriteIndent(3);
				writer.Write("Stencil {\n");
				if(!StencilRef.IsZero)
				{
					writer.WriteIndent(4);
					writer.Write("Ref {0}\n", StencilRef.Val);
				}
				if(!StencilReadMask.IsMax)
				{
					writer.WriteIndent(4);
					writer.Write("ReadMask {0}\n", StencilReadMask.Val);
				}
				if(!StencilWriteMask.IsMax)
				{
					writer.WriteIndent(4);
					writer.Write("WriteMask {0}\n", StencilWriteMask.Val);
				}
				if(!StencilOp.IsDefault)
				{
					StencilOp.Export(writer, StencilType.Base);
				}
				if(!StencilOpFront.IsDefault)
				{
					StencilOpFront.Export(writer, StencilType.Front);
				}
				if(!StencilOpBack.IsDefault)
				{
					StencilOpBack.Export(writer, StencilType.Back);
				}
				writer.WriteIndent(3);
				writer.Write("}\n");
			}
			
			if(!FogMode.IsUnknown() || !FogColor.IsZero || !FogDensity.IsZero || !FogStart.IsZero || !FogEnd.IsZero)
			{
				writer.WriteIndent(3);
				writer.Write("Fog {\n");
				if(!FogMode.IsUnknown())
				{
					writer.WriteIndent(4);
					writer.Write("Mode {0}\n", FogMode);
				}
				if (!FogColor.IsZero)
				{
					writer.WriteIndent(4);
					writer.Write("Color ({0},{1},{2},{3})\n",
						FogColor.X.Val.ToString(CultureInfo.InvariantCulture),
						FogColor.Y.Val.ToString(CultureInfo.InvariantCulture),
						FogColor.Z.Val.ToString(CultureInfo.InvariantCulture),
						FogColor.W.Val.ToString(CultureInfo.InvariantCulture));
				}
				if (!FogDensity.IsZero)
				{
					writer.WriteIndent(4);
					writer.Write("Density {0}\n", FogDensity.Val.ToString(CultureInfo.InvariantCulture));
				}
				if (!FogStart.IsZero ||!FogEnd.IsZero)
				{
					writer.WriteIndent(4);
					writer.Write("Range {0}, {1}\n",
						FogStart.Val.ToString(CultureInfo.InvariantCulture),
						FogEnd.Val.ToString(CultureInfo.InvariantCulture));
				}
				writer.WriteIndent(3);
				writer.Write("}\n");
			}

			if(Lighting)
			{
				writer.WriteIndent(3);
				writer.Write("Lighting {0}\n", LightingValue);
			}
			writer.WriteIndent(3);
			writer.Write("GpuProgramID {0}\n", GpuProgramID);
		}*/

        public YAMLNode ExportYAML(IExportContainer container)
        {
			var node = new YAMLMappingNode();
			node.Add("m_Name", Name);
			node.Add("rtBlend0", RtBlend0.ExportYAML(container));
			node.Add("rtBlend1", RtBlend1.ExportYAML(container));
			node.Add("rtBlend2", RtBlend2.ExportYAML(container));
			node.Add("rtBlend3", RtBlend3.ExportYAML(container));
			node.Add("rtBlend4", RtBlend4.ExportYAML(container));
			node.Add("rtBlend5", RtBlend5.ExportYAML(container));
			node.Add("rtBlend6", RtBlend6.ExportYAML(container));
			node.Add("rtBlend7", RtBlend7.ExportYAML(container));
			node.Add("rtSeparateBlend", RtSeparateBlend);
			node.Add("zClip", ZClip.ExportYAML(container));
			node.Add("zTest", ZTest.ExportYAML(container));
			node.Add("zWrite", ZWrite.ExportYAML(container));
			node.Add("culling", Culling.ExportYAML(container));
			//TODO: 2020 or something - node.Add("conservative", Conservative);
			node.Add("offsetFactor", OffsetFactor.ExportYAML(container));
			node.Add("offsetUnits", OffsetUnits.ExportYAML(container));
			node.Add("alphaToMask", AlphaToMask.ExportYAML(container));
			node.Add("stencilOP", StencilOp.ExportYAML(container));
			node.Add("stencilOpFront", StencilOpFront.ExportYAML(container));
			node.Add("stencilOpBack", StencilOpBack.ExportYAML(container));
			node.Add("stencilReadMask", StencilReadMask.ExportYAML(container));
			node.Add("stencilWriteMask", StencilWriteMask.ExportYAML(container));
			node.Add("stencilRef", StencilRef.ExportYAML(container));
			node.Add("fogStart", FogStart.ExportYAML(container));
			node.Add("fogEnd", FogEnd.ExportYAML(container));
			node.Add("fogDensity", FogDensity.ExportYAML(container));
			node.Add("fogColor", FogColor.ExportYAML(container));
			node.Add("fogMode", (int)FogMode);
			node.Add("gpuProgramID", GpuProgramID);
			node.Add("m_Tags", Tags.ExportYAML(container));
			node.Add("m_LOD", LOD);
			node.Add("lighting", Lighting);
			return node;
        }

        public string Name { get; set; }
		public bool RtSeparateBlend { get; set; }
		public FogMode FogMode { get; set; }
		public int GpuProgramID { get; set; }
		public int LOD { get; set; }
		public bool Lighting { get; set; }

		private ZClip ZClipValue => (ZClip)ZClip.Val;
		private ZTest ZTestValue => (ZTest)ZTest.Val;
		private ZWrite ZWriteValue => (ZWrite)ZWrite.Val;
		private Cull CullingValue => (Cull)Culling.Val;
		private bool AlphaToMaskValue => AlphaToMask.Val > 0;
		private string LightingValue => Lighting ? "On" : "Off";

		public SerializedShaderRTBlendState RtBlend0;
		public SerializedShaderRTBlendState RtBlend1;
		public SerializedShaderRTBlendState RtBlend2;
		public SerializedShaderRTBlendState RtBlend3;
		public SerializedShaderRTBlendState RtBlend4;
		public SerializedShaderRTBlendState RtBlend5;
		public SerializedShaderRTBlendState RtBlend6;
		public SerializedShaderRTBlendState RtBlend7;
		public SerializedShaderFloatValue ZClip;
		public SerializedShaderFloatValue ZTest;
		public SerializedShaderFloatValue ZWrite;
		public SerializedShaderFloatValue Culling;
		public SerializedShaderFloatValue OffsetFactor;
		public SerializedShaderFloatValue OffsetUnits;
		public SerializedShaderFloatValue AlphaToMask;
		public SerializedStencilOp StencilOp;
		public SerializedStencilOp StencilOpFront;
		public SerializedStencilOp StencilOpBack;
		public SerializedShaderFloatValue StencilReadMask;
		public SerializedShaderFloatValue StencilWriteMask;
		public SerializedShaderFloatValue StencilRef;
		public SerializedShaderFloatValue FogStart;
		public SerializedShaderFloatValue FogEnd;
		public SerializedShaderFloatValue FogDensity;
		public SerializedShaderVectorValue FogColor;
		public SerializedTagMap Tags;
	}
}
