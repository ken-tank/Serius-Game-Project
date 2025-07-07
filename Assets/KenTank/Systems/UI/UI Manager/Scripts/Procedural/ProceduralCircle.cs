using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; 

#if UNITY_EDITOR
using UnityEditor; 
#endif

namespace KenTank.Systems.UI.Procedural
{
    [AddComponentMenu("KenTank/UI/Shape/Circle")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class ProceduralCircle : MaskableGraphic
    {
        public enum CircleFillMethod
        {
            FullCircle, 
            Radial360 
        }

        public enum CircleFillOrigin
        {
            Bottom,
            Right,
            Top,
            Left
        }

        public enum OutlinePosition
        {
            Inner,
            Center,
            Outer
        }

        [Header("Main Settings")]
        [Tooltip("Whether the circle should be filled with the main color.")]
        public bool filled = true; 

        [Tooltip("Number of segments used to draw the circle. Higher values result in a smoother circle but more vertices.")]
        [Range(3, 256)] 
        public int segments = 64;

        [Header("Fill Settings")]
        [Tooltip("Method used to fill the circle.")]
        public CircleFillMethod fillMethod = CircleFillMethod.FullCircle; 

        [Tooltip("Amount of the circle to fill (0.0 to 1.0). Only applicable for Radial360 fill method.")]
        [Range(0f, 1f)]
        public float fillAmount = 1f;

        [Tooltip("Origin point for radial filling. Only applicable for Radial360 fill method.")]
        public CircleFillOrigin fillOrigin = CircleFillOrigin.Bottom;

        [Tooltip("Direction of radial filling. Only applicable for Radial360 fill method.")]
        public bool fillClockwise = true;

        [Header("Outline Settings")]
        [Tooltip("Whether to draw an outline.")]
        public bool drawOutline = false;
        [Tooltip("The thickness of the outline.")]
        public float outlineThickness = 2f;
        public Color outlineColor = Color.black;
        [Tooltip("The position of the outline relative to the shape's boundaries: Inner, Center, or Outer.")]
        public OutlinePosition outlinePosition = OutlinePosition.Center;
        
        // Properti ini sekarang akan mengontrol kehalusan lingkaran cap, bukan bentuk busur
        [Tooltip("Number of segments for the rounded end caps of the radial outline. Only applies to Radial360 fill.")]
        [Range(3, 32)] // Minimal 3 segmen untuk lingkaran
        public int radialOutlineCapSegments = 8; // Default 8 untuk kebulatan yang layak

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            float rectWidth = rectTransform.rect.width;
            float rectHeight = rectTransform.rect.height;

            if (rectWidth <= 0 || rectHeight <= 0) return;
            if (segments < 3) segments = 3;
            if (outlineThickness < 0) outlineThickness = 0;
            if (radialOutlineCapSegments < 3) radialOutlineCapSegments = 3; // Ensure minimum for a circle
            
            float currentRadius = Mathf.Min(rectWidth / 2f, rectHeight / 2f);

            // --- PERUBAHAN UTAMA DI SINI ---
            // Hitung pusat relatif terhadap pivot
            // RectTransform.pivot adalah 0-1, di mana (0,0) adalah kiri bawah, (0.5,0.5) tengah, (1,1) kanan atas.
            // RectTransform.rect.min adalah sudut kiri bawah dari persegi panjang lokal.
            // RectTransform.rect.max adalah sudut kanan atas dari persegi panjang lokal.
            // RectTransform.rect.size adalah lebar dan tinggi.
            // Untuk mendapatkan posisi pivot dalam koordinat lokal, kita bisa menggunakan:
            // (pivot.x * width) + rect.xMin
            // (pivot.y * height) + rect.yMin
            Vector2 pivotOffset = new Vector2(
                rectTransform.pivot.x * rectWidth + rectTransform.rect.xMin,
                rectTransform.pivot.y * rectHeight + rectTransform.rect.yMin
            );

            // Karena kita ingin menggambar relatif terhadap pusat RectTransform (yang biasanya di 0,0)
            // dan pivot menggeser titik rotasi/skala, kita perlu menggeser pusat lingkaran
            // agar secara visual tetap di lokasi yang sama meskipun pivot berubah.
            // Sebenarnya, menggambar dari (0,0) sudah benar karena vh.AddVert() menggunakan koordinat lokal
            // yang di-transformasi oleh RectTransform di akhir.
            // Namun, jika kita ingin lingkaran selalu "tertahan" di pivot secara visual, 
            // maka pusat lingkaran harus dihitung dari pivot.
            // Untuk MaskableGraphic, pusat lokal 0,0 adalah pusat RectTransform.
            // Jadi, jika pivot di (0.5, 0.5), maka 0,0 adalah pusatnya.
            // Jika pivot di (0,0), maka 0,0 adalah sudut kiri bawah.
            // Untuk membuatnya sesuai dengan "rectTransform.pivot", kita perlu mengkompensasi
            // pergeseran yang diberikan oleh pivot itu sendiri relatif terhadap pusat RectTransform.
            // Jika pivot adalah (0.5, 0.5), offset adalah (0,0).
            // Jika pivot adalah (0,0), offset adalah (-width/2, -height/2).
            Vector2 center = new Vector2(
                (0.5f - rectTransform.pivot.x) * rectWidth,
                (0.5f - rectTransform.pivot.y) * rectHeight
            );
            // --- AKHIR PERUBAHAN UTAMA ---

            float startAngleRad, endAngleRad;
            bool isPartialFill = (fillMethod == CircleFillMethod.Radial360 && fillAmount < 1f);

            // Tambahkan kondisi untuk fillAmount 0 atau 1
            // Jika fillAmount sangat mendekati 0 atau 1, perlakukan sebagai lingkaran penuh/kosong.
            // Ini akan mematikan caps dan jika fillAmount ~0, juga mematikan outline.
            bool isEffectivelyFullOrEmpty = (fillAmount <= 0.001f || fillAmount >= 0.999f);

            if (isPartialFill && isEffectivelyFullOrEmpty)
            {
                isPartialFill = false; // Perlakukan sebagai lingkaran penuh/kosong, jangan gambar caps.
                                        // Ini juga akan menyebabkan outline digambar penuh jika filled=true dan fillAmount~1,
                                        // atau tidak digambar sama sekali jika fillAmount~0.
            }


            if (isPartialFill)
            {
                CalculateRadialFillAngles(fillOrigin, fillClockwise, fillAmount, out startAngleRad, out endAngleRad);
            }
            else
            {
                startAngleRad = 0;
                endAngleRad = Mathf.PI * 2;
            }

            if (filled)
            {
                // Jika fillMethod Radial360 dan fillAmount ~0, jangan gambar isian sama sekali
                if (!(fillMethod == CircleFillMethod.Radial360 && fillAmount <= 0.001f))
                {
                    DrawArcFilled(vh, center, currentRadius, segments, startAngleRad, endAngleRad, color); 
                }
            }

            if (drawOutline && outlineThickness > 0)
            {
                // KONDISI BARU: JANGAN GAMBAR OUTLINE UTUH JIKA fillAmount ~0 dan fillMethod adalah Radial360
                // Jika isiannya kosong, maka outlinenya juga harus kosong (kecuali itu adalah partial fill)
                if (fillMethod == CircleFillMethod.Radial360 && fillAmount <= 0.001f)
                {
                    // Jangan gambar outline sama sekali jika fillAmount adalah 0 untuk Radial360
                    // Kita hanya akan menggambar outline jika ada bagian yang terisi
                }
                else 
                {
                    float innerOutlineOffset = 0f; 
                    float outerOutlineOffset = 0f; 

                    switch (outlinePosition)
                    {
                        case OutlinePosition.Inner:
                            innerOutlineOffset = outlineThickness;
                            outerOutlineOffset = 0f; 
                            break;
                        case OutlinePosition.Center:
                            innerOutlineOffset = outlineThickness / 2f;
                            outerOutlineOffset = outlineThickness / 2f;
                            break;
                        case OutlinePosition.Outer:
                            innerOutlineOffset = 0f; 
                            outerOutlineOffset = outlineThickness;
                            break;
                    }

                    float innerRadius = Mathf.Max(0, currentRadius - innerOutlineOffset);
                    float outerRadius = currentRadius + outerOutlineOffset;

                    // Draw the main arc outline
                    DrawOutlineArc(vh, center, innerRadius, outerRadius, segments, startAngleRad, endAngleRad, outlineColor);

                    // Draw the radial "caps" ONLY if it's a partial fill AND not a full/empty circle
                    // isPartialFill sudah disaring untuk 0 dan 1 di atas, jadi ini akan bekerja dengan benar.
                    if (isPartialFill) 
                    {
                        // Radius of the cap circle will be half the outline thickness
                        float capRadius = outlineThickness / 2f;

                        float capCircleDistFromCenter = 0f;
                        switch (outlinePosition)
                        {
                            case OutlinePosition.Inner:
                                capCircleDistFromCenter = innerRadius + capRadius;
                                break;
                            case OutlinePosition.Center:
                                capCircleDistFromCenter = currentRadius; 
                                break;
                            case OutlinePosition.Outer:
                                capCircleDistFromCenter = outerRadius - capRadius;
                                break;
                        }
                        
                        // Ensure the cap radius doesn't exceed the available space for inner outlines
                        if (outlinePosition == OutlinePosition.Inner && innerRadius < capRadius)
                        {
                            capRadius = innerRadius; 
                            if (innerRadius > 0)
                            {
                                capCircleDistFromCenter = innerRadius / 2f; 
                            }
                            else
                            {
                                capCircleDistFromCenter = 0; 
                            }
                        }

                        // Start Cap - a small filled circle
                        Vector2 startCapCenter = new Vector2(center.x + capCircleDistFromCenter * Mathf.Cos(startAngleRad), center.y + capCircleDistFromCenter * Mathf.Sin(startAngleRad));
                        DrawArcFilled(vh, startCapCenter, capRadius, radialOutlineCapSegments, 0, Mathf.PI * 2, outlineColor);

                        // End Cap - a small filled circle
                        Vector2 endCapCenter = new Vector2(center.x + capCircleDistFromCenter * Mathf.Cos(endAngleRad), center.y + capCircleDistFromCenter * Mathf.Sin(endAngleRad));
                        DrawArcFilled(vh, endCapCenter, capRadius, radialOutlineCapSegments, 0, Mathf.PI * 2, outlineColor);
                    }
                }
            }
        }

        private void CalculateRadialFillAngles(CircleFillOrigin origin, bool clockwise, float amount, out float startAngleRad, out float endAngleRad)
        {
            float originAngleRad = 0;
            switch (origin)
            {
                case CircleFillOrigin.Bottom: originAngleRad = Mathf.PI * 0.5f; break; 
                case CircleFillOrigin.Right:  originAngleRad = 0; break;           
                case CircleFillOrigin.Top:    originAngleRad = Mathf.PI * 1.5f; break; 
                case CircleFillOrigin.Left:   originAngleRad = Mathf.PI; break;    
            }

            float fillAngleRad = amount * Mathf.PI * 2f;

            if (clockwise)
            {
                startAngleRad = originAngleRad;
                endAngleRad = originAngleRad - fillAngleRad;
            }
            else
            {
                startAngleRad = originAngleRad;
                endAngleRad = originAngleRad + fillAngleRad;
            }
        }

        // Renamed from DrawCircleFilled to DrawArcFilled for clarity
        private void DrawArcFilled(VertexHelper vh, Vector2 center, float currentRadius, int numSegments, float startAngleRad, float endAngleRad, Color vertexColor)
        {
            if (currentRadius <= 0) return;

            UIVertex vert = UIVertex.simpleVert;
            vert.color = vertexColor;

            int centerIdx = vh.currentVertCount;
            vert.position = center;
            vh.AddVert(vert);

            float angleRange = endAngleRad - startAngleRad;
            float angleStep = angleRange / numSegments;

            for (int i = 0; i <= numSegments; i++)
            {
                float angle = startAngleRad + angleStep * i;
                vert.position = new Vector2(center.x + currentRadius * Mathf.Cos(angle), center.y + currentRadius * Mathf.Sin(angle));
                vh.AddVert(vert);
            }

            for (int i = 0; i < numSegments; i++)
            {
                vh.AddTriangle(centerIdx, centerIdx + 1 + i, centerIdx + 1 + i + 1);
            }
        }

        private void DrawOutlineArc(VertexHelper vh, Vector2 center, float innerRadius, float outerRadius, int numSegments, float startAngleRad, float endAngleRad, Color outlineColor)
        {
            if (innerRadius <= 0 && outerRadius <= 0) return; 

            List<UIVertex> arcVertices = new List<UIVertex>();
            UIVertex tempVert = UIVertex.simpleVert;
            tempVert.color = outlineColor;

            float angleRange = endAngleRad - startAngleRad;
            float angleStep = angleRange / numSegments;

            // Add vertices for the inner arc
            for (int i = 0; i <= numSegments; i++)
            {
                float angle = startAngleRad + angleStep * i;
                tempVert.position = new Vector2(center.x + innerRadius * Mathf.Cos(angle), center.y + innerRadius * Mathf.Sin(angle));
                arcVertices.Add(tempVert);
            }

            // Add vertices for the outer arc
            for (int i = 0; i <= numSegments; i++)
            {
                float angle = startAngleRad + angleStep * i;
                tempVert.position = new Vector2(center.x + outerRadius * Mathf.Cos(angle), center.y + outerRadius * Mathf.Sin(angle));
                arcVertices.Add(tempVert);
            }

            int baseIndex = vh.currentVertCount; 
            foreach(UIVertex v in arcVertices)
            {
                vh.AddVert(v);
            }

            for (int i = 0; i < numSegments; i++)
            {
                vh.AddUIVertexQuad(new UIVertex[] {
                    arcVertices[i],         
                    arcVertices[i + 1],         
                    arcVertices[numSegments + 1 + i + 1],   
                    arcVertices[numSegments + 1 + i]        
                });
            }
        }

        private UIVertex UIVertexFromPos(Vector2 position, Color color)
        {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.color = color;
            return vertex;
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            segments = Mathf.Max(3, segments); 
            fillAmount = Mathf.Clamp01(fillAmount); 
            radialOutlineCapSegments = Mathf.Max(3, radialOutlineCapSegments); 
            SetVerticesDirty();
        }
        #endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(ProceduralCircle))]
    public class ProceduralCircleEditor : Editor 
    {
        private SerializedProperty m_Color;
        private SerializedProperty m_Material; 
        private SerializedProperty m_RaycastTarget; 
        private SerializedProperty m_Maskable;      

        private SerializedProperty m_Filled; 
        private SerializedProperty m_Segments;
        private SerializedProperty m_FillMethod;
        private SerializedProperty m_FillAmount;
        private SerializedProperty m_FillOrigin;
        private SerializedProperty m_FillClockwise;

        private SerializedProperty m_DrawOutline;
        private SerializedProperty m_OutlineThickness;
        private SerializedProperty m_OutlineColor;
        private SerializedProperty m_OutlinePosition;
        private SerializedProperty m_RadialOutlineCapSegments; 

        protected virtual void OnEnable() 
        {
            m_Color = serializedObject.FindProperty("m_Color"); 
            m_Material = serializedObject.FindProperty("m_Material"); 
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget"); 
            m_Maskable = serializedObject.FindProperty("m_Maskable");          
            
            m_Filled = serializedObject.FindProperty("filled"); 
            m_Segments = serializedObject.FindProperty("segments");
            m_FillMethod = serializedObject.FindProperty("fillMethod");
            m_FillAmount = serializedObject.FindProperty("fillAmount");
            m_FillOrigin = serializedObject.FindProperty("fillOrigin");
            m_FillClockwise = serializedObject.FindProperty("fillClockwise");

            m_DrawOutline = serializedObject.FindProperty("drawOutline");
            m_OutlineThickness = serializedObject.FindProperty("outlineThickness");
            m_OutlineColor = serializedObject.FindProperty("outlineColor");
            m_OutlinePosition = serializedObject.FindProperty("outlinePosition");
            m_RadialOutlineCapSegments = serializedObject.FindProperty("radialOutlineCapSegments"); 
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update(); 

            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
            GUI.enabled = true;

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Graphic Base Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Color, new GUIContent("Color")); 
            EditorGUILayout.PropertyField(m_Material, new GUIContent("Material")); 
            EditorGUILayout.PropertyField(m_RaycastTarget, new GUIContent("Raycast Target"));
            EditorGUILayout.PropertyField(m_Maskable, new GUIContent("Maskable"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Circle Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Filled, new GUIContent("Filled")); 
            EditorGUILayout.PropertyField(m_Segments, new GUIContent("Segments"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Fill Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_FillMethod, new GUIContent("Fill Method"));

            ProceduralCircle.CircleFillMethod currentFillMethod = (ProceduralCircle.CircleFillMethod)m_FillMethod.enumValueIndex;

            if (currentFillMethod == ProceduralCircle.CircleFillMethod.Radial360)
            {
                EditorGUILayout.PropertyField(m_FillAmount, new GUIContent("Fill Amount"));
                EditorGUILayout.PropertyField(m_FillOrigin, new GUIContent("Fill Origin"));
                EditorGUILayout.PropertyField(m_FillClockwise, new GUIContent("Clockwise"));
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Outline Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_DrawOutline, new GUIContent("Draw Outline"));

            if (m_DrawOutline.boolValue)
            {
                EditorGUILayout.PropertyField(m_OutlineThickness, new GUIContent("Outline Thickness"));
                if (m_OutlineThickness.floatValue < 0) m_OutlineThickness.floatValue = 0;

                EditorGUILayout.PropertyField(m_OutlineColor, new GUIContent("Outline Color"));
                EditorGUILayout.PropertyField(m_OutlinePosition, new GUIContent("Outline Position"));

                if (currentFillMethod == ProceduralCircle.CircleFillMethod.Radial360) 
                {
                    EditorGUILayout.PropertyField(m_RadialOutlineCapSegments, new GUIContent("Radial Cap Segments"));
                }
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                ((ProceduralCircle)target).SetVerticesDirty(); 
            }
        }
    }
    #endif
}