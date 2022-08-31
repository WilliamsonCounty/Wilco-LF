using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Laserfiche.DocumentServices;
using Laserfiche.RepositoryAccess;
using Wilco;

namespace Wilco.LF
{
	public class ImportTask : IImportTask
	{
		public event ImportCompletedEventHandler ImportCompleted;

		private readonly DirectoryInfo _rootDirectory;
		private readonly List<IImportDocument> _entries;

		public IImportDirectory ImportDirectory { get; }
		public string LaserfichePath { get; set; }
		public Session LaserficheSession { get; set; }
		public string VolumeName { get; set; }

		public ImportTask(IImportDocument importDocument) => _entries.Add(importDocument);

		public ImportTask(IImportDirectory importDirectory)
		{
			ImportDirectory = importDirectory;
			_rootDirectory = importDirectory.DirectoryInfo;
			//_entries = importDirectory.Entries;
		}

		public void Run()
		{
			if (!ImportDirectory.DirectoryInfo.HasFiles(out _))
				return;

			ParseDirectoryStructure();
			Run(ImportOptions.AllDirectories);
		}

		private void Run(ImportOptions options)
		{
			var entryId = 0;
			var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
			var concurrentBag = new ConcurrentBag<IImportDocument>(_entries);

			var loopResult = Parallel.ForEach(concurrentBag, parallelOptions, importDocument =>
			{
				if (importDocument.Skip) return;

				var args = new ImportEventArgs();
				var docImporter = new DocumentImporter();
				var filePath = ParseFilePath(options, importDocument);
				var folderInfo = Folder.GetFolderInfo(filePath, LaserficheSession);

				if (TryCreateDocument(folderInfo, importDocument, out DocumentInfo documentInfo))
				{
					try
					{
						entryId = documentInfo.Id;
						docImporter.Document = documentInfo;
						docImporter.OcrImages = false;
						importDocument.LaserfichePath = documentInfo.Path;

						using (var file = File.OpenRead(importDocument.FullName))
						{
							if (importDocument.MimeType.Contains("text/plain"))
							{
								_ = docImporter.ImportText(file);
							}
							else
							{
								docImporter.ImportEdoc(importDocument.MimeType, file);
							}

							args.ImportDocument = importDocument;
							args.Succeeded = true;
						}
					}
					catch (Exception ex)
					{
						// [9002] = file exists
						// [9035] = too many tasks

						args.ImportDocument = importDocument;
						args.Succeeded = false;
						args.LaserficheImportException = ex;
					}

					OnImportCompleted(args);
				}
			});
		}

		private bool TryCreateDocument(FolderInfo folderInfo, IImportDocument importDocument, out DocumentInfo documentInfo)
		{
			using (var docInfo = new DocumentInfo(LaserficheSession))
			{
				try
				{
					docInfo.Create(folderInfo, importDocument.Name, VolumeName, EntryNameOption.Overwrite);

					if (importDocument.FieldValues != null)
					{
						docInfo.SetTemplate(importDocument.TemplateName);
						docInfo.SetFieldValues(importDocument.FieldValues);
					}

					docInfo.MimeType = importDocument.MimeType;
					docInfo.Extension = importDocument.Extension;
					docInfo.Save();
					documentInfo = docInfo;

					return true;
				}
				catch
				{
					documentInfo = null;

					return false;
				}
			}
		}

		private void ParseDirectoryStructure()
		{
			var path = ImportDirectory.FullName;
			var parent = Directory.GetParent(path).FullName;
			var folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();
			var laserficheFolders = folders.Select(folder => folder.Replace(parent, LaserfichePath)).ToList();

			folders.Sort();
			laserficheFolders.Insert(0, Path.Combine(LaserfichePath, _rootDirectory.Name));

			foreach (var folder in laserficheFolders)
			{
				CreateLaserficheDirectory(folder);
			}
		}

		private void CreateLaserficheDirectory(string folder)
		{
			var entryInfo = Entry.TryGetEntryInfo(folder, LaserficheSession);

			if (entryInfo is EntryInfo) return;

			try
			{
				var tocid = Folder.Create(folder, VolumeName, EntryNameOption.None, LaserficheSession);
			}
			catch
			{
				// [9002] = folder exists
			}
		}

		private string ParseFilePath(ImportOptions options, IImportable entryToUpload)
		{
			var filePath = Path.GetDirectoryName(entryToUpload.FullName);

			if (options == ImportOptions.AllDirectories)
			{
				return filePath.Replace(_rootDirectory.Parent.FullName, LaserfichePath); ;
			}

			return filePath.Replace(_rootDirectory.FullName, LaserfichePath); ;
		}

		protected virtual void OnImportCompleted(ImportEventArgs e) => ImportCompleted?.Invoke(this, e);
	}
}