using System.IO;

namespace Wilco.LF
{
	public interface IImportDocument : IImportable
	{
		FileInfo FileInfo { get; set; }
		string Extension { get; }
		string LaserfichePath { get; set; }
		string MimeType { get; set; }
	}
}