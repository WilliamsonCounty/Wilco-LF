using System.Collections.Generic;
using System.IO;

namespace Wilco.LF
{
	public interface IImportDirectory : IImportable
	{
		DirectoryInfo DirectoryInfo { get; }
		IEnumerable<IImportDocument> Entries { get; }
	}
}