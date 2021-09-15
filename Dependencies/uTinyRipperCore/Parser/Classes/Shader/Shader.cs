using System;
using System.Collections.Generic;
using System.IO;
using uTinyRipper.Classes.Materials;
using uTinyRipper.Classes.Shaders;
using System.Text;
using uTinyRipper.YAML;
using uTinyRipper.Converters;
using uTinyRipper.Converters.Shaders;
using uTinyRipper.Layout;
using System.Linq;

namespace uTinyRipper.Classes
{
	public sealed class Shader : NamedObject
	{
		public Shader(AssetInfo assetInfo) :
			base(assetInfo)
		{
		}

		public static int ToSerializedVersion(Version version)
		{
			// double blob arrays (offsets, compressedLengths and decompressedLengths)
			if (version.IsGreaterEqual(2019, 3))
			{
				return 2;
			}
			return 1;
		}

		/// <summary>
		/// 5.5.0 and greater
		/// </summary>
		public static bool IsSerialized(Version version) => version.IsGreaterEqual(5, 5);
		/// <summary>
		/// 5.3.0 and greater
		/// </summary>
		public static bool HasBlob(Version version) => version.IsGreaterEqual(5, 3);
		/// <summary>
		/// Less than 2.0.0
		/// </summary>
		public static bool HasFallback(Version version) => version.IsLess(2);
		/// <summary>
		/// Less than 3.2.0
		/// </summary>
		public static bool HasDefaultProperties(Version version) => version.IsLess(3, 2);
		/// <summary>
		/// 2.0.0 to 3.0.0 exclusive
		/// </summary>
		public static bool HasStaticProperties(Version version) => version.IsGreaterEqual(2) && version.IsLess(3);
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool HasDependencies(Version version) => version.IsGreaterEqual(4);
		/// <summary>
		/// 2018.1 and greater
		/// </summary>
		public static bool HasNonModifiableTextures(Version version) => version.IsGreaterEqual(2018);
		/// <summary>
		/// 4.0.0 and greater
		/// </summary>
		public static bool HasShaderIsBaked(Version version) => version.IsGreaterEqual(4);
		/// <summary>
		/// 3.4.0 to 5.5.0 exclusive and Not Release
		/// </summary>
		public static bool HasErrors(Version version, TransferInstructionFlags flags)
		{
			return !flags.IsRelease() && version.IsGreaterEqual(3, 4) && version.IsLess(5, 5);
		}
		/// <summary>
		/// 4.2.0 and greater and Not Release
		/// </summary>
		public static bool HasDefaultTextures(Version version, TransferInstructionFlags flags) => !flags.IsRelease() && version.IsGreaterEqual(4, 2);
		/// <summary>
		/// 4.5.0 and greater and Not Release and Not Buildin
		/// </summary>
		public static bool HasCompileInfo(Version version, TransferInstructionFlags flags)
		{
			return !flags.IsRelease() && !flags.IsBuiltinResources() && version.IsGreaterEqual(4, 5);
		}

		/// <summary>
		/// 2019.3 and greater
		/// </summary>
		private static bool IsDoubleArray(Version version) => version.IsGreaterEqual(2019, 3);

