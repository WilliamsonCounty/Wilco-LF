using Laserfiche.RepositoryAccess;

namespace Wilco.LF;

public interface IImportable
{
	bool Skip { get; }
	EntryType EntryType { get; }
	FieldValueCollection FieldValues { get; }
	Guid Id { get; }
	string FullName { get; }
	string Name { get; }
	string TemplateName { get; }

	void Import();
}