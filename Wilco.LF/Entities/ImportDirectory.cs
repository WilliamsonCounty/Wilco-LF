using Laserfiche.RepositoryAccess;

namespace Wilco.LF;

public class ImportDirectory : IImportDirectory
{
	public bool Skip { get; set; } = false;
	public DirectoryInfo DirectoryInfo { get; }
	public EntryType EntryType => EntryType.Folder;
	public FieldValueCollection FieldValues { get; set; } = new();
	public Guid Id { get; } = Guid.NewGuid();
	public IEnumerable<IImportDocument> Entries { get; }
	public string FullName => DirectoryInfo.FullName;
	public string Name => DirectoryInfo.Name;
	public string TemplateName { get; set; } = "";

	public ImportDirectory(DirectoryInfo directoryInfo)
	{
		DirectoryInfo = directoryInfo;
		//Entries = GetEntries("", EntryType.Document, true);
	}

	private static List<EntryInfo> GetEntries(string parentFolder, EntryType entryType, bool recursive)
	{
		var type = entryType == EntryType.Document ? "D" : "F";

		var searchParameters = recursive
			? $"{{LF:Name=\"*\", Type=\"{type}\"}} & {{LF:LOOKIN=\"\\\\{parentFolder}\"}}"
			: $"{{LF:Name=\"*\", Type=\"{type}\"}} & {{LF:LOOKIN=\"\\\\{parentFolder}\", SUBFOLDERS=0}}";

		return EntrySearch.GetEntries(searchParameters);
	}

	public void Import() => throw new NotImplementedException();
}