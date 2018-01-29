using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

using LexisNexis.Red.Common.Interface;
using Foundation;

namespace LexisNexis.Red.Mac.Common.Implementation
{
	public class UnZipFiles : IPackageFile
	{
		public async Task DepressFile(string sourceFileName, string targetPath, CancellationToken cancelToken)
		{
			var fileDirectory = new FileDirectory ();
			string cachePath = fileDirectory.GetAppExternalRootPath();
			var sourceFullPath = Path.Combine (cachePath, sourceFileName); 
			var targetFullPath = Path.Combine (cachePath, targetPath);

			if (!Directory.Exists(targetFullPath))  
				Directory.CreateDirectory(targetFullPath);  

			using (ZipInputStream inputStream = new ZipInputStream (File.OpenRead (sourceFullPath))) {
				ZipEntry entry;

				while ((entry = inputStream.GetNextEntry ()) != null) {

					if (cancelToken.IsCancellationRequested) {
						cancelToken.ThrowIfCancellationRequested ();
					}

					//Console.WriteLine ("name:{0}", entry.Name);

					if (entry.IsDirectory) {  // Folder
						await CreateRecursiveFolder(targetFullPath, entry);
					} else { // File
						await ExtractFile(targetFullPath, inputStream, entry);
					}
				}
			}
		}

		async Task CreateRecursiveFolder (string path, ZipEntry entry)
		{
			await Task.Run(() => {
				string filePath = Path.Combine (path, entry.Name);
				Directory.CreateDirectory(filePath);
			});
		}

		async Task ExtractFile(string destPath, ZipInputStream inputStream, ZipEntry entry)
		{
			string filePath = Path.Combine (destPath, entry.Name);

			using (FileStream outputSteam = File.Create (filePath)) {

				int bufferSize = 4096;  
				byte[] data = new byte[bufferSize];  
				int size = 0;
				while (size<entry.Size) {
					size = inputStream.Read (data, 0, data.Length);  
					if (size > 0) {  
						outputSteam.Write (data, 0, size);  
					} else {  
						break;  
					}  
				}

				await outputSteam.FlushAsync ();
			}  

				//await fileData.CopyToAsync (outputSteam);
		}


		public async Task<Byte[]> UnZipAsync(Byte[] bytes)
		{
			return await Task.Run<Byte[]> (()=>{
				using (MemoryStream outMemStream = new MemoryStream())
				using (ZipInputStream newinStream = new ZipInputStream (new MemoryStream (bytes))) {

					int size = 4096;
					Byte[] buffer = new byte[size];

					while (newinStream.GetNextEntry () != null) {
						while (true) {
							size = newinStream.Read (buffer, 0, buffer.Length);
							if (size > 0) {
								outMemStream.Write (buffer, 0, size);
							} else {
								break;
							}
						}
					}

					Byte[] results = outMemStream.ToArray();
					return results;
				}
			});

		}

	}
}


