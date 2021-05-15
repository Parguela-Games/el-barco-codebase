using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class MapAnalyzer
{
    public static Vector3[] GetPath(GameObject[] areasToExplore) {
        Vector3[] points = new Vector3[areasToExplore.Length];
        int i = 0;

        foreach(GameObject obj in areasToExplore) {
            Transform objTransform = obj.transform;
            Bounds bounds = obj.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
            Vector3 meshCenter = bounds.center;
            Vector3 center = obj.transform.TransformPoint(meshCenter);
            NavMeshHit hit;
            NavMesh.SamplePosition(center, out hit, 1f, NavMesh.AllAreas);

            points[i] = hit.position;

            i++;
        }

        return points;
    }

    public static bool CheckArea(Vector3 position, int areaMask) {
        bool res = false;
        NavMeshHit hit;

        NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas);

        if(hit.hit) {
            int hitArea = Mathf.RoundToInt(Mathf.Log10(hit.mask) / Mathf.Log10(2f));

            res = areaMask == hitArea;
        }

        return res;
    }
}
