using System;
using System.IO;
using LexisNexis.Red.Droid.Implementation;

namespace LexisNexis.Red.Droid.PrintUtility
{
	public static class PdfPrintCacheHelper
	{
		private const string CacheFolder = "PdfCache";

		public static DirectoryInfo PrepareCacheFolder()
		{
			var extStorage = FileDirectory.AppExternalStorage;
			var cacheFolder = new DirectoryInfo(Path.Combine(extStorage, CacheFolder));
			if(!cacheFolder.Exists)
			{
				cacheFolder.Create();
			}

			return cacheFolder;
		}

		public static void DeleteCacheFolder()
		{
			var extStorage = FileDirectory.AppExternalStorage;
			var cacheFolder = new DirectoryInfo(Path.Combine(extStorage, CacheFolder));
			if(cacheFolder.Exists)
			{
				cacheFolder.Delete(true);
			}
		}
	}
}

