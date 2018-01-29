using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using LexisNexis.Red.Common;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac.Common.Implementation
{
	public class FileDirectory : IDirectory
	{


		#region IDirectory implementation
		/// <summary>
		/// Gets the app root path.
		/// </summary>
		/// <returns>The app root path.</returns>
		public string GetAppRootPath ()
		{
			var cachepath = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory,NSSearchPathDomain.User);
			var hideDirPath = NSBundle.MainBundle.BundleIdentifier;

			string appRootPath = cachepath[0]+"/"+hideDirPath+"/";
			//Console.WriteLine (appRootPath);
				
			if (!Directory.Exists (appRootPath)) {
				Directory.CreateDirectory (appRootPath);
			}
			return appRootPath;
		}

		/// <summary>
		/// Gets the app external stroage root path.
		/// The external storage root path in specific platform used to storge big files
		/// </summary>
		/// <returns>The app external storage root path.</returns>
		public string GetAppExternalRootPath ()
		{
			return GetAppRootPath ();	
		}

		/// <summary>
		/// Creates the directory.
		/// </summary>
		/// <returns><c>true</c>, if directory was created, <c>false</c> otherwise.</returns>
		/// <param name="path">Path.</param>
		public async Task<bool> CreateDirectory (string path)
		{
			return await Task.Run(() =>
				{
					path = Path.Combine(GetAppRootPath(), path);
					Directory.CreateDirectory(path);
					return true;
				});
		}

		public async Task<bool> InternalFileExists(string fileName)
		{
			return await Task.Run(() =>
				{
					fileName = Path.Combine(GetAppRootPath(), fileName);
					return File.Exists(fileName);
				});
		}

		/// <summary>
		/// Determines whether the specified file exist in application internal storage
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task<bool> FileExists(string fileName)
		{
			return await Task.Run(() =>
				{
					fileName = Path.Combine(GetAppRootPath(), fileName);
					return File.Exists(fileName);
				});
		}

		public async Task<bool> DirectoryExists(string pathName)
		{
			return await Task.Run(() =>
				{
					pathName = Path.Combine(GetAppRootPath(), pathName);
					return Directory.Exists(pathName);
				});
		}

		/// <summary>
		/// Save file to application internal storage
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="buffer"></param>
		public async Task SaveFileToInternal(string fileName, byte[] buffer)
		{
			await Task.Run(() => {
				File.WriteAllBytes (GetAppRootPath () + fileName, buffer);
			});
		}

		public async Task DeleteFile(string fileName)
		{
			await Task.Run(() =>
				{
					fileName = Path.Combine(GetAppRootPath(), fileName);
					File.Delete(fileName);
				});
		}

		public async Task DeleteDirectory(string pathName)
		{
			await Task.Run(() =>
				{
					pathName = Path.Combine(GetAppRootPath(), pathName);
					Directory.Delete(pathName, true);
				});
		}

		public async Task<Stream> OpenFile(string fileName, FileModeEnum fileMode = FileModeEnum.Create)
		{
			string fullFileName = Path.Combine(GetAppRootPath(), fileName);
			return await Task.Run<Stream>(() =>
				{
                    switch (fileMode)
                    {
                        case FileModeEnum.Create:
                            return File.Open(fullFileName, FileMode.Create);
                        case FileModeEnum.Open:
                            return File.Open(fullFileName, FileMode.Open);
                        default:
                            return File.Open(fullFileName, FileMode.Append);
                    }
				});
		}

		public async Task<string[]> GetFiles(string pathName)
		{
			return await Task.Run (()=>{
				string [] fileNames = new string[] {""};
				return fileNames;
			}); 
		}

		#endregion
			
	}
}

