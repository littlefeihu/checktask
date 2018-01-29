using System;
using System.IO;
using System.Threading.Tasks;

using LexisNexis.Red.Common;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.iOS.Common.Implementation
{
	public class FileDirectory : IDirectory
	{

 		#region IDirectory implementation
		///Users/chana/Library/Developer/CoreSimulator/Devices/0707BA8B-19B6-46E7-880F-52BB31397658/data/Containers

 		/// Gets the app root path.
		/// </summary>
		/// <returns>The app root path.</returns>
		public string GetAppRootPath ()
		{
			string appRootPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/";
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
			return await Task.Run<bool> (()=>{
				if (path == null)
					throw new ArgumentNullException ("path");
				Directory.CreateDirectory (GetAppRootPath() + path);
				return false;
			});

		}

		/// <summary>
		/// Determines whether the specified file exist in application internal storage
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async Task<bool> FileExists(string fileName)
		{
			return await Task.Run<bool> (()=>{
				string fullFilePath = GetAppRootPath() + fileName;
				return File.Exists(fullFilePath);
			});
		}

		/// <summary>
		/// Save file to application internal storage
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="buffer"></param>
		public async Task SaveFileToInternal(string fileName, byte[] buffer)
		{
			await Task.Run (()=>{
				File.WriteAllBytes (GetAppRootPath () + fileName, buffer);
			});
		}

		public bool PathExists (string filePath)
		{
			string fullFilePath = GetAppRootPath() + filePath;
			return File.Exists(fullFilePath);
		}
			

		public async Task DeleteFile (string fileName)
		{
			await Task.Run (()=>{
				File.Delete (GetAppRootPath() + fileName);
			});
		}

		public void DeletePath (string pathName)
		{
			Directory.Delete (GetAppRootPath () + pathName);
		}

		public Stream CreateFile (string fileName)
		{
			return File.Create (GetAppRootPath () + fileName);
		}

		public async Task<bool> InternalFileExists (string fileName)
		{
			return await FileExists (fileName);
		}

		public async Task<bool> DirectoryExists (string pathName)
		{
			return await Task.Run<bool> (()=>{
				return Directory.Exists (GetAppRootPath () + pathName);
			});
		}

		public async Task DeleteDirectory (string pathName)
		{
			await Task.Run (()=>{
				Directory.Delete (GetAppRootPath () + pathName, true);
			});
		}

		public async Task<Stream> OpenFile (string fileName, FileModeEnum fileMode = FileModeEnum.Create)
		{
			FileMode curOsFileMode;
			switch (fileMode) {
			case FileModeEnum.Append:
				curOsFileMode = FileMode.Append;
				break;
			case FileModeEnum.Open:
				curOsFileMode = FileMode.Open;
				break;
			default:
				curOsFileMode = FileMode.OpenOrCreate;
				break;
			}
			string fullFilePath = GetAppRootPath () + fileName;
			string pureFileName = fileName;
			if (fileName.LastIndexOf ("/") != -1) {
				pureFileName = fileName.Substring (fileName.LastIndexOf ("/"));
			}
			if (File.Exists (fullFilePath)) {
				return File.Open (fullFilePath, curOsFileMode, FileAccess.Read, FileShare.Read);
			} else {
				await CreateDirectory (fullFilePath.Replace(pureFileName, ""));
				return File.Create (fullFilePath);
			}
		}

		public Task<string[]> GetFiles(string pathName)
		{
			return Task.Run<string[]> (()=>{
				return Directory.GetFiles(pathName);
			});
		}

		#endregion
			
	}
}