		public override void Read(AssetReader reader)
		{
			if (IsSerialized(reader.Version))
			{
				base.Read(reader);

				ParsedForm.Read(reader);
				Platforms = reader.ReadArray((t) => (GPUPlatform)t);
				if (IsDoubleArray(reader.Version))
				{
					Offsets2D = reader.ReadUInt32ArrayArray();
					CompressedLengths2D = reader.ReadUInt32ArrayArray();
					DecompressedLengths2D = reader.ReadUInt32ArrayArray();
					CompressedBlob = reader.ReadByteArray();
					reader.AlignStream();

					//UnpackSubProgramBlobs(reader.Layout, offsets, compressedLengths, decompressedLengths, compressedBlob);
				}
				else
				{
					Offsets1D = reader.ReadUInt32Array();
					CompressedLengths1D = reader.ReadUInt32Array();
					DecompressedLengths1D = reader.ReadUInt32Array();
					CompressedBlob = reader.ReadByteArray();
					reader.AlignStream();

					//UnpackSubProgramBlobs(reader.Layout, offsets, compressedLengths, decompressedLengths, compressedBlob);
				}
			}
			else
			{
				base.Read(reader);

				if (HasBlob(reader.Version))
				{
					DecompressedSize = reader.ReadUInt32();
					CompressedBlob = reader.ReadByteArray();
					reader.AlignStream();

					//UnpackSubProgramBlobs(reader.Layout, 0, (uint)compressedBlob.Length, decompressedSize, compressedBlob);
				}

				if (HasFallback(reader.Version))
				{
					Fallback.Read(reader);
				}
				if (HasDefaultProperties(reader.Version))
				{
					DefaultProperties.Read(reader);
				}
				if (HasStaticProperties(reader.Version))
				{
					StaticProperties.Read(reader);
				}
			}

			if (HasDependencies(reader.Version))
			{
				Dependencies = reader.ReadAssetArray<PPtr<Shader>>();
			}
			if (HasNonModifiableTextures(reader.Version))
			{
				NonModifiableTextures = new Dictionary<string, PPtr<Texture>>();
				NonModifiableTextures.Read(reader);
			}
			if (HasShaderIsBaked(reader.Version))
			{
				ShaderIsBaked = reader.ReadBoolean();
				reader.AlignStream();
			}

#if UNIVERSAL
			if (HasErrors(reader.Version, reader.Flags))
			{
				Errors = reader.ReadAssetArray<ShaderError>();
			}
			if (HasDefaultTextures(reader.Version, reader.Flags))
			{
				DefaultTextures = new Dictionary<string, PPtr<Texture>>();
				DefaultTextures.Read(reader);
			}
			if (HasCompileInfo(reader.Version, reader.Flags))
			{
				CompileInfo.Read(reader);
			}
#endif
		}

		/*public override void ExportBinary(IExportContainer container, Stream stream)
		{
			ExportBinary(container, stream, DefaultShaderExporterInstantiator);
		}*/

		/*public void ExportBinary(IExportContainer container, Stream stream, Func<Version, GPUPlatform, ShaderTextExporter> exporterInstantiator)
		{
			if (IsSerialized(container.Version))
			{
				using (ShaderWriter writer = new ShaderWriter(stream, this, exporterInstantiator))
				{
					ParsedForm.Export(writer);
				}
			}
			else if (HasBlob(container.Version))
			{
				using (ShaderWriter writer = new ShaderWriter(stream, this, exporterInstantiator))
				{
					string header = Encoding.UTF8.GetString(Script);
					if (Blobs.Length == 0)
					{
						writer.Write(header);
					}
					else
					{
						Blobs[0].Export(writer, header);
					}
				}
			}
			else
			{
				base.ExportBinary(container, stream);
			}
		}*/

		public override IEnumerable<PPtr<Object>> FetchDependencies(DependencyContext context)
		{
			foreach (PPtr<Object> asset in base.FetchDependencies(context))
			{
				yield return asset;
			}

			if (HasDependencies(context.Version))
			{
				foreach (PPtr<Object> asset in context.FetchDependencies(Dependencies, DependenciesName))
				{
					yield return asset;
				}
			}
		}

		/*public static ShaderTextExporter DefaultShaderExporterInstantiator(Version version, GPUPlatform graphicApi)
		{
			switch (graphicApi)
			{
				case GPUPlatform.unknown:
					return new ShaderTextExporter();

				case GPUPlatform.openGL:
				case GPUPlatform.gles:
				case GPUPlatform.gles3:
				case GPUPlatform.glcore:
					return new ShaderGLESExporter();

				case GPUPlatform.metal:
					return new ShaderMetalExporter();

				default:
					return new ShaderUnknownExporter(graphicApi);
			}
		}*/

		protected override YAMLMappingNode ExportYAMLRoot(IExportContainer container)
		{
			YAMLMappingNode node = base.ExportYAMLRoot(container);
			node.InsertSerializedVersion(ToSerializedVersion(container.ExportVersion));
			node.Add("m_ParsedForm", ParsedForm.ExportYAML(container));
			node.Add("platforms", Platforms.Cast<int>().ExportYAML(false));
			if (!IsSerialized(container.Version))
            {
				node.Add("decompressedSize", DecompressedSize);

			}
			else if (IsDoubleArray(container.Version))
			{
				node.Add("offsets", Offsets2D.ExportYAML(false));
				node.Add("compressedLengths", CompressedLengths2D.ExportYAML(false));
				node.Add("decompressedLengths", DecompressedLengths2D.ExportYAML(false));
			}
            else
			{
				node.Add("offsets", Offsets1D.ExportYAML(false));
				node.Add("compressedLengths", CompressedLengths1D.ExportYAML(false));
				node.Add("decompressedLengths", DecompressedLengths1D.ExportYAML(false));
			}
			node.Add("compressedBlob", CompressedBlob.ExportYAML());
			node.Add("m_Dependencies", Dependencies.ExportYAML(container));

			return node;
		}

