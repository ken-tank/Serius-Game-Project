using System.Linq;
using UnityEngine;

namespace KenTank.Utilities.Extensions 
{
    public static class ArrayExtensions 
    {
        public static T[] ShuffleItems<T>(this T[] data)
        {
            var array = data.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                var rng = Random.Range(0, i + 1);
                (array[rng], array[i]) = (array[i], array[rng]);
            }
            return array;
        }
        
        public static T PickOne<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }
}