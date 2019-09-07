using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WSGJ.Utils
{
	public static class Extensions
	{
		public static bool HasCollidedWithGround(this Collision2D collision)
		{
			return collision.collider.CompareTag("Ground");
		}
		
		public static bool HasCollidedWithTruck(this Collision2D collision)
		{
			return collision.collider.CompareTag("Truck");
		}

		public static T GetRandomElement<T>(this T[] arr)
		{
			return arr[Random.Range(0, arr.Length)];
		}
		
		public static T GetRandomElement<T>(this IEnumerable<T> enumerable)
		{
			IEnumerable<T> ts = enumerable as T[] ?? enumerable.ToArray();
			return ts.ElementAtOrDefault(Random.Range(0, ts.Count()));
		}
	}
}