using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
public class Planet : MonoBehaviour {

    [Range(2,256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    ShapeGenerator shapeGenerator = new();
    ColorGenerator colorGenerator = new();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    public TerrainFace[] terrainFaces;

    public float minCityDistance = 1f;
    public float flatnessThreshold = 0.1f;
    public int numberOfCities = 10;     
    public GameObject cityPrefab;

	void Initialize() {

        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);
        if (meshFilters == null || meshFilters.Length == 0) {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++) {
            if (meshFilters[i] == null) {
                GameObject meshObj = new GameObject("Mesh " + i);
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;


            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void GeneratePlanet() {
        Initialize();
        GenerateMesh();
        GenerateColours();
        // GameObject tempHolder = GameObject.Find("TempHolder");
        // if(tempHolder != null && tempHolder.GetComponentsInChildren<Transform>().Length > 0) {
        //     foreach (Transform obj in tempHolder.GetComponentsInChildren<Transform>()) {
        //         Destroy(obj.gameObject);
        //     }
        // }
        // PickPoints();
    }

    public void GeneratePlanetFromSeed() {
        shapeSettings.noiseSeed = Random.Range(0, int.MaxValue);
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated() {
        if (autoUpdate) {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh() {
        foreach (TerrainFace face in terrainFaces) {
            face.ConstructMesh();
        }
        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours() {
         colorGenerator.UpdateColors();
    }

    void PickPoints() {
        PointPicker pointPicker = new PointPicker(1.0f, 0.9f, 10); // Adjust parameters as needed
        List<Vector3> points = pointPicker.PickPoints(this);
        Instantiate(new GameObject("TempHolder"));
        foreach (Vector3 point in points)
        {
            Debug.Log("Picked point: " + point);
            GameObject cube = Instantiate(cityPrefab, point, Quaternion.identity);
            cube.transform.parent = GameObject.Find("TempHolder").transform;
        }
    }

}


