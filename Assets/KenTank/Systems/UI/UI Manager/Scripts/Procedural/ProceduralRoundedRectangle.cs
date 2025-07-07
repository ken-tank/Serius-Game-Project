using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; 

#if UNITY_EDITOR
using UnityEditor; 
#endif

namespace KenTank.Systems.UI.Procedural
{
    [AddComponentMenu("KenTank/UI/Shape/Rectangle")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class ProceduralRoundedRectangle : MaskableGraphic
    {
        public enum CornerRadiusMode
        {
            Universal,
            Individual
        }

        public enum OutlinePosition
        {
            Inner,
            Center,
            Outer
        }

        [Header("Main Settings")]
        [Tooltip("Whether the shape should be filled with the main color or only an outline.")]
        public bool filled = true;

        [Header("Corner Settings")]
        public CornerRadiusMode cornerMode = CornerRadiusMode.Universal;

        [Tooltip("Radius for all corners if in Universal mode.")]
        public float universalCornerRadius = 20f;

        [Tooltip("Radius for each corner individually if in Individual mode.")]
        public float topLeftRadius = 20f;
        public float topRightRadius = 20f;
        public float bottomRightRadius = 20f;
        public float bottomLeftRadius = 20f;

        [Tooltip("Number of segments for each corner arc. Higher values result in smoother corners but more vertices.")]
        [Range(1, 64)]
        public int segmentsPerCorner = 8;

        [Header("Outline Settings")]
        [Tooltip("Whether to draw an outline.")]
        public bool drawOutline = false;
        [Tooltip("The thickness of the outline.")]
        public float outlineThickness = 2f;
        public Color outlineColor = Color.black;
        [Tooltip("The position of the outline relative to the shape's boundaries: Inner, Center, or Outer.")]
        public OutlinePosition outlinePosition = OutlinePosition.Center;


        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            // rectWidth and rectHeight are already correct from RectTransform.rect
            float rectWidth = rectTransform.rect.width;
            float rectHeight = rectTransform.rect.height;

            if (rectWidth <= 0 || rectHeight <= 0) return;
            if (segmentsPerCorner < 1) segmentsPerCorner = 1;
            if (outlineThickness < 0) outlineThickness = 0; 

            // Ensure radii don't exceed half of the smallest dimension
            float minHalfDim = Mathf.Min(rectWidth, rectHeight) / 2f;

            float actualTopLeftRadius = (cornerMode == CornerRadiusMode.Universal) ? universalCornerRadius : topLeftRadius;
            float actualTopRightRadius = (cornerMode == CornerRadiusMode.Universal) ? universalCornerRadius : topRightRadius;
            float actualBottomRightRadius = (cornerMode == CornerRadiusMode.Universal) ? universalCornerRadius : bottomRightRadius;
            float actualBottomLeftRadius = (cornerMode == CornerRadiusMode.Universal) ? universalCornerRadius : bottomLeftRadius;

            // Clamp radii to prevent corners from overlapping or exceeding bounds
            actualTopLeftRadius = Mathf.Min(actualTopLeftRadius, minHalfDim);
            actualTopRightRadius = Mathf.Min(actualTopRightRadius, minHalfDim);
            actualBottomRightRadius = Mathf.Min(actualBottomRightRadius, minHalfDim);
            actualBottomLeftRadius = Mathf.Min(actualBottomLeftRadius, minHalfDim);

            if (filled)
            {
                DrawFilledRectangle(vh, 
                                    actualTopLeftRadius, actualTopRightRadius, actualBottomRightRadius, actualBottomLeftRadius,
                                    segmentsPerCorner, color);
            }

            if (drawOutline && outlineThickness > 0)
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

                DrawOutline(vh, 
                            actualTopLeftRadius, actualTopRightRadius, actualBottomRightRadius, actualBottomLeftRadius,
                            segmentsPerCorner, outlineColor, innerOutlineOffset, outerOutlineOffset);
            }
        }

