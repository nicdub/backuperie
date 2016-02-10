using System;
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

		public string GetLongPath => LongPathPrefix + _path;
	}
}
