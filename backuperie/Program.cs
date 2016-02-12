using System;
using System.Collections.Generic;
using System.IO;

namespace backuperie
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//var src = args[0];
			//var dst = args[1];
			var src = @"\\?\C:\Users\NicDub\Desktop\backuperie\src\Test.txt";
			var dst = @"\\?\C:\Users\NicDub\Desktop\backuperie\dst";

			//using (FileStream fs = new FileStream(new Path(src).GetLongPath, FileMode.Open, FileAccess.Read))
			//{
			//	// Read the source file into a byte array.
			//	byte[] bytes = new byte[fs.Length];
			//	int numBytesToRead = (int)fs.Length;
			//	int numBytesRead = 0;
			//	while (numBytesToRead > 0)
			//	{
			//		// Read may return anything from 0 to numBytesToRead.
			//		int n = fs.Read(bytes, numBytesRead, numBytesToRead);

			//		// Break when the end of the file is reached.
			//		if (n == 0)
			//			break;

			//		numBytesRead += n;
			//		numBytesToRead -= n;
			//	}
			//	numBytesToRead = bytes.Length;
			//	Console.WriteLine(bytes);
			//}

			var safe = IoLongPath.CreateFile(
				new Path(src).GetLongPath,
				IoLongPath.EFileAccess.GenericRead,
				IoLongPath.EFileShare.Read, IntPtr.Zero,
				IoLongPath.ECreationDisposition.OpenExisting,
				0,
				IntPtr.Zero);

			using (FileStream fs = new FileStream(safe, FileAccess.Read))
			{
				StreamReader sr = new StreamReader(fs);
				Console.WriteLine(sr.ReadToEnd());

				//// Read the source file into a byte array.
				//byte[] bytes = new byte[fs.Length];
				//int numBytesToRead = (int)fs.Length;
				//int numBytesRead = 0;
				//while (numBytesToRead > 0)
				//{
				//	// Read may return anything from 0 to numBytesToRead.
				//	int n = fs.Read(bytes, numBytesRead, numBytesToRead);

				//	// Break when the end of the file is reached.
				//	if (n == 0)
				//		break;

				//	numBytesRead += n;
				//	numBytesToRead -= n;
				//}
				//numBytesToRead = bytes.Length;
				//Console.WriteLine(bytes);
			}

			//IoLongPath.FindClose(ptr);

			//Backup(new Path(src), new Path(dst));

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
