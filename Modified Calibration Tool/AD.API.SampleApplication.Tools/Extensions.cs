using System.Collections.Generic;
using System.Text;

namespace AD.API.SampleApplication.Tools;

public static class Extensions
{
	public static void AddRange<TItem>(this Stack<TItem> stack, IEnumerable<TItem> range)
	{
		if (stack == null)
		{
			return;
		}
		foreach (TItem item in range)
		{
			stack.Push(item);
		}
	}

	public static string RemoveControlCharacters(this string inString)
	{
		if (inString == null)
		{
			return null;
		}
		StringBuilder result = new StringBuilder();
		foreach (char character in inString)
		{
			if (!char.IsControl(character))
			{
				result.Append(character);
			}
		}
		return result.ToString();
	}
}