        /// <summary>
        /// Draws the filled area of the rounded rectangle.
        /// All positions are calculated relative to the RectTransform's local bounds (xMin, yMin, xMax, yMax).
        /// </summary>
        private void DrawFilledRectangle(VertexHelper vh,
                                         float tlRadius, float trRadius, float brRadius, float blRadius,
                                         int segments, Color fillColor)
        {
            // Get the actual bounds of the RectTransform's rect in local space.
            // These values already account for the RectTransform's pivot.
            float xMin = rectTransform.rect.xMin;
            float yMin = rectTransform.rect.yMin;
            float xMax = rectTransform.rect.xMax;
            float yMax = rectTransform.rect.yMax;

            // Calculate center points for the arcs, relative to the rect's actual bounds.
            // These are the centers of the circles from which the corners are cut.
            Vector2 topLeftCenter = new Vector2(xMin + tlRadius, yMax - tlRadius);
            Vector2 topRightCenter = new Vector2(xMax - trRadius, yMax - trRadius);
            Vector2 bottomRightCenter = new Vector2(xMax - brRadius, yMin + brRadius);
            Vector2 bottomLeftCenter = new Vector2(xMin + blRadius, yMin + blRadius);

            List<UIVertex> uiVertices = new List<UIVertex>();
            UIVertex tempVertex = UIVertex.simpleVert;
            tempVertex.color = fillColor;

            // Add the four inner "corners" of the main rectangle part.
            // These points are where the straight edges meet the tangent points of the arcs.
            // Order: Top-Left, Top-Right, Bottom-Right, Bottom-Left
            tempVertex.position = topLeftCenter; uiVertices.Add(tempVertex); // v0
            tempVertex.position = topRightCenter; uiVertices.Add(tempVertex); // v1
            tempVertex.position = bottomRightCenter; uiVertices.Add(tempVertex); // v2
            tempVertex.position = bottomLeftCenter; uiVertices.Add(tempVertex); // v3

            // Create the central rectangular quad using the inner corner points.
            // This forms the core of the filled shape.
            vh.AddUIVertexQuad(new UIVertex[] {
                uiVertices[3], // bottomLeftCenter
                uiVertices[2], // bottomRightCenter
                uiVertices[1], // topRightCenter
                uiVertices[0]  // topLeftCenter
            });

            // Add rectangular sections for the straight edges, connecting the inner corners to the outer edges.

            // Top rectangle (connecting inner top corners to the actual top edge)
            tempVertex.position = new Vector2(xMin + tlRadius, yMax); uiVertices.Add(tempVertex); // v4 (top-left point on top edge)
            tempVertex.position = new Vector2(xMax - trRadius, yMax); uiVertices.Add(tempVertex); // v5 (top-right point on top edge)
            vh.AddUIVertexQuad(new UIVertex[] {
                uiVertices[0], // topLeftCenter
                uiVertices[1], // topRightCenter
                uiVertices[5], // top-right point on top edge
                uiVertices[4]  // top-left point on top edge
            });

            // Bottom rectangle (connecting inner bottom corners to the actual bottom edge)
            tempVertex.position = new Vector2(xMax - brRadius, yMin); uiVertices.Add(tempVertex); // v6 (bottom-right point on bottom edge)
            tempVertex.position = new Vector2(xMin + blRadius, yMin); uiVertices.Add(tempVertex); // v7 (bottom-left point on bottom edge)
            vh.AddUIVertexQuad(new UIVertex[] {
                uiVertices[7], // bottom-left point on bottom edge
                uiVertices[6], // bottom-right point on bottom edge
                uiVertices[2], // bottomRightCenter
                uiVertices[3]  // bottomLeftCenter
            });

            // Right rectangle (connecting inner right corners to the actual right edge)
            tempVertex.position = new Vector2(xMax, yMax - trRadius); uiVertices.Add(tempVertex); // v8 (top-right point on right edge)
            tempVertex.position = new Vector2(xMax, yMin + brRadius); uiVertices.Add(tempVertex); // v9 (bottom-right point on right edge)
            vh.AddUIVertexQuad(new UIVertex[] {
                uiVertices[1], // topRightCenter
                uiVertices[8], // top-right point on right edge
                uiVertices[9], // bottom-right point on right edge
                uiVertices[2]  // bottomRightCenter
            });

            // Left rectangle (connecting inner left corners to the actual left edge)
            tempVertex.position = new Vector2(xMin, yMin + blRadius); uiVertices.Add(tempVertex); // v10 (bottom-left point on left edge)
            tempVertex.position = new Vector2(xMin, yMax - tlRadius); uiVertices.Add(tempVertex); // v11 (top-left point on left edge)
            vh.AddUIVertexQuad(new UIVertex[] {
                uiVertices[10], // bottom-left point on left edge
                uiVertices[11], // top-left point on left edge
                uiVertices[0],  // topLeftCenter
                uiVertices[3]   // bottomLeftCenter
            });

            // Add the arcs for each corner. Each arc is composed of segments of triangles originating from the arc's center.
            // Angles are in degrees, relative to the positive X-axis (right is 0, up is 90).
            AddArc(vh, topLeftCenter, tlRadius, -180f, -270f, segments, fillColor); // Top-Left arc (from left to top)
            AddArc(vh, topRightCenter, trRadius, 90f, 0f, segments, fillColor);     // Top-Right arc (from top to right)
            AddArc(vh, bottomRightCenter, brRadius, 0f, -90f, segments, fillColor); // Bottom-Right arc (from right to bottom)
            AddArc(vh, bottomLeftCenter, blRadius, -90f, -180f, segments, fillColor); // Bottom-Left arc (from bottom to left)
        }

