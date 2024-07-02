using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ProceduralGenDeleteMe : MonoBehaviour
{/*
    [SerializeField] private int xSize;
    [SerializeField] private int zSize;

    Mesh _generatedMesh;

    private Vector3[] _vertices;
    private int[] _triangles;
    private MeshCollider _collider;
    private void Awake()
    {
        _collider = GetComponent<MeshCollider>();
        GenerateMap();
    }

    private void GenerateMap()
    { 
        GetComponent<MeshFilter>().mesh = _generatedMesh = new Mesh();
        _generatedMesh.name = "Procedural Grid";

        _vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        Vector2[] _uv = new Vector2[_vertices.Length];

        for (int z = 0, i = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                float perlinNoise = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) *10f;
                _vertices[i] = new Vector3(x, perlinNoise, z);
                _uv[i] = new Vector2((float)x / xSize, (float)z / zSize);
            }
        }
        _generatedMesh.vertices = _vertices;
        _generatedMesh.uv = _uv;

        _triangles = new int[xSize * zSize * 6];
        for (int tr = 0, vr = 0, z = 0; z < zSize; z++, vr++)
        {
            for (int x = 0; x < xSize; x++, tr+=6, vr++)
            {
                _triangles[tr] = vr;
                _triangles[tr + 3] = _triangles[tr + 2] = vr + 1;
                _triangles[tr + 4] = _triangles[tr + 1] = vr + 1 + xSize;
                _triangles[tr + 5] = vr + 2 + xSize;
            }
        }
        _generatedMesh.triangles = _triangles;
        _generatedMesh.RecalculateNormals();
        _collider.sharedMesh = _generatedMesh;
    }*/
    #region Variables
    Mesh mesh;
    MeshCollider meshCollider;
    Vector3[] vertices;
    //Vector2[] Uvs;
    Color[] colors;
    int[] triangles;

    [Range(1, 9999)]
    public int xSize = 100;
    [Range(1, 9999)]
    public int zSize = 100;

    public Gradient gradient;

    public float MinHeight = 0;
    public float MaxHeight = 0;

    public bool Reset_Min_Max;
    #endregion
    #region Octaves
    [Range(1, 6)]
    public int Octaves = 6;
    public int Scale = 50;

    public float offsetX = 0f;
    public float offsetY = 0f;

    public float Frequency_01 = 5f;
    public float FreqAmp_01 = 3f;

    public float Frequency_02 = 6f;
    public float FreqAmp_02 = 2.5f;

    public float Frequency_03 = 3f;
    public float FreqAmp_03 = 1.5f;

    public float Frequency_04 = 2.5f;
    public float FreqAmp_04 = 1f;

    public float Frequency_05 = 2f;
    public float FreqAmp_05 = .7f;

    public float Frequency_06 = 1f;
    public float FreqAmp_06 = .5f;
    #endregion
    #region Start
    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);
    }
    #endregion
    void ResetMinMax()
    {
        MinHeight = 0f;
        MaxHeight = 0f;
        Reset_Min_Max = false;
    }
    #region Update
    private void Update()
    {
        if (Reset_Min_Max)
            ResetMinMax();

        CreateShape();
        UpdateMesh();
    }
    #endregion
    #region CreateShape
    void CreateShape()
    {
        #region Vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Calculate(x, z);
                vertices[i] = new Vector3(x, y, z);

                if (y > MaxHeight)
                    MaxHeight = y;
                if (y < MinHeight)
                    MinHeight = y;

                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        #endregion
        #region Triangles
        triangles = new int[xSize * zSize * 6];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
        #endregion
        #region Gradient Color
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float Height = Mathf.InverseLerp(MinHeight, MaxHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(Height);
                i++;
            }
        }
        #endregion
        #region UVs
        /*
         Uvs = new Vector2[vertices.Length];
         for (int i = 0, z = 0; z <= zSize; z++)
         {
             for (int x = 0; x <= xSize; x++)
             {
                 Uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                 i++;
             }
         }
         */
        #endregion
    }
    #endregion
    #region Octaves Calculation
    public float Calculate(float x, float z)
    {
        float[] octaveFrequencies = new float[] { Frequency_01, Frequency_02, Frequency_03, Frequency_04, Frequency_05, Frequency_06 };
        float[] octaveAmplitudes = new float[] { FreqAmp_01, FreqAmp_02, FreqAmp_03, FreqAmp_04, FreqAmp_05, FreqAmp_06 };
        float y = 0;

        for (int i = 0; i < Octaves; i++)
        {
            y += octaveAmplitudes[i] * Mathf.PerlinNoise(
                     octaveFrequencies[i] * x + offsetX * Scale,
                     octaveFrequencies[i] * z + offsetY * Scale);

        }

        return y;
    }
    #endregion
    #region UpdateMesh
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        meshCollider.sharedMesh = mesh;
        //mesh.uv = Uvs;

        mesh.RecalculateNormals();

    }
    #endregion
    #region  Gizmos
    /* 
     private void OnDrawGizmos()
    {
        if (vertices == null)
            return;

        for (int i = 0; i < vertices.Length; i++){
            Gizmos.DrawSphere(vertices[i], .1f);
        }           
    }
    */
    #endregion
}