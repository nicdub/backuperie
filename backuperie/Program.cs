using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;

namespace backuperie
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//var src = args[0];
			//var dst = args[1];
			var src = @"\\?\C:\Users\NicDub\Desktop\backuperie\src\web";
			var dst = @"\\?\C:\Users\NicDub\Desktop\backuperie\dst\web";


			//var safe = IoLongPath.CreateFile(
			//	new Path(src).GetLongPath,
			//	IoLongPath.EFileAccess.GenericRead,
			//	IoLongPath.EFileShare.Read, IntPtr.Zero,
			//	IoLongPath.ECreationDisposition.OpenExisting,
			//	0,
			//	IntPtr.Zero);

			//using (FileStream fs = new FileStream(safe, FileAccess.Read))
			//{
			//	StreamReader sr = new StreamReader(fs);
			//	Console.WriteLine(sr.ReadToEnd());

			//}

			//IoLongPath.FindClose(ptr);

			Backup(new Path(src), new Path(dst));

			//CopyFile(new Path(src), new Path(dst));
			//ZipFile(new Path(src), new Path(dst));

		}


		private static void ZipFile(Path src, Path dst)
		{
			var fhSrc = IoLongPath.CreateFile(
				src.LongPath,
				IoLongPath.EFileAccess.GenericRead,
				IoLongPath.EFileShare.Read,
				IntPtr.Zero,
				IoLongPath.ECreationDisposition.OpenExisting,
				0,
				IntPtr.Zero);

			// Check for errors
			var lastWin32Error = Marshal.GetLastWin32Error();
			if (fhSrc.IsInvalid)
			{
				throw new System.ComponentModel.Win32Exception(lastWin32Error);
			}

			using (var fsSrc = new FileStream(fhSrc, FileAccess.Read))
			{

				// Create destination zip file
				var fhDst = IoLongPath.CreateFile(
					dst.LongPath,
					IoLongPath.EFileAccess.GenericWrite,
					IoLongPath.EFileShare.None,
					IntPtr.Zero,
					IoLongPath.ECreationDisposition.CreateAlways,
					0,
					IntPtr.Zero);

				// Check for errors
				lastWin32Error = Marshal.GetLastWin32Error();
				if (fhDst.IsInvalid)
				{
					throw new System.ComponentModel.Win32Exception(lastWin32Error);
				}

				using (var fsDst = new FileStream(fhDst, FileAccess.Write))
				{
					using (var archive = new ZipArchive(fsDst, ZipArchiveMode.Create))
					{
						var readmeEntry = archive.CreateEntry(src.GetFilename);
						//using (var writer = new StreamWriter(readmeEntry.Open()))
						using (var writer = new BinaryWriter(readmeEntry.Open()))
						{
							byte[] b = new byte[1024];
							int n;
							while ((n = fsSrc.Read(b, 0, b.Length)) > 0)
							{
								writer.Write(b, 0, n);
							}
						}
					}
					//byte[] b = new byte[1024];
					//int n;
					//while ((n = fsSrc.Read(b, 0, b.Length)) > 0)
					//{
					//	fsDst.Write(b, 0, n);
					//}
				}


			}
		}


		private static void CopyFile(Path src, Path dst)
		{
			var fhSrc = IoLongPath.CreateFile(
				src.LongPath,
				IoLongPath.EFileAccess.GenericRead,
				IoLongPath.EFileShare.Read,
				IntPtr.Zero,
				IoLongPath.ECreationDisposition.OpenExisting,
				0,
				IntPtr.Zero);

			// Check for errors
			var lastWin32Error = Marshal.GetLastWin32Error();
			if (fhSrc.IsInvalid)
			{
				throw new System.ComponentModel.Win32Exception(lastWin32Error);
			}

			using (var fsSrc = new FileStream(fhSrc, FileAccess.Read))
			{

				var fhDst = IoLongPath.CreateFile(
					dst.LongPath,
					IoLongPath.EFileAccess.GenericWrite,
					IoLongPath.EFileShare.None,
					IntPtr.Zero,
					IoLongPath.ECreationDisposition.CreateAlways,
					0,
					IntPtr.Zero);

				// Check for errors
				lastWin32Error = Marshal.GetLastWin32Error();
				if (fhDst.IsInvalid)
				{
					throw new System.ComponentModel.Win32Exception(lastWin32Error);
				}

				using (var fsDst = new FileStream(fhDst, FileAccess.Write))
				{
					var b = new byte[1024];
					int n;
					while ((n = fsSrc.Read(b, 0, b.Length)) > 0)
					{
						fsDst.Write(b, 0, n);
					}
				}


			}
		}


		/// <summary>
		/// Check if the specified directory exists.
		/// </summary>
		/// <param name="directory">Path of directory to check.  Must begin with \\?\ for local paths or \\?\UNC\ for network paths.</param>
		/// <returns></returns>
		public static bool Exists(Path dir)
		{
			FileAttributes fa = IoLongPath.GetFileAttributes(dir.LongPath);
			if ((int)fa == -1)
			{
				return false;
			}
			return fa.HasFlag(FileAttributes.Directory);
		}


		private static void Backup(Path src, Path dst)
		{
			Console.WriteLine("Current dir: " + src.LongPath);

			// If dst folder does not exist
			if (!Exists(dst))
			{
				bool result = IoLongPath.CreateDirectory(dst.LongPath, IntPtr.Zero);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (!result)
				{
					throw new System.ComponentModel.Win32Exception(lastWin32Error);
				}
			}

			var directories = new List<string>();

			IoLongPath.WIN32_FIND_DATA lpFindFileData;
			var ptr = IoLongPath.FindFirstFile(src.LongPath + @"\*", out lpFindFileData);
			do
			{
				var filename = lpFindFileData.cFileName;

				// Ignore some folders
				if (filename == "." || filename == "..") continue;

				Console.WriteLine(filename);

				// If file
				if ((lpFindFileData.dwFileAttributes & FileAttributes.Directory) == 0)
				{
					ZipFile(
						new Path(src.LongPath + @"\" + filename),
						new Path(dst.LongPath + @"\" + filename + ".zip"));
				}

				// If directory
				if ((lpFindFileData.dwFileAttributes & FileAttributes.Directory) != 0)
				{
					directories.Add(filename);
				}

			} while (IoLongPath.FindNextFile(ptr, out lpFindFileData));
			IoLongPath.FindClose(ptr);

			foreach (var directory in directories)
			{
				Backup(new Path(src.LongPath + "\\" + directory), new Path(dst.LongPath + "\\" + directory));
			}
		}
	}
}
