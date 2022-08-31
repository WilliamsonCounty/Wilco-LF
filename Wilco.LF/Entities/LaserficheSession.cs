using Laserfiche.RepositoryAccess;

namespace Wilco.LF;

public static class LaserficheSession
{
	private static string _repository;
	private static int _retries = 1;
	private static string _serverName;

	private static Session CreateSession()
	{
		var lfRegistration = new RepositoryRegistration(new Server(_serverName), _repository);
		var lfSession = Session.Create(lfRegistration);

		return lfSession;
	}

	public static void Initialize(string serverName, string repository, int retries) =>
		(_serverName, _repository, _retries) = (serverName, repository, retries);

	public static Session Open()
	{
		while (true)
			try
			{
				return CreateSession();
			}
			catch
			{
				if (_retries-- <= 1) throw new AccessDeniedException("Error connecting to Laserficher server");

				Thread.Sleep(1000);
			}
	}
}