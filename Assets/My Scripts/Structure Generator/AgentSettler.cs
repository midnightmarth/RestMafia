using UnityEngine;
using System.Collections.Generic;

public class PointPicker {
    public float minDistance;
    public float flatnessThreshold;
    public int numberOfPoints;

    public PointPicker(float minDistance, float flatnessThreshold, int numberOfPoints) {
        this.minDistance = minDistance;
        this.flatnessThreshold = flatnessThreshold;
        this.numberOfPoints = numberOfPoints;
    }

    public List<Vector3> PickPoints(Planet planet) {
        List<Vector3> points = new List<Vector3>();
        List<Vector3> candidatePoints = new List<Vector3>();

        foreach (TerrainFace face in planet.terrainFaces) {
            Mesh mesh = face.mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;

            for (int i = 0; i < vertices.Length; i++) {
                Vector3 pointOnSurface = vertices[i].normalized; // Ensure point is on the surface
                if (IsFlat(normals[i])) {
                    candidatePoints.Add(pointOnSurface);
                }
            }
        }

        while (points.Count < numberOfPoints && candidatePoints.Count > 0) {
            int index = Random.Range(0, candidatePoints.Count);
            Vector3 candidatePoint = candidatePoints[index];
            candidatePoints.RemoveAt(index);

            if (IsFarEnough(points, candidatePoint)) {
                points.Add(candidatePoint);
            }
        }

        return points;
    }

    private bool IsFlat(Vector3 normal) {
        return Mathf.Abs(Vector3.Dot(normal, Vector3.up)) > flatnessThreshold;
    }

    private bool IsFarEnough(List<Vector3> points, Vector3 newPoint) {
        foreach (Vector3 point in points) {
            if (GreatCircleDistance(point, newPoint) < minDistance) {
                return false;
            }
        }
        return true;
    }

    private float GreatCircleDistance(Vector3 point1, Vector3 point2) {
        float angle = Vector3.Angle(point1, point2);
        return Mathf.Deg2Rad * angle;
    }
}