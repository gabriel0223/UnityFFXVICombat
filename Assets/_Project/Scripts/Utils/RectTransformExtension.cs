using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectTransformExtension
{
    /// <summary>
    /// Set pivot without changing the position of the element
    /// </summary>
    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
        deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
        deltaPosition.Scale(rectTransform.localScale);          // apply scaling
        deltaPosition = rectTransform.localRotation * deltaPosition; // apply rotation
    
        rectTransform.pivot = pivot;                            // change the pivot
        rectTransform.localPosition -= deltaPosition;           // reverse the position change
    }
}
