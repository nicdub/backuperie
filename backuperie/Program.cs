using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace backuperie
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//var src = args[0];
			//var dst = args[1];
			var src = @"\\?\C:\Users\NicDub\Desktop\backuperie\src\Test.7z";
			var dst = @"\\?\C:\Users\NicDub\Desktop\backuperie\dst\Test.7z";


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

			//Backup(new Path(src), new Path(dst));

			CopyFile(new Path(src), new Path(dst));

		}

		private static void CopyFile(Path src, Path dst)
		{
			var fhSrc = IoLongPath.CreateFile(
				src.GetLongPath,
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
					dst.GetLongPath,
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
					byte[] b = new byte[1024];
					int n;
					while ((n = fsSrc.Read(b, 0, b.Length)) > 0)
					{
						fsDst.Write(b, 0, n);
					}
				}


			}
		}

		private static void Backup(Path src, Path dst)
		{
			Console.WriteLine("Current dir: " + src.GetLongPath);

			var directories = new List<string>();

			IoLongPath.WIN32_FIND_DATA lpFindFileData;
			var ptr = IoLongPath.FindFirstFile(src.GetLongPath + @"\*", out lpFindFileData);
			do
			{
				var filename = lpFindFileData.cFileName;

				// Ignore some folders
				if (filename == "." || filename == "..") continue;

				Console.WriteLine(filename);

				// If directory
				if ((lpFindFileData.dwFileAttributes & FileAttributes.Directory) != 0)
				{
					directories.Add(filename);
				}

			} while (IoLongPath.FindNextFile(ptr, out lpFindFileData));
			IoLongPath.FindClose(ptr);

			foreach (var directory in directories)
			{
				Backup(new Path(src.GetLongPath + "\\" + directory), dst);
			}
		}
	}
}
