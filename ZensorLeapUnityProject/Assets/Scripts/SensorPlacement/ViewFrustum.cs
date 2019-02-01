using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewFrustum : MonoBehaviour {

    Vector3[] points = new Vector3[8];
    Vector3 mountPoint;
    Quaternion orientation;
    public Material radarMaterial;

	// Use this for initialization
	void Start () {
        
        points = MountRadarPoints(GeneratePoints(-15, 15, -10, 10, 2, 70), orientation, mountPoint);

        GameObject Radar = new GameObject("Radar");

        Radar.AddComponent<MeshRenderer>();
        Radar.AddComponent<MeshFilter>();
        Radar.GetComponent<MeshFilter>().mesh = CreateCube(points);
        Radar.GetComponent<MeshRenderer>().material = radarMaterial;

	}

    private Vector3[] MountRadarPoints(Vector3[] generatedPoints, Quaternion orien, Vector3 mount)
    {
        Vector3[] newPoints = new Vector3[8];

        for (int i = 0; i < newPoints.Length; i++){
            newPoints[i] = (Quaternion.Inverse(orien) * generatedPoints[i]) + mount; 
        }

        return newPoints;
    }

    private Vector3[] GeneratePoints(float minAz, float maxAz, float minEl, float maxEl, float minRange, float maxRange)
    {
        Vector3[] newPoints = new Vector3[8];

        Vector3[] rawPoints = new[] { 
            new Vector3(minAz, maxEl, minRange), 
            new Vector3(maxAz, maxEl, minRange), 
            new Vector3(maxAz, minEl, minRange), 
            new Vector3(minAz, minEl, minRange), 
            new Vector3(minAz, maxEl, maxRange), 
            new Vector3(maxAz, maxEl, maxRange), 
            new Vector3(maxAz, minEl, maxRange), 
            new Vector3(minAz, minEl, maxRange) 
        };

        for (int i = 0; i < rawPoints.Length; i++){
            var az = rawPoints[i].x;
            var el = rawPoints[i].y;
            var range = rawPoints[i].z;

            Quaternion q = Quaternion.Euler(el, 0, 0) * Quaternion.Euler(0, az, 0);
            var rotatedVector = q * new Vector3(0, 0, 1);
            newPoints[i] = range * rotatedVector;
        }

        return newPoints;
    }

    private Mesh CreateCube(Vector3[] vertices)
    {
        int[] triangles = {
            0, 2, 1, //face front
            0, 3, 2,
            2, 3, 4, //face top
            2, 4, 5,
            1, 2, 5, //face right
            1, 5, 6,
            0, 7, 4, //face left
            0, 4, 3,
            5, 4, 7, //face back
            5, 7, 6,
            0, 6, 7, //face bottom
            0, 1, 6
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
