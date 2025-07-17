using System;

namespace Game.Core 
{
    public class RandomLCG
    {
        private long _seed;
        private long _multiplier;
        private long _increment;
        private long _modulus;

        /// <summary>
        /// Membuat instance LCG baru.
        /// </summary>
        /// <param name="seed">Nilai awal (seed) untuk generator angka acak.</param>
        /// <param name="multiplier">Parameter 'a' (pengali).</param>
        /// <param name="increment">Parameter 'c' (penambah).</param>
        /// <param name="modulus">Parameter 'm' (modulus).</param>
        public RandomLCG(long seed, long multiplier, long increment, long modulus)
        {
            if (modulus <= 0)
            {
                throw new ArgumentException("Modulus harus lebih besar dari 0.");
            }
            if (multiplier < 0 || multiplier >= modulus)
            {
                throw new ArgumentException("Multiplier harus antara 0 dan modulus-1.");
            }
            if (increment < 0 || increment >= modulus)
            {
                throw new ArgumentException("Increment harus antara 0 dan modulus-1.");
            }

            _seed = seed % modulus; // Pastikan seed berada dalam rentang modulus
            _multiplier = multiplier;
            _increment = increment;
            _modulus = modulus;
        }

        /// <summary>
        /// Membuat instance LCG dengan parameter yang umum digunakan (misalnya, Numerical Recipes).
        /// </summary>
        /// <param name="seed">Nilai awal (seed).</param>
        public RandomLCG(long seed)
        {
            // Parameter umum yang dikenal baik dari "Numerical Recipes in C"
            _modulus = 2147483647; // 2^31 - 1 (bilangan prima Mersenne)
            _multiplier = 16807;
            _increment = 0; // Kadang-kadang disebut Multiplicative Congruential Generator jika c = 0
            
            _seed = seed % _modulus;
            if (_seed < 0) _seed += _modulus; // Pastikan seed positif jika hasil % adalah negatif
        }

        /// <summary>
        /// Menghasilkan angka acak non-negatif berikutnya.
        /// </summary>
        /// <returns>Angka acak long.</returns>
        public long Next()
        {
            _seed = (_multiplier * _seed + _increment) % _modulus;
            return _seed;
        }

        /// <summary>
        /// Menghasilkan angka acak integer berikutnya dalam rentang [0, maxValue).
        /// </summary>
        /// <param name="maxValue">Batas atas eksklusif.</param>
        /// <returns>Angka acak integer.</returns>
        public int Next(int maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("maxValue harus lebih besar dari 0.");
            }
            return (int)(Next() % maxValue);
        }

        /// <summary>
        /// Menghasilkan angka acak integer berikutnya dalam rentang [0, maxValue).
        /// </summary>
        /// <param name="maxValue">Batas atas eksklusif.</param>
        /// <returns>Angka acak Float.</returns>
        public float Next(float maxValue)
        {
            if (maxValue <= 0)
            {
                throw new ArgumentException("maxValue harus lebih besar dari 0.");
            }
            return (float)(Next() % maxValue);
        }

        /// <summary>
        /// Menghasilkan angka acak integer berikutnya dalam rentang [minValue, maxValue).
        /// </summary>
        /// <param name="minValue">Batas bawah inklusif.</param>
        /// <param name="maxValue">Batas atas eksklusif.</param>
        /// <returns>Angka acak integer.</returns>
        public int Next(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("minValue harus lebih kecil dari maxValue.");
            }
            long range = (long)maxValue - minValue;
            return (int)(minValue + (Next() % range));
        }

        /// <summary>
        /// Menghasilkan angka acak integer berikutnya dalam rentang [minValue, maxValue).
        /// </summary>
        /// <param name="minValue">Batas bawah inklusif.</param>
        /// <param name="maxValue">Batas atas eksklusif.</param>
        /// <returns>Angka acak Float.</returns>
        public float Next(float minValue, float maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentException("minValue harus lebih kecil dari maxValue.");
            }
            double range = (double)maxValue - minValue;
            return (float)(minValue + (Next() % range));
        }

        /// <summary>
        /// Menghasilkan angka acak double berikutnya dalam rentang [0.0, 1.0).
        /// </summary>
        /// <returns>Angka acak double.</returns>
        public double NextDouble()
        {
            return (double)Next() / _modulus;
        }

        /// <summary>
        /// Mereset generator ke seed awal.
        /// Catatan: Jika menggunakan konstruktor tanpa parameter, ini akan mereset ke seed yang diberikan saat inisialisasi.
        /// </summary>
        /// <param name="newSeed">Seed baru untuk mereset.</param>
        public void Reset(long newSeed)
        {
            _seed = newSeed % _modulus;
            if (_seed < 0) _seed += _modulus;
        }
    } 
}