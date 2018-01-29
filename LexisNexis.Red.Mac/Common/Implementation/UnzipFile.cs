using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.IO.Compression;

using LexisNexis.Red.Common.Interface;
using Foundation;

namespace LexisNexis.Red.Mac.Common.Implementation
{
	public class UnzipFile : IPackageFile
	{
		public async Task DepressFile(string sourceFileName, string targetPath, CancellationToken cancelToken)
		{
			var fileDirectory = new FileDirectory ();

			string cachePath = fileDirectory.GetAppExternalRootPath();
			var sourceFullPath = Path.Combine (cachePath, sourceFileName); 
			var targetFullPath = Path.Combine (cachePath, targetPath);
				
			using (var zipArchive = new ZipArchive(File.OpenRead(sourceFullPath),ZipArchiveMode.Read)) {
				foreach (ZipArchiveEntry entry in zipArchive.Entries) {
					
					if (cancelToken.IsCancellationRequested) {
						cancelToken.ThrowIfCancellationRequested ();
					}

					if (entry.Name == "") {  // Folder
						await CreateRecursiveFolder(targetFullPath, entry);
					} else { // File
						await ExtractFile(targetFullPath, entry);
					}
				}
			}
		}

		async Task CreateRecursiveFolder (string path, ZipArchiveEntry entry)
		{
			await Task.Run(() => {
				string filePath = Path.Combine (path, entry.FullName);
				Directory.CreateDirectory(filePath);
			});
		}

		async Task ExtractFile(string destPath, ZipArchiveEntry entry)
		{
			using (Stream fileData = entry.Open()) {

				string filePath = Path.Combine (destPath, entry.FullName);
				using (FileStream outputSteam = File.Create(filePath)) {

					await fileData.CopyToAsync (outputSteam);
					await outputSteam.FlushAsync ();
				}

			}
		}

		public async Task<byte[]> UnZipAsync(byte[] bytes)
		{
			// get content from zip
			using (MemoryStream memStream = new MemoryStream())
			{
				string entryName = string.Empty;
				using (MemoryStream strm = new MemoryStream(bytes))
				using (ZipArchive input = new ZipArchive(strm))
				{
					Byte[] buffer = new byte[4096];
					if (input.Entries.Count > 0)
					{
						ZipArchiveEntry e = input.Entries[0];
						entryName = e.Name;
						using (Stream entryStream = e.Open())
						{
							int n = 0;
							while ((n = await entryStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
							{
								await entryStream.FlushAsync();
								await memStream.WriteAsync(buffer, 0, n);
								await memStream.FlushAsync();
							}
						}
					}
				}
				return memStream.ToArray();
			}


		}
	}
}

