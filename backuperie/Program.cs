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
			var src = @"\\?\C:\Users\NicDub\Desktop\backuperie\src";
			var dst = @"\\?\C:\Users\NicDub\Desktop\backuperie\dst";
			
				

			Backup(new Path(src), new Path(dst));

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
