using UnityEditor;
using UnityEngine;

namespace EditorExpansion
{
    [InitializeOnLoad]
    internal static class ZebraStripeRenderer
    {
        private const int RowHeight = 16;
        private const int OffsetY = -4;
        private static readonly Color StripeColor = new Color(0, 0, 0, 0.1f);
    
        static ZebraStripeRenderer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DrawZebraStripesInHierarchy;
            EditorApplication.projectWindowItemOnGUI += DrawZebraStripesInProject;
        }
    
        private static void DrawZebraStripesInHierarchy(int instanceID, Rect selectionRect)
        {
            int rowIndex = CalculateRowIndex(selectionRect.y);
            if (IsOddRow(rowIndex))
            {
                DrawStripe(selectionRect);
            }
        }
    
        private static void DrawZebraStripesInProject(string guid, Rect selectionRect)
        {
            int rowIndex = CalculateRowIndex(selectionRect.y);
            if (IsOddRow(rowIndex))
            {
                DrawStripe(selectionRect);
            }
        }
    
        private static int CalculateRowIndex(float yPosition)
        {
            return (int)(yPosition + OffsetY) / RowHeight;
        }
    
        private static bool IsOddRow(int rowIndex)
        {
            return rowIndex % 2 != 0;
        }
    
        private static void DrawStripe(Rect rect)
        {
            float originalXMax = rect.xMax;
            rect.x = 32;
            rect.xMax = originalXMax + 16;
            EditorGUI.DrawRect(rect, StripeColor);
        }
    }
}