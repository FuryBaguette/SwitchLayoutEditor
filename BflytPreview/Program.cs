using SwitchThemes.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BflytPreview
{
	static class Program
	{
		/// <summary>
		/// Punto di ingresso principale dell'applicazione.
		/// </summary>
		[STAThread]
		static void Main(params string[] args)
		{
			/*
			 * This is a terrible hack: at some point newtonsoft json started using the type converter in the json files.
			 * This causes the position values to be serialized as "X;Y;Z" which break deserialization in the injector (cause converters are only part of the layout editor)
			 * and in the installer (cause the C++ parser expects position to be an object). As a workaround i serialize a dummy patch before registering the converters
			 * this populates json caches so it will always serialize properly. This may break at any time.
			*/
			LayoutPatch.Load(LayoutPatch.CreateTestPatches());

			TypeDescriptor.AddAttributes(typeof(Vector3), new TypeConverterAttribute(typeof(Vector3Converter)));
			TypeDescriptor.AddAttributes(typeof(Vector2), new TypeConverterAttribute(typeof(Vector2Converter)));

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1(args));
		}
	}
}
