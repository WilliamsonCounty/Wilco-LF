using System;
using System.Collections.Generic;
using System.Data;
using Laserfiche.RepositoryAccess;
using Laserfiche.RepositoryAccess.Data;

namespace Wilco.LF
{
	public class Query
	{
		public string ReturnSingleValue(string query, Session lfSession)
		{
			var returnString = string.Empty;
			LfConnection con = new LfConnection(lfSession);
			LfCommand lfqlCommand = new LfCommand(query, con);
			con.Open();

			try
			{
				using (LfDataReader reader = lfqlCommand.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							returnString = reader[0].ToString();
						}
					}
				}
			}
			catch
			{
				throw new Exception("Could not retrieve values");
			}
			finally
			{
				con.Close();
			}

			return returnString;
		}

		public static string ReturnSingleValue(string query, Session lfSession, bool whatever = false)
		{
			var returnString = string.Empty;
			LfConnection con = new LfConnection(lfSession);
			LfCommand lfqlCommand = new LfCommand(query, con);
			con.Open();

			try
			{
				using (LfDataReader reader = lfqlCommand.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							returnString = reader[0].ToString();
						}
					}
				}
			}
			catch
			{
				throw new Exception("Could not retrieve values");
			}
			finally
			{
				con.Close();
			}

			return returnString;
		}

		public static IEnumerable<string> ReturnMultiValue(string query, Session lfSession)
		{
			using (var con = new LfConnection(lfSession))
			using (var lfqlCommand = new LfCommand(query, con))
			{
				con.Open();

				using (LfDataReader reader = lfqlCommand.ExecuteReader())
				{
					if (reader.HasRows)
					{
						while (reader.Read())
						{
							yield return reader[0].ToString();
						}
					}
				}
			}
		}

		public static DataTable ReturnTable(string query, Session lfSession)
		{
			LfConnection con = new LfConnection(lfSession);
			LfCommand lfqlCommand = new LfCommand(query, con);
			DataTable dt = new DataTable();
			con.Open();

			try
			{
				using (LfDataReader reader = lfqlCommand.ExecuteReader())
				{
					dt.Load(reader);
				}
			}
			catch
			{
				throw new Exception("Could not retrieve values");
			}
			finally
			{
				con.Close();
			}

			return dt;
		}
	}
}