using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LODGenerator : MonoBehaviour{
    /*
    This class is responsible for generating the chunks of the planet.
    Using the same TerrainFace class, we can use it to create a more detailed square of the planet.
    For now, we will only generate 1 "chunk", the first square that comes up in the first face in the faces array, but in the future, we will generate multiple chunks.



    */

    public ShapeSettings shapeSettings;
    public ShapeGenerator shapeGenerator;
    public GameObject obj;
    Mesh mesh;
    [Range(2, 256)]
    public int resolution = 2;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public LODGenerator(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp) {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    void Start() {
        shapeGenerator = new ShapeGenerator();
        shapeGenerator.UpdateSettings(shapeSettings);
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
        
        mesh = new();
        mesh.Clear();
        mesh.vertices = new Vector3[6];
        for (int i = 0; i < 6; i++) {
            Vector3[] verts = this.obj.GetComponent<MeshFilter>().sharedMesh.vertices;
            mesh.vertices[i] = verts[i];
        }
        int[] tris = new int[6];
        tris[0] = this.obj.GetComponent<MeshFilter>().sharedMesh.triangles[0];
        tris[1] = this.obj.GetComponent<MeshFilter>().sharedMesh.triangles[1];
        tris[2] = this.obj.GetComponent<MeshFilter>().sharedMesh.triangles[2];
        tris[3] = this.obj.GetComponent<MeshFilter>().sharedMesh.triangles[3];
        tris[4] = this.obj.GetComponent<MeshFilter>().sharedMesh.triangles[4];
        tris[5] = this.obj.GetComponent<MeshFilter>().sharedMesh.triangles[5];
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        // SubdivideMesh();
    }

    public void SubdivideMesh() {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}

