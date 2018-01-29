using System;
using LexisNexis.Red.Common.Interface;
using System.Threading.Tasks;
using Java.Util.Zip;
using System.IO;
using LexisNexis.Red.Droid.Utility;

namespace LexisNexis.Red.Droid.Implementation
{
	public class PackageFile: IPackageFile
	{
		private const int BufferSize = 2048;

		public Task DepressFile(string sourceFileName, string targetPath, System.Threading.CancellationToken cancelToken)
		{
			var sourceFullPath = Path.Combine(FileDirectory.AppExternalStorage, sourceFileName);
			var targetFullPath = Path.Combine(FileDirectory.AppExternalStorage, targetPath);

			return Task.Run(delegate {
				Unzip(sourceFullPath, targetFullPath, cancelToken);
			});
		}

		public Task<byte[]> UnZipAsync(byte[] bytes)
		{
			return Task.Run(delegate {
				return Unzip(bytes);
			});
		}

		private static byte[] Unzip(byte[] bytes)
		{
			using(var outStream = new MemoryStream())
			using(var inStream = new MemoryStream(bytes))
			using(var zip = new ZipInputStream(inStream))
			{
				var firstEntry = zip.NextEntry;
				if(firstEntry == null)
				{
					return new byte[0];
				}

				int size;
				var data = new byte[BufferSize];
				while(true)
				{
					size = zip.Read(data, 0, data.Length);
					if(size > 0)
					{
						outStream.Write(data, 0, size);
					}
					else
					{
						break;
					}
				}

				return outStream.ToArray();
			}
		}

		private static void Unzip(string sourceFullPath, string targetFullPath, System.Threading.CancellationToken cancelToken)
		{
			if (cancelToken.IsCancellationRequested)
			{
				cancelToken.ThrowIfCancellationRequested();
			}

			using(ZipInputStream s = new ZipInputStream(File.OpenRead(sourceFullPath)))
			{
				ZipEntry theEntry;
				while((theEntry = s.NextEntry) != null)
				{
					if (cancelToken.IsCancellationRequested)
					{
						cancelToken.ThrowIfCancellationRequested();
					}

					string directoryName = Path.GetDirectoryName(theEntry.Name);
					string fileName = Path.GetFileName(theEntry.Name);
					directoryName = Path.Combine(targetFullPath, directoryName);
					if(directoryName.Length > 0)
					{
						Directory.CreateDirectory(directoryName);
					}

					if(fileName != String.Empty)
					{
						if (cancelToken.IsCancellationRequested)
						{
							cancelToken.ThrowIfCancellationRequested();
						}

						using(FileStream streamWriter = File.Create(Path.Combine(targetFullPath, theEntry.Name)))
						{
							int size;
							byte[] data = new byte[BufferSize];
							while(true)
							{
								if (cancelToken.IsCancellationRequested)
								{
									cancelToken.ThrowIfCancellationRequested();
								}

								size = s.Read(data, 0, data.Length);
								if(size > 0)
								{
									streamWriter.Write(data, 0, size);
								}
								else
								{
									break;
								}
							}
						}
					}
				}
			}
		}
	}
}

