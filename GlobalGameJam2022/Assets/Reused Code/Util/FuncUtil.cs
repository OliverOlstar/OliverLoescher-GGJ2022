using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OliverLoescher 
{
    public class FuncUtil : MonoBehaviour
    {
        public static Vector3 Horizontalize(Vector3 pVector, bool pNormalize = false)
        {
            pVector.y = 0;
            if (pNormalize)
                pVector.Normalize();
            return pVector;
        }

        public static float SmoothStep(float pMin, float pMax, float pIn)
        {
            return Mathf.Clamp01((pIn - pMin) / (pMax - pMin));
        }
        public static float SmoothStep(Vector2 pMinMax, float pIn)
        {
            return SmoothStep(pMinMax.x, pMinMax.y, pIn);
        }
        
        public static float SafeAngle(float pAngle)
        {
            if (pAngle > 180)
            {
                pAngle -= 360;
            }
            return pAngle;
        }
    }
}