using Laserfiche.RepositoryAccess;

namespace Wilco.LF;

public class ImportDocument : IImportDocument
{
	public bool Skip { get; set; } = false;
	public EntryType EntryType => EntryType.Document;
	public FieldValueCollection FieldValues { get; set; } = new();
	public FileInfo FileInfo { get; set; }
	public Guid Id { get; } = Guid.NewGuid();
	public string Extension => FileInfo.Extension;
	public string FullName => FileInfo.FullName;
	public string LaserfichePath { get; set; }
	public string MimeType { get; set; }
	public string Name => FileInfo.Name;
	public string TemplateName { get; set; }

	public void Import() => throw new NotImplementedException();
}