        /// <summary>
        /// Draws the outline of the rounded rectangle.
        /// Calculates inner and outer paths based on the original RectTransform bounds and outline offsets.
        /// </summary>
        private void DrawOutline(VertexHelper vh,
                                 float tlRadius, float trRadius, float brRadius, float blRadius,
                                 int segments, Color outlineCol, float innerOutlineOffset, float outerOutlineOffset)
        {
            // Get the base RectTransform bounds.
            float baseRectXMin = rectTransform.rect.xMin;
            float baseRectYMin = rectTransform.rect.yMin;
            float baseRectXMax = rectTransform.rect.xMax;
            float baseRectYMax = rectTransform.rect.yMax;

            // Calculate the bounds for the inner edge of the outline.
            // These are determined by shrinking the base rectangle by innerOutlineOffset.
            float innerXMin = baseRectXMin + innerOutlineOffset;
            float innerYMin = baseRectYMin + innerOutlineOffset;
            float innerXMax = baseRectXMax - innerOutlineOffset;
            float innerYMax = baseRectYMax - innerOutlineOffset;

            // Calculate the bounds for the outer edge of the outline.
            // These are determined by expanding the base rectangle by outerOutlineOffset.
            float outerXMin = baseRectXMin - outerOutlineOffset;
            float outerYMin = baseRectYMin - outerOutlineOffset;
            float outerXMax = baseRectXMax + outerOutlineOffset;
            float outerYMax = baseRectYMax + outerOutlineOffset;

            // Clamp the radii for the inner and outer paths.
            // The radius cannot be larger than the available space (half of the respective rectangle's width/height).
            float actualTLInnerRadius = Mathf.Min(Mathf.Max(0, tlRadius - innerOutlineOffset), (innerXMax - innerXMin) / 2f, (innerYMax - innerYMin) / 2f);
            float actualTRInnerRadius = Mathf.Min(Mathf.Max(0, trRadius - innerOutlineOffset), (innerXMax - innerXMin) / 2f, (innerYMax - innerYMin) / 2f);
            float actualBRInnerRadius = Mathf.Min(Mathf.Max(0, brRadius - innerOutlineOffset), (innerXMax - innerXMin) / 2f, (innerYMax - innerYMin) / 2f);
            float actualBLInnerRadius = Mathf.Min(Mathf.Max(0, blRadius - innerOutlineOffset), (innerXMax - innerXMin) / 2f, (innerYMax - innerYMin) / 2f);

            float actualTLOuterRadius = Mathf.Min(Mathf.Max(0, tlRadius + outerOutlineOffset), (outerXMax - outerXMin) / 2f, (outerYMax - outerYMin) / 2f);
            float actualTROuterRadius = Mathf.Min(Mathf.Max(0, trRadius + outerOutlineOffset), (outerXMax - outerXMin) / 2f, (outerYMax - outerYMin) / 2f);
            float actualBROuterRadius = Mathf.Min(Mathf.Max(0, brRadius + outerOutlineOffset), (outerXMax - outerXMin) / 2f, (outerYMax - outerYMin) / 2f);
            float actualBLOuterRadius = Mathf.Min(Mathf.Max(0, blRadius + outerOutlineOffset), (outerXMax - outerXMin) / 2f, (outerYMax - outerYMin) / 2f);


            // Generate the lists of points for the inner and outer outlines.
            // The GetRoundedRectanglePath function now directly takes xMin, yMin, xMax, yMax.
            List<Vector2> innerPathPoints = GetRoundedRectanglePath(innerXMin, innerYMin, innerXMax, innerYMax,
                                                                   actualTLInnerRadius, actualTRInnerRadius, actualBRInnerRadius, actualBLInnerRadius,
                                                                   segments);

            List<Vector2> outerPathPoints = GetRoundedRectanglePath(outerXMin, outerYMin, outerXMax, outerYMax,
                                                                   actualTLOuterRadius, actualTROuterRadius, actualBROuterRadius, actualBLOuterRadius,
                                                                   segments);

            // Ensure the paths have a consistent number of points and at least two points to form quads.
            if (innerPathPoints.Count != outerPathPoints.Count || innerPathPoints.Count < 2) return;

            // Iterate through the path points to create the outline quads.
            // Each segment of the outline is a quad formed by two adjacent points from the inner path
            // and their corresponding points from the outer path.
            for (int i = 0; i < innerPathPoints.Count; i++)
            {
                int nextIdx = (i + 1) % innerPathPoints.Count; // Get the next index, wrapping around for the last point.

                Vector2 p1_inner = innerPathPoints[i];
                Vector2 p2_inner = innerPathPoints[nextIdx];
                Vector2 p1_outer = outerPathPoints[i];
                Vector2 p2_outer = outerPathPoints[nextIdx];

                // Add a quad for the current outline segment.
                // The order of vertices for the quad matters for rendering correctly (clockwise or counter-clockwise).
                vh.AddUIVertexQuad(new UIVertex[] {
                    UIVertexFromPos(p1_inner, outlineCol), // Bottom-left of quad segment
                    UIVertexFromPos(p2_inner, outlineCol), // Bottom-right of quad segment
                    UIVertexFromPos(p2_outer, outlineCol), // Top-right of quad segment
                    UIVertexFromPos(p1_outer, outlineCol)  // Top-left of quad segment
                });
            }
        }

