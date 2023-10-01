using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class Bezier
    {
        public static List<Vector3> GetCurve(Vector3 startPos, Vector3 endPos, float hight, float percentHeight = 1, int vertexCount = 16)
        {
            var pointList = new List<Vector3>();
            GetCurve(ref pointList, startPos, endPos, hight, percentHeight, vertexCount);
            return pointList;
        }

        public static void GetCurve(ref List<Vector3> pointList, Vector3 startPos, Vector3 endPos, float hight,
            float percentHeight = 1, int vertexCount = 16)
        {
            pointList.Clear();
            var resultHeight = Mathf.Max(hight / 2, percentHeight * hight);
            var center = new Vector3(
                (startPos.x + endPos.x) / 2, 
                resultHeight,
                (startPos.z + endPos.z) / 2);
      
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                var tangentLineVertex1 = Vector3.Lerp(startPos, center, ratio);
                var tangentLineVertex2 = Vector3.Lerp(center, endPos, ratio);
                var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierpoint);
            }
            pointList.Add(endPos);
        }
    }
}