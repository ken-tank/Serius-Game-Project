using UnityEngine;
using UnityEngine.UI;

namespace KenTank.Systems.UI.Procedural
{
    // Memastikan komponen ini memiliki sebuah CanvasRenderer, yang diperlukan untuk menggambar UI kustom.
    [AddComponentMenu("KenTank/UI/Effect/Shadow")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class RoundedRectShadowUI : MaskableGraphic
    {
        [SerializeField]
        private float m_Radius = 10f;
        /// <summary>
        /// Radius sudut membulat untuk bayangan. Nilai harus positif.
        /// </summary>
        public float radius
        {
            get { return m_Radius; }
            set
            {
                // Pemeriksaan manual apakah nilai berubah sebelum memperbarui dan memanggil SetVerticesDirty().
                if (m_Radius != value)
                {
                    m_Radius = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField]
        private Color m_ShadowColor = new Color(0f, 0f, 0f, 0.5f);
        /// <summary>
        /// Warna bayangan. Termasuk opasitas (alpha) untuk mengontrol transparansi.
        /// Ini adalah properti warna yang relevan untuk komponen ini.
        /// </summary>
        public Color shadowColor
        {
            get { return m_ShadowColor; }
            set
            {
                // Pemeriksaan manual apakah nilai berubah sebelum memperbarui dan memanggil SetVerticesDirty().
                if (m_ShadowColor != value)
                {
                    m_ShadowColor = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField]
        private Vector2 m_ShadowOffset = new Vector2(5f, -5f);
        /// <summary>
        /// Offset posisi bayangan. Positif X ke kanan, positif Y ke atas.
        /// Ini adalah posisi 'inti' bayangan yang menjadi pusat penyebaran blur.
        /// </summary>
        public Vector2 shadowOffset
        {
            get { return m_ShadowOffset; }
            set
            {
                // Pemeriksaan manual apakah nilai berubah sebelum memperbarui dan memanggil SetVerticesDirty().
                if (m_ShadowOffset != value)
                {
                    m_ShadowOffset = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField]
        [Range(0, 20)] // Range diubah dari 10 menjadi 20 untuk spread yang lebih agresif.
        private int m_ShadowSpread = 3; 
        /// <summary>
        /// Mengontrol seberapa "menyebar" bayangan, mensimulasikan efek blur dengan menggambar beberapa lapisan.
        /// Nilai 0 akan menghasilkan bayangan tunggal yang tajam. Nilai lebih tinggi membuat bayangan lebih lembut dan menyebar.
        /// </summary>
        public int shadowSpread
        {
            get { return m_ShadowSpread; }
            set
            {
                // Pemeriksaan manual apakah nilai berubah sebelum memperbarui dan memanggil SetVerticesDirty().
                if (m_ShadowSpread != value)
                {
                    m_ShadowSpread = value;
                    SetVerticesDirty();
                }
            }
        }

        [SerializeField]
        [Range(4, 32)]
        private int m_Segments = 16; 
        /// <summary>
        /// Jumlah segmen yang digunakan untuk menggambar setiap sudut membulat pada bayangan.
        /// Nilai yang lebih tinggi menghasilkan sudut yang lebih halus, tetapi menggunakan lebih banyak verteks.
        /// </summary>
        public int segments
        {
            get { return m_Segments; }
            set
            {
                // Pemeriksaan manual apakah nilai berubah sebelum memperbarui dan memanggil SetVerticesDirty().
                if (m_Segments != value)
                {
                    m_Segments = value;
                    SetVerticesDirty();
                }
            }
        }

        /// <summary>
        /// Dipanggil oleh sistem UI saat mesh perlu dibuat ulang atau diperbarui.
        /// Di sinilah kita mendefinisikan geometri (verteks dan segitiga) untuk bayangan.
        /// </summary>
        /// <param name="vh">VertexHelper yang digunakan untuk menambahkan verteks dan indeks segitiga.</param>
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear(); // Bersihkan semua verteks dan indeks sebelumnya dari mesh.

            Rect rect = GetPixelAdjustedRect(); // Dapatkan ukuran persegi panjang yang disesuaikan piksel dari RectTransform.
            float currentRadius = Mathf.Max(0, radius); // Pastikan radius tidak negatif.
            int currentSegments = Mathf.Max(1, segments); // Pastikan segmen setidaknya 1.

            // Batasi radius agar tidak melebihi setengah dari dimensi terpendek persegi panjang.
            // Ini mencegah sudut tumpang tindih jika ukurannya terlalu kecil.
            currentRadius = Mathf.Min(currentRadius, rect.width / 2f, rect.height / 2f);

            // Hitung pusat efek bayangan. Ini adalah titik di sekitar mana blur/spread akan menyebar secara simetris.
            // Ini adalah pusat RectTransform asli + offset bayangan yang ditentukan pengguna.
            Vector2 shadowEffectCenter = rect.center + m_ShadowOffset;

            // --- Menggambar Bayangan ---
            // Hanya bayangan yang digambar.
            if (m_ShadowSpread > 0)
            {
                for (int i = 0; i < m_ShadowSpread; i++)
                {
                    // normalizedSpread akan bervariasi dari ~0 hingga 1, mewakili seberapa jauh lapisan ini dari inti.
                    float normalizedSpread = (float)(i + 1) / m_ShadowSpread; 
                    
                    Color currentShadowColor = m_ShadowColor;
                    // Kurangi opasitas seiring dengan penyebaran bayangan untuk efek fading yang lebih cepat dan lebih agresif.
                    // Menggunakan Mathf.Pow untuk fading yang tidak linear, membuat bagian luar lebih cepat menghilang.
                    currentShadowColor.a *= Mathf.Pow(1f - normalizedSpread, 2f); // Eksponen 2.0f membuat fading lebih agresif

                    // Hitung seberapa besar ukuran keseluruhan bayangan untuk lapisan ini.
                    // expansionFactor mengontrol seberapa banyak ukuran total bayangan bertambah.
                    float expansionFactor = normalizedSpread * 0.3f; // Misalnya, maks 30% peningkatan ukuran total

                    float expandedWidth = rect.width * (1f + expansionFactor);
                    float expandedHeight = rect.height * (1f + expansionFactor);

                    // Hitung sudut kiri atas untuk persegi panjang yang diperluas,
                    // memastikan tetap terpusat di sekitar shadowEffectCenter.
                    float shadowRectX = shadowEffectCenter.x - expandedWidth / 2f;
                    float shadowRectY = shadowEffectCenter.y - expandedHeight / 2f;

                    Rect shadowRect = new Rect(shadowRectX, shadowRectY, expandedWidth, expandedHeight);
                    
                    // Juga tingkatkan radius sudut untuk blur yang lebih besar.
                    // radiusSpreadAmount mengontrol seberapa banyak pembulatan sudut meningkat.
                    float radiusSpreadAmount = normalizedSpread * currentRadius * 0.7f; // Misalnya, maks 70% dari radius asli ditambahkan
                    
                    AddRoundedRectGeometry(vh, shadowRect, currentRadius + radiusSpreadAmount, currentShadowColor, currentSegments);
                }
            }
            else
            {
                // Jika 'shadowSpread' adalah 0, gambar satu bayangan tunggal yang tajam.
                // Posisi bayangan tunggal ini harus tetap tepat di offset yang ditentukan pengguna.
                Rect shadowRect = new Rect(rect.x + m_ShadowOffset.x, rect.y + m_ShadowOffset.y, rect.width, rect.height);
                AddRoundedRectGeometry(vh, shadowRect, currentRadius, m_ShadowColor, currentSegments);
            }
        }

        /// <summary>
        /// Helper method untuk menambahkan geometri persegi panjang membulat ke VertexHelper.
        /// Ini membangun mesh menggunakan 3 bagian: persegi panjang tengah, 4 sisi lurus, dan 4 sudut membulat.
        /// </summary>
        /// <param name="vh">VertexHelper untuk menambahkan verteks dan indeks.</param>
        /// <param name="rect">Ukuran dan posisi persegi panjang.</param>
        /// <param name="radius">Radius sudut membulat.</param>
        /// <param name="color">Warna objek.</param>
        /// <param name="segments">Jumlah segmen per sudut.</param>
        private void AddRoundedRectGeometry(VertexHelper vh, Rect rect, float radius, Color color, int segments)
        {
            // Dapatkan indeks awal untuk batch verteks ini
            int currentBaseIndex = vh.currentVertCount;

            // Hitung titik-titik pusat untuk setiap sudut (inner corners) dari persegi panjang yang tidak membulat.
            Vector2 bl_inner = new Vector2(rect.x + radius, rect.y + radius);
            Vector2 br_inner = new Vector2(rect.x + rect.width - radius, rect.y + radius);
            Vector2 tr_inner = new Vector2(rect.x + rect.width - radius, rect.y + rect.height - radius);
            Vector2 tl_inner = new Vector2(rect.x + radius, rect.y + rect.height - radius);

            // --- 1. Bagian Tengah Persegi Panjang (solid quad) ---
            // Verteks 0-3 dari batch saat ini.
            vh.AddVert(bl_inner, color, Vector2.zero); // 0 (Bottom-Left inner)
            vh.AddVert(br_inner, color, Vector2.zero); // 1 (Bottom-Right inner)
            vh.AddVert(tr_inner, color, Vector2.zero); // 2 (Top-Right inner)
            vh.AddVert(tl_inner, color, Vector2.zero); // 3 (Top-Left inner)
            
            // Tambahkan indeks segitiga untuk membentuk persegi panjang tengah.
            vh.AddTriangle(currentBaseIndex + 0, currentBaseIndex + 1, currentBaseIndex + 2);
            vh.AddTriangle(currentBaseIndex + 2, currentBaseIndex + 3, currentBaseIndex + 0);

            // --- 2. Empat Sisi Lurus (quads) ---
            // Sisi Bawah
            int v_bl_outer_bottom = vh.currentVertCount; vh.AddVert(new Vector2(bl_inner.x, rect.y), color, Vector2.zero); // 4
            int v_br_outer_bottom = vh.currentVertCount; vh.AddVert(new Vector2(br_inner.x, rect.y), color, Vector2.zero); // 5
            vh.AddTriangle(currentBaseIndex + 0, v_bl_outer_bottom, v_br_outer_bottom);
            vh.AddTriangle(currentBaseIndex + 0, v_br_outer_bottom, currentBaseIndex + 1);

            // Sisi Kanan
            int v_br_outer_right = vh.currentVertCount; vh.AddVert(new Vector2(rect.x + rect.width, br_inner.y), color, Vector2.zero); // 6
            int v_tr_outer_right = vh.currentVertCount; vh.AddVert(new Vector2(rect.x + rect.width, tr_inner.y), color, Vector2.zero); // 7
            vh.AddTriangle(currentBaseIndex + 1, v_br_outer_right, v_tr_outer_right);
            vh.AddTriangle(currentBaseIndex + 1, v_tr_outer_right, currentBaseIndex + 2);

            // Sisi Atas
            int v_tr_outer_top = vh.currentVertCount; vh.AddVert(new Vector2(tr_inner.x, rect.y + rect.height), color, Vector2.zero); // 8
            int v_tl_outer_top = vh.currentVertCount; vh.AddVert(new Vector2(tl_inner.x, rect.y + rect.height), color, Vector2.zero); // 9
            vh.AddTriangle(currentBaseIndex + 2, v_tr_outer_top, v_tl_outer_top);
            vh.AddTriangle(currentBaseIndex + 2, v_tl_outer_top, currentBaseIndex + 3);

            // Sisi Kiri
            int v_tl_outer_left = vh.currentVertCount; vh.AddVert(new Vector2(rect.x, tl_inner.y), color, Vector2.zero); // 10
            int v_bl_outer_left = vh.currentVertCount; vh.AddVert(new Vector2(rect.x, bl_inner.y), color, Vector2.zero); // 11
            vh.AddTriangle(currentBaseIndex + 3, v_tl_outer_left, v_bl_outer_left);
            vh.AddTriangle(currentBaseIndex + 3, v_bl_outer_left, currentBaseIndex + 0);

            // --- 3. Empat Sudut Membulat (menggunakan triangle fan) ---
            // Parameter untuk setiap sudut: (center, radius, color, segments, startAngle, fanCenterIndex, startEdgeVertexIndex, endEdgeVertexIndex)
            // fanCenterIndex adalah indeks ke inner corner yang sesuai.
            // startEdgeVertexIndex dan endEdgeVertexIndex adalah indeks ke verteks di sisi lurus yang berdekatan.

            // Sudut Bawah Kiri (BL)
            AddCornerArc(vh, bl_inner, radius, color, segments, 180f, 
                        currentBaseIndex + 0,      // fanCenterIndex: bl_inner
                        v_bl_outer_left,           // startEdgeVertexIndex: (rect.x, rect.y+radius)
                        v_bl_outer_bottom          // endEdgeVertexIndex: (rect.x+radius, rect.y)
            );

            // Sudut Bawah Kanan (BR)
            AddCornerArc(vh, br_inner, radius, color, segments, 270f, 
                        currentBaseIndex + 1,      // fanCenterIndex: br_inner
                        v_br_outer_bottom,         // startEdgeVertexIndex: (rect.x+width-radius, rect.y)
                        v_br_outer_right           // endEdgeVertexIndex: (rect.x+width, rect.y+radius)
            );

            // Sudut Atas Kanan (TR)
            AddCornerArc(vh, tr_inner, radius, color, segments, 0f, // 0 derajat adalah arah +X
                        currentBaseIndex + 2,      // fanCenterIndex: tr_inner
                        v_tr_outer_right,          // startEdgeVertexIndex: (rect.x+width, rect.y+height-radius)
                        v_tr_outer_top             // endEdgeVertexIndex: (rect.x+width-radius, rect.y+height)
            );

            // Sudut Atas Kiri (TL)
            AddCornerArc(vh, tl_inner, radius, color, segments, 90f, 
                        currentBaseIndex + 3,      // fanCenterIndex: tl_inner
                        v_tl_outer_top,            // startEdgeVertexIndex: (rect.x+radius, rect.y+height)
                        v_tl_outer_left            // endEdgeVertexIndex: (rect.x, rect.y+height-radius)
            );
        }

        /// <summary>
        /// Helper method untuk menambahkan geometri busur sudut membulat (menggunakan triangle fan).
        /// </summary>
        /// <param name="vh">VertexHelper.</param>
        /// <param name="center">Pusat lingkaran untuk busur (yaitu, sudut dalam persegi panjang).</param>
        /// <param name="radius">Radius busur.</param>
        /// <param name="color">Warna.</param>
        /// <param name="segments">Jumlah segmen per kuartal lingkaran.</param>
        /// <param name="startAngle">Sudut awal busur dalam derajat (mis. 180f untuk BL, 270f untuk BR).</param>
        /// <param name="fanCenterIndex">Indeks verteks pusat fan (sudut dalam persegi panjang).</param>
        /// <param name="startEdgeVertexIndex">Indeks verteks awal pada busur (titik pada sisi lurus yang berdekatan).</param>
        /// <param name="endEdgeVertexIndex">Indeks verteks akhir pada busur (titik pada sisi lurus yang berdekatan).</param>
        private void AddCornerArc(VertexHelper vh, Vector2 center, float radius, Color color, int segments, float startAngle, 
                                int fanCenterIndex, int startEdgeVertexIndex, int endEdgeVertexIndex)
        {
            // Verteks pertama fan sudah ada (fanCenterIndex).
            // Titik awal busur (sudah ada di sisi lurus) akan menjadi verteks sebelumnya untuk segitiga pertama.
            int prevArcVertexIndex = startEdgeVertexIndex; 

            // Loop untuk membuat segmen sudut (membentuk busur dan segitiga fan).
            for (int i = 1; i <= segments; i++)
            {
                // Hitung sudut untuk segmen saat ini. Busur adalah 90 derajat.
                float angle = startAngle + (i / (float)segments) * 90f; 
                float rad = angle * Mathf.Deg2Rad; // Konversi ke radian.
                
                // Hitung posisi verteks baru pada busur.
                Vector2 currentVertexPos = center + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
                
                vh.AddVert(currentVertexPos, color, Vector2.zero); // Tambahkan verteks baru pada busur.
                int newArcVertexIndex = vh.currentVertCount - 1; // Indeks verteks yang baru ditambahkan.

                // Tambahkan segitiga untuk kipas sudut.
                // Segitiga dibentuk dari: pusat fan, verteks sebelumnya di busur, dan verteks saat ini di busur.
                vh.AddTriangle(fanCenterIndex, prevArcVertexIndex, newArcVertexIndex);
                
                prevArcVertexIndex = newArcVertexIndex; // Perbarui prevArcVertexIndex untuk iterasi selanjutnya.
            }

            // Tambahkan segitiga terakhir untuk menghubungkan ke titik akhir busur yang sudah ada di sisi lurus.
            vh.AddTriangle(fanCenterIndex, prevArcVertexIndex, endEdgeVertexIndex);
        }
    }
}