        /// <summary>
        /// Generates a list of Vector2 points that define the path of a rounded rectangle's outer perimeter.
        /// Points are generated in clockwise order.
        /// </summary>
        /// <param name="xMin">The minimum X coordinate of the rectangle's bounds.</param>
        /// <param name="yMin">The minimum Y coordinate of the rectangle's bounds.</param>
        /// <param name="xMax">The maximum X coordinate of the rectangle's bounds.</param>
        /// <param name="yMax">The maximum Y coordinate of the rectangle's bounds.</param>
        /// <param name="tlRadius">Top-left corner radius.</param>
        /// <param name="trRadius">Top-right corner radius.</param>
        /// <param name="brRadius">Bottom-right corner radius.</param>
        /// <param name="blRadius">Bottom-left corner radius.</param>
        /// <param name="segments">Number of segments per corner arc.</param>
        /// <returns>A list of Vector2 points forming the rounded rectangle's path.</returns>
        private List<Vector2> GetRoundedRectanglePath(float xMin, float yMin, float xMax, float yMax,
                                                     float tlRadius, float trRadius, float brRadius, float blRadius,
                                                     int segments)
        {
            List<Vector2> pathPoints = new List<Vector2>();

            // Calculate arc centers based on the given bounds and radii.
            // These centers are in the absolute local space of the rectangle being described.
            Vector2 topLeftCenter = new Vector2(xMin + tlRadius, yMax - tlRadius);
            Vector2 topRightCenter = new Vector2(xMax - trRadius, yMax - trRadius);
            Vector2 bottomRightCenter = new Vector2(xMax - brRadius, yMin + brRadius);
            Vector2 bottomLeftCenter = new Vector2(xMin + blRadius, yMin + blRadius);

            // Helper to add points, preventing duplicates at segment boundaries (due to floating point precision).
            Action<Vector2> AddUniquePoint = (p) => {
                if (pathPoints.Count == 0 || Vector2.Distance(p, pathPoints[pathPoints.Count - 1]) > 0.0001f)
                {
                    pathPoints.Add(p);
                }
            };

            // Generate points starting from the top edge, moving clockwise.

            // Top-Left corner's right tangent point (start of top straight line)
            AddUniquePoint(new Vector2(xMin + tlRadius, yMax));

            // Top-Right arc and its tangent points
            if (trRadius > 0.001f) // Only add arc segments if radius is significant
            {
                AddArcPointsOnlyMiddle(pathPoints, topRightCenter, trRadius, 90f, 0f, segments);
            }
            // Top-Right corner's bottom tangent point (end of right straight line)
            AddUniquePoint(new Vector2(xMax, yMax - trRadius));

            // Bottom-Right arc and its tangent points
            if (brRadius > 0.001f)
            {
                AddArcPointsOnlyMiddle(pathPoints, bottomRightCenter, brRadius, 0f, -90f, segments);
            }
            // Bottom-Right corner's left tangent point (end of bottom straight line)
            AddUniquePoint(new Vector2(xMax - brRadius, yMin));

            // Bottom-Left arc and its tangent points
            if (blRadius > 0.001f)
            {
                AddArcPointsOnlyMiddle(pathPoints, bottomLeftCenter, blRadius, -90f, -180f, segments);
            }
            // Bottom-Left corner's top tangent point (end of left straight line)
            AddUniquePoint(new Vector2(xMin, yMin + blRadius));

            // Top-Left arc and its tangent points
            if (tlRadius > 0.001f)
            {
                AddArcPointsOnlyMiddle(pathPoints, topLeftCenter, tlRadius, -180f, -270f, segments);
            }
            // The path implicitly closes as the first point is the start of the next segment in the calling loop.

            return pathPoints;
        }

