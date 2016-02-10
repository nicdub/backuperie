using System;
using System.IO;

namespace backuperie
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//var src = args[0];
			//var dst = args[1];
			var src = @"\\?\C:\Users\NicDub\Desktop\backuperie\src\*";
			var dst = @"\\?\C:\Users\NicDub\Desktop\backuperie\dst\";
			
				

			Backup(new Path(src), new Path(dst));

		}

		private static void Backup(Path src, Path dst)
		{
			Console.WriteLine("Current dir: " + src);

			IoLongPath.WIN32_FIND_DATA lpFindFileData;
			var ptr = IoLongPath.FindFirstFile(src.GetLongPath, out lpFindFileData);
			while (IoLongPath.FindNextFile(ptr, out lpFindFileData))
			{
			}
		}
	}
}
