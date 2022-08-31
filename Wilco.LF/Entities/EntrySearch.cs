using Laserfiche.RepositoryAccess;

namespace Wilco.LF;

public class EntrySearch
{
	public static List<EntryInfo> GetEntries(string searchParameters)
	{
		var entries = new List<EntryInfo>();
		var settings = new SearchListingSettings();

		using var lfSearch = new Search(LaserficheSession.Open(), searchParameters);

		lfSearch.Run();

		var searchResults = lfSearch.GetResultListing(settings);

		if (searchResults.RowCount <= 0) return entries;

		PopulateEntries(entries, searchResults);

		return entries;
	}

	public static IEnumerable<EntryListingRow> GetEntries(string searchParameters, SearchListingSettings settings)
	{
		using (var lfSearch = new Search(LaserficheSession.Open(), searchParameters))
		{
			lfSearch.Run();

			var results = lfSearch.GetResultListing(settings);

			foreach (var entry in results) yield return entry;
		}
	}

	private static void PopulateEntries(List<EntryInfo> entries, SearchResultListing searchResults) => 
		entries.AddRange(searchResults
			   .Select(item => 
			   Entry.GetEntryInfo((int)item[SystemColumn.Id], LaserficheSession.Open())));
}