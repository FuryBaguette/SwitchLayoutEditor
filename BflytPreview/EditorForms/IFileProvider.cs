using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BflytPreview.EditorForms
{
	public interface IFileProvider
	{
		void SaveToArchive(byte[] Data, IFormSaveToArchive ChildForm);
		void EditorClosed(IFormSaveToArchive ChildForm);
	}

	public interface IFormSaveToArchive
	{
		IFileProvider ParentArchive { get; set; }
	}
}
