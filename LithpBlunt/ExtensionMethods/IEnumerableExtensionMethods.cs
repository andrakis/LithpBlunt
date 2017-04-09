using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LithpBlunt.ExtensionMethods
{
	public static class IEnumerableExtensionMethods
	{
		public static void Each(this string[] values, Action<string,int> callback) {
			int index = 0;
			foreach (var x in values)
				callback(x, index++);
		}
	}
}
