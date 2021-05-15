using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VerticesDrawer : MonoBehaviour
{
    MeshFilter meshFilter;

    List<Vector3> vertices;

    Vector3 center;
    // Start is called before the first frame update
    void Start() {}

    private int CompareAngles(Vector3 p1, Vector3 p2) {
        int res = 0;

        float p1Angle = Mathf.Atan2(p1.z - center.z, p1.x - center.x);
        float p2Angle = Mathf.Atan2(p1.z - center.z, p1.x - center.x);

        if(p1Angle - p2Angle <= Mathf.Epsilon) {
            float p1Distance = Vector3.Distance(p1, center);
            float p2Distance = Vector3.Distance(p2, center);

            res = p1Distance.CompareTo(p2Distance);
        } else {
            res = p1Angle.CompareTo(p2Angle);
        }

        return res;
    }

    private void OnDrawGizmosSelected() {
        vertices = new List<Vector3>(GetComponent<MeshFilter>().sharedMesh.vertices);
        center = vertices[0];

        vertices.Sort(CompareAngles);

        vertices.Add(vertices[0]);

        Gizmos.color = Color.red;

        Vector3 centroid = Vector3.zero;

        float twiceArea = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 vertex = vertices[i];
            Vector3 worldVertex = transform.TransformPoint(vertex);

            Gizmos.DrawWireSphere(worldVertex, 0.3f);
            Handles.Label(worldVertex, i.ToString());

            if (i + 1 < vertices.Count) {
                Vector3 nextWorldVertex = transform.TransformPoint(vertices[i+1]);
                float secondComponent = worldVertex.x * nextWorldVertex.z - nextWorldVertex.x * worldVertex.z;
                
                twiceArea += secondComponent;

                centroid.x += (worldVertex.x + nextWorldVertex.x) * secondComponent;
                centroid.z += (worldVertex.y + nextWorldVertex.y) * secondComponent;
            }
        }

        centroid /= 3 * twiceArea;

        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(centroid, 0.3f);
    }
}
