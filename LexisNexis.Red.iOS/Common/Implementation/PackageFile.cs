using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.HelpClass;

using ICSharpCode.SharpZipLib.Zip;

using Xamarin;

namespace LexisNexis.Red.iOS.Common.Implementation
{
	/// <summary>
	/// used to process digital looseleaf package
	/// </summary>
	public class PackageFile : IPackageFile
	{

		/// <summary>
		/// Extract file to required path
		/// </summary>
		/// <param name="sourceFileName">zip name</param>
		/// <param name="targetPath">target path</param>
		/// <param name="cancelToken">cancelToken</param>
		/// <returns></returns>
		public async Task DepressFile(string sourceFileName, string targetPath, CancellationToken cancelToken)
		{
			var fileDirectory = new FileDirectory ();

			string cachePath = fileDirectory.GetAppExternalRootPath();
			var sourceFullPath = Path.Combine (cachePath, sourceFileName);
			var targetFullPath = Path.Combine (cachePath, targetPath);

			var zip = new MiniZip.ZipArchive.ZipArchive ();
			zip.UnzipOpenFile (sourceFullPath);
			zip.UnzipFileTo (targetFullPath, true);

			zip.OnError += (sender, args) => {
				Insights.Report(new Exception(), new Dictionary<string, string>{
					{"Exception summary","Depress publication file"},
					{"Publication ID", AppDataUtil.Instance.GetCurrentPublication().BookId.ToString()},
					{"Publication Name", AppDataUtil.Instance.GetCurrentPublication().Name},
					{"Country", GlobalAccess.Instance.CurrentUserInfo.Country.CountryName},
					{"Email", GlobalAccess.Instance.CurrentUserInfo.Email}
				});

			};
			zip.UnzipCloseFile ();
				
		}


		/// <summary>
		/// unzip byte array
		/// </summary>
		/// <param name="bytes">zip bytes data</param>
		/// <returns>unzip bytes data</returns>
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