        /// <summary>
        /// Adds intermediate points of an arc to a list. Does not add start/end points as they are handled by straight segments.
        /// </summary>
        private void AddArcPointsOnlyMiddle(List<Vector2> pointsList, Vector2 centerPoint, float radius, float startAngle, float endAngle, int segments)
        {
            if (radius <= 0.001f || segments <= 0) return;

            float angleStep = (endAngle - startAngle) / segments;
            
            // Loop from 1 to segments - 1 to get only the intermediate points of the arc.
            // The start and end points of the arc are expected to be handled by the straight segments or other arc connections.
            for (int i = 1; i < segments; i++) 
            {
                float angle = startAngle + angleStep * i;
                Vector2 p = new Vector2(centerPoint.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad), centerPoint.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad));
                pointsList.Add(p);
            }
        }

        /// <summary>
        /// Adds triangle fans to the VertexHelper for a single arc segment of a filled shape.
        /// Each triangle has the arc's center as one vertex, and two adjacent points on the arc as the other two vertices.
        /// </summary>
        private void AddArc(VertexHelper vh, Vector2 centerPoint, float radius, float startAngle, float endAngle, int segments, Color arcColor)
        {
            if (radius <= 0.001f || segments <= 0) return; 

            float angleStep = (endAngle - startAngle) / segments;

            // Add the center point first for all triangles to pivot around.
            int centerIdx = vh.currentVertCount;
            vh.AddVert(UIVertexFromPos(centerPoint, arcColor));

            // Generate vertices for each segment of the arc.
            for (int i = 0; i <= segments; i++)
            {
                float angle = startAngle + angleStep * i;
                Vector2 p = new Vector2(centerPoint.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad), centerPoint.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad));
                vh.AddVert(UIVertexFromPos(p, arcColor));
            }

            // Create triangles from the center point to each arc segment.
            for (int i = 0; i < segments; i++)
            {
                // The indices are relative to the vertices just added for this arc.
                // centerIdx is the first vertex (the center point).
                // (centerIdx + 1 + i) and (centerIdx + 1 + i + 1) are the two points on the arc for the current segment.
                vh.AddTriangle(centerIdx, centerIdx + 1 + i, centerIdx + 1 + i + 1); 
            }
        }

        /// <summary>
        /// Helper function to create a UIVertex from a position and color.
        /// </summary>
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
            // Ensure radii and thickness are non-negative.
            universalCornerRadius = Mathf.Max(0, universalCornerRadius);
            topLeftRadius = Mathf.Max(0, topLeftRadius);
            topRightRadius = Mathf.Max(0, topRightRadius);
            bottomRightRadius = Mathf.Max(0, bottomRightRadius);
            bottomLeftRadius = Mathf.Max(0, bottomLeftRadius);
            outlineThickness = Mathf.Max(0, outlineThickness);
            
            // Mark vertices as dirty to trigger a mesh rebuild in the editor.
            SetVerticesDirty();
        }
        #endif

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            // Mark vertices as dirty when the RectTransform's size changes.
            SetVerticesDirty();
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Custom editor for the ProceduralRoundedRectangle component to provide a more organized Inspector.
    /// </summary>
    [CustomEditor(typeof(ProceduralRoundedRectangle))]
    public class ProceduralRoundedRectangleEditor : Editor 
    {
        // SerializedProperty references for all public fields to be displayed in the Inspector.
        private SerializedProperty m_Color;
        private SerializedProperty m_Material; 
        private SerializedProperty m_RaycastTarget; 
        private SerializedProperty m_Maskable;      

        private SerializedProperty m_Filled;
        private SerializedProperty m_CornerMode;
        private SerializedProperty m_UniversalCornerRadius;
        private SerializedProperty m_TopLeftRadius;
        private SerializedProperty m_TopRightRadius;
        private SerializedProperty m_BottomRightRadius;
        private SerializedProperty m_BottomLeftRadius;
        private SerializedProperty m_SegmentsPerCorner;
        private SerializedProperty m_DrawOutline;
        private SerializedProperty m_OutlineThickness;
        private SerializedProperty m_OutlineColor;
        private SerializedProperty m_OutlinePosition;

        /// <summary>
        /// Called when the editor becomes active or the target object changes.
        /// Used to find and cache the SerializedProperty instances.
        /// </summary>
        protected virtual void OnEnable() 
        {
            m_Color = serializedObject.FindProperty("m_Color"); 
            m_Material = serializedObject.FindProperty("m_Material"); 
            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget"); 
            m_Maskable = serializedObject.FindProperty("m_Maskable");             
            
            m_Filled = serializedObject.FindProperty("filled");
            m_CornerMode = serializedObject.FindProperty("cornerMode");
            m_UniversalCornerRadius = serializedObject.FindProperty("universalCornerRadius");
            m_TopLeftRadius = serializedObject.FindProperty("topLeftRadius");
            m_TopRightRadius = serializedObject.FindProperty("topRightRadius");
            m_BottomRightRadius = serializedObject.FindProperty("bottomRightRadius");
            m_BottomLeftRadius = serializedObject.FindProperty("bottomLeftRadius");
            m_SegmentsPerCorner = serializedObject.FindProperty("segmentsPerCorner");
            m_DrawOutline = serializedObject.FindProperty("drawOutline");
            m_OutlineThickness = serializedObject.FindProperty("outlineThickness");
            m_OutlineColor = serializedObject.FindProperty("outlineColor");
            m_OutlinePosition = serializedObject.FindProperty("outlinePosition");
        }

        /// <summary>
        /// Renders the custom Inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update(); // Always call this at the start of OnInspectorGUI.

            // Display script field (read-only).
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
            GUI.enabled = true;

            EditorGUILayout.Space();

            // Display base Graphic settings.
            EditorGUILayout.LabelField("Graphic Base Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Color, new GUIContent("Color")); 
            EditorGUILayout.PropertyField(m_Material, new GUIContent("Material")); 
            EditorGUILayout.PropertyField(m_RaycastTarget, new GUIContent("Raycast Target"));
            EditorGUILayout.PropertyField(m_Maskable, new GUIContent("Maskable"));

            EditorGUILayout.Space();

            // Display custom ProceduralRoundedRectangle settings.
            EditorGUILayout.LabelField("Procedural Rounded Rectangle Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Filled, new GUIContent("Filled"));
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Corner Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_CornerMode, new GUIContent("Corner Mode"));

            ProceduralRoundedRectangle.CornerRadiusMode currentCornerMode = (ProceduralRoundedRectangle.CornerRadiusMode)m_CornerMode.enumValueIndex;

            // Show appropriate radius fields based on the selected corner mode.
            if (currentCornerMode == ProceduralRoundedRectangle.CornerRadiusMode.Universal)
            {
                EditorGUILayout.PropertyField(m_UniversalCornerRadius, new GUIContent("Universal Radius"));
                // Clamp the universal radius to be non-negative.
                if (m_UniversalCornerRadius.floatValue < 0) m_UniversalCornerRadius.floatValue = 0;
            }
            else 
            {
                EditorGUILayout.LabelField("Individual Radii", EditorStyles.miniLabel);
                EditorGUILayout.PropertyField(m_TopLeftRadius, new GUIContent("Top Left Radius"));
                EditorGUILayout.PropertyField(m_TopRightRadius, new GUIContent("Top Right Radius"));
                EditorGUILayout.PropertyField(m_BottomRightRadius, new GUIContent("Bottom Right Radius"));
                EditorGUILayout.PropertyField(m_BottomLeftRadius, new GUIContent("Bottom Left Radius"));
                
                // Clamp individual radii to be non-negative.
                if (m_TopLeftRadius.floatValue < 0) m_TopLeftRadius.floatValue = 0;
                if (m_TopRightRadius.floatValue < 0) m_TopRightRadius.floatValue = 0;
                if (m_BottomRightRadius.floatValue < 0) m_BottomRightRadius.floatValue = 0;
                if (m_BottomLeftRadius.floatValue < 0) m_BottomLeftRadius.floatValue = 0;
            }

            EditorGUILayout.PropertyField(m_SegmentsPerCorner, new GUIContent("Segments Per Corner"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Outline Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_DrawOutline, new GUIContent("Draw Outline"));

            // Only show outline specific settings if drawOutline is true.
            if (m_DrawOutline.boolValue)
            {
                EditorGUILayout.PropertyField(m_OutlineThickness, new GUIContent("Outline Thickness"));
                // Clamp outline thickness to be non-negative.
                if (m_OutlineThickness.floatValue < 0) m_OutlineThickness.floatValue = 0;

                EditorGUILayout.PropertyField(m_OutlineColor, new GUIContent("Outline Color"));
                EditorGUILayout.PropertyField(m_OutlinePosition, new GUIContent("Outline Position"));
            }

            // Apply changes back to the serialized object and trigger a mesh rebuild if properties were modified.
            if (serializedObject.ApplyModifiedProperties())
            {
                ((ProceduralRoundedRectangle)target).SetVerticesDirty(); 
            }
        }
    }
    #endif
}
