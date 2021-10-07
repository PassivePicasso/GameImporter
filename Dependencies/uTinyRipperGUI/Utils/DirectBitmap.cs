using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace uTinyRipperGUI
{
	public sealed class DirectBitmap : IDisposable
	{
		public DirectBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new byte[width * height * 4];
			m_bitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
		}

		~DirectBitmap()
		{
			Dispose(false);
		}

		public void Save(Stream stream)
		{
			Texture2D texture = new Texture2D(Width, Height, TextureFormat.BGRA32, false);
			texture.LoadRawTextureData(Bits);
			
			byte[] pngData = texture.EncodeToPNG();
			stream.Write(pngData, 0, pngData.Length);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		private void Dispose(bool _)
		{
			if (!m_disposed)
			{
				m_bitsHandle.Free();
				m_disposed = true;
			}
		}

		public int Height { get; }
		public int Width { get; }
		public byte[] Bits { get; }
		public IntPtr BitsPtr => m_bitsHandle.AddrOfPinnedObject();

		private readonly GCHandle m_bitsHandle;
		private bool m_disposed;
	}
}
