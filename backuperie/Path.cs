﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backuperie
{
	internal class Path
	{
		private string _path;
		private const string LongPathPrefix = @"\\?\";

		public Path(string path)
		{
			_path = path.StartsWith(LongPathPrefix) ? path.Substring(LongPathPrefix.Length) : path;
			{
			}
		}

		public string LongPath => LongPathPrefix + _path;

		public string GetFilename => System.IO.Path.GetFileName(_path);
	}
}
