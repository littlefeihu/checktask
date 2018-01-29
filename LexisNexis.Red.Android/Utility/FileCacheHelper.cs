using System;
using LexisNexis.Red.Droid.App;
using System.IO;
using System.Text;

namespace LexisNexis.Red.Droid.Utility
{
	public static class FileCacheHelper
	{
		private const string CacheFolder = "Cache";

		public static void SaveCacheFile(string catagory, string fileName, string content)
		{
			var cacheFolder = PrepareCacheFolder(catagory);
			var cacheFile = new FileInfo(Path.Combine(cacheFolder.FullName, fileName));
			cacheFile.Delete();

			using(var fs = new FileStream(cacheFile.FullName, FileMode.CreateNew, FileAccess.Write))
			using(var sw = new StreamWriter(fs, Encoding.UTF8))
			{
				sw.Write(content);
			}
		}

		public static string ReadCacheFile(string catagory, string fileName)
		{
			var cacheFolder = PrepareCacheFolder(catagory);
			var cacheFile = new FileInfo(Path.Combine(cacheFolder.FullName, fileName));
			if(!cacheFile.Exists)
			{
				return null;
			}

			using(var fs = new FileStream(cacheFile.FullName, FileMode.Open, FileAccess.Read))
			using(var sr = new StreamReader(fs, Encoding.UTF8))
			{
				return sr.ReadToEnd();
			}
		}

		public static void DeleteCacheFile(string catagory, string fileName)
		{
			var cacheFolder = PrepareCacheFolder(catagory);
			var cacheFile = new FileInfo(Path.Combine(cacheFolder.FullName, fileName));
			cacheFile.Delete();
		}

		public static void DeleteCacheCatagory(string catagory)
		{
			var appRoot = MainApp.ThisApp.FilesDir.AbsolutePath;
			var cacheFolder = new DirectoryInfo(
				Path.Combine(Path.Combine(appRoot, catagory), CacheFolder));
			cacheFolder.Delete(true);
		}

		private static DirectoryInfo PrepareCacheFolder(string catagory)
		{
			var appRoot = MainApp.ThisApp.FilesDir.AbsolutePath;
			var cacheFolder = new DirectoryInfo(
				Path.Combine(Path.Combine(appRoot, CacheFolder), catagory));
			if(!cacheFolder.Exists)
			{
				cacheFolder.Create();
			}

			return cacheFolder;
		}
	}
}

