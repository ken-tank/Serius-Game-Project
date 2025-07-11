using System;

namespace Game.Core
{
    /// <summary>
    /// Mengacak elemen array menggunakan algoritma Fisher-Yates Shuffle dengan LCG
    /// dan mengembalikan array baru yang sudah diacak. Array asli tidak diubah.
    /// </summary>
    /// <typeparam name="T">Tipe data elemen array.</typeparam>
    /// <param name="seed">Seed untuk generated random konsisten.</param>
    /// <returns>Array baru yang berisi elemen-elemen asli dalam urutan acak.</returns>
    public static class ExtensionsHelper
    {
        public static T[] FisherYatesShuffle<T>(this T[] self, int seed)
        {
            var lcg = new RandomLCG(seed);
            var clone = new T[self.Length];
            Array.Copy(self, clone, self.Length);

            int n = clone.Length;
            for (int i = n - 1; i > 0; i--)
            {
                int j = lcg.Next(i + 1);

                (clone[j], clone[i]) = (clone[i], clone[j]);
            }

            return clone;
        }
    }
}