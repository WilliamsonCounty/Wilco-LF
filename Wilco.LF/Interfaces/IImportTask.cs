using Laserfiche.RepositoryAccess;

namespace Wilco.LF
{
	public interface IImportTask
	{
		event ImportCompletedEventHandler ImportCompleted;

		IImportDirectory ImportDirectory { get; }
		Session LaserficheSession { get; set; }
		string LaserfichePath { get; set; }
		string VolumeName { get; set; }

		void Run();
	}
}