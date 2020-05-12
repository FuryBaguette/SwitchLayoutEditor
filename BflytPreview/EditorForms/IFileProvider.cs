using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BflytPreview
{
	public interface IFileWriter
	{
		void Save(byte[] Data);
		void EditorClosed();

		string Path { get; }
	}

	public class DiskFileProvider : IFileWriter
	{
		public DiskFileProvider(string path) =>
			Path = path;

		public string Path { get; set; }

		public void EditorClosed() { }

		public void Save(byte[] Data) =>
			System.IO.File.WriteAllBytes(Path, Data);

		public override string ToString() => Path;
	}
}
