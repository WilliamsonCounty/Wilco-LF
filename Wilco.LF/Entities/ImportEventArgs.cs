using System;

namespace Wilco.LF
{
	public class ImportEventArgs : EventArgs
	{
		public IImportDocument ImportDocument { get; internal set; }
		public Exception LaserficheImportException { get; internal set; }
		public bool Succeeded { get; internal set; }
	}
}