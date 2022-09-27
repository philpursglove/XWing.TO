﻿namespace XWingTO.Core;

public static class IEnumerableExtensions
{
	public static T? Random<T>(this IEnumerable<T> enumerable)
	{
		if (enumerable == null)
		{
			throw new ArgumentNullException(nameof(enumerable));
		}

		// note: creating a Random instance each call may not be correct for you,
		// consider a thread-safe static instance
		var r = new Random();
		var list = enumerable as IList<T> ?? enumerable.ToList();
		return list.Count == 0 ? default : list[r.Next(0, list.Count)];
	}
}