		/*private void UnpackSubProgramBlobs(AssetLayout layout, uint offset, uint compressedLength, uint decompressedLength, byte[] compressedBlob)
		{
			if (compressedBlob.Length == 0)
			{
				Blobs = Array.Empty<ShaderSubProgramBlob>();
			}
			else
			{
				Blobs = new ShaderSubProgramBlob[1];
				using (MemoryStream memStream = new MemoryStream(compressedBlob))
				{
					uint[] offsets = new uint[] { offset };
					uint[] compressedLengths = new uint[] { compressedLength };
					uint[] decompressedLengths = new uint[] { decompressedLength };
					Blobs[0].Read(layout, memStream, offsets, compressedLengths, decompressedLengths);
				}
			}
		}

		private void UnpackSubProgramBlobs(AssetLayout layout, uint[] offsets, uint[] compressedLengths, uint[] decompressedLengths, byte[] compressedBlob)
		{
			Blobs = new ShaderSubProgramBlob[offsets.Length];
			using (MemoryStream memStream = new MemoryStream(compressedBlob))
			{
				for (int i = 0; i < Blobs.Length; i++)
				{
					uint[] blobOffsets = new uint[] { offsets[i] };
					uint[] blobCompressedLengths = new uint[] { compressedLengths[i] };
					uint[] blobDecompressedLengths = new uint[] { decompressedLengths[i] };
					Blobs[i].Read(layout, memStream, blobOffsets, blobCompressedLengths, blobDecompressedLengths);
				}
			}
		}

		private void UnpackSubProgramBlobs(AssetLayout layout, uint[][] offsets, uint[][] compressedLengths, uint[][] decompressedLengths, byte[] compressedBlob)
		{
			Blobs = new ShaderSubProgramBlob[offsets.Length];
			using (MemoryStream memStream = new MemoryStream(compressedBlob))
			{
				for (int i = 0; i < Platforms.Length; i++)
				{
					uint[] blobOffsets = offsets[i];
					uint[] blobCompressedLengths = compressedLengths[i];
					uint[] blobDecompressedLengths = decompressedLengths[i];
					Blobs[i].Read(layout, memStream, blobOffsets, blobCompressedLengths, blobDecompressedLengths);
				}
			}
		}*/

		public override string ExportExtension => AssetExtension;

		public override string ValidName => IsSerialized(File.Version) ? ParsedForm.Name : base.ValidName;

		public GPUPlatform[] Platforms { get; set; }

		//2D arrays starting from 2019.3
		public uint[][] Offsets2D { get; set; }
		public uint[][] CompressedLengths2D { get; set; }
		public uint[][] DecompressedLengths2D { get; set; }
		//1D arrays before 2019.3
		public uint[] Offsets1D { get; set; }
		public uint[] CompressedLengths1D { get; set; }
		public uint[] DecompressedLengths1D { get; set; }
		//Not serialized
		public uint DecompressedSize { get; set; }
		public byte[] CompressedBlob { get; set; }

		//public ShaderSubProgramBlob[] Blobs { get; set; }
		public PPtr<Shader>[] Dependencies { get; set; }
		public Dictionary<string, PPtr<Texture>> NonModifiableTextures { get; set; }
		public bool ShaderIsBaked { get; set; }
#if UNIVERSAL
		public ShaderError[] Errors { get; set; }
		public Dictionary<string, PPtr<Texture>> DefaultTextures { get; set; }
#endif

		public const string ErrorsName = "errors";
		public const string DependenciesName = "m_Dependencies";

		public SerializedShader ParsedForm;
		public PPtr<Shader> Fallback;
		public UnityPropertySheet DefaultProperties;
		public UnityPropertySheet StaticProperties;
#if UNIVERSAL
		public ShaderCompilationInfo CompileInfo;
#endif
	}
}
