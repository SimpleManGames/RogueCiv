using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    public bool useCollider, useColors, useUVCoordinates;

    private Mesh hexMesh;
    private MeshCollider _meshCollider;
    private MeshCollider meshCollider
    {
        get
        {
            if (_meshCollider == null)
                _meshCollider = GetComponent<MeshCollider>();

            if (_meshCollider == null && useCollider)
                _meshCollider = gameObject.AddComponent<MeshCollider>();

            return _meshCollider;
        }
    }

    [NonSerialized]
    List<Vector3> vertices = new List<Vector3>();
    [NonSerialized]
    List<int> triangles = new List<int>();
    [NonSerialized]
    List<Color> colors = new List<Color>();
    [NonSerialized]
    List<Vector2> uvs = new List<Vector2>();

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
    }

    public void Clear()
    {
        hexMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        if (useColors)
            colors = ListPool<Color>.Get();
        triangles = ListPool<int>.Get();
        if (useUVCoordinates)
            uvs = ListPool<Vector2>.Get();
    }

    public void Apply()
    {
        hexMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);

        if (useColors)
        {
            hexMesh.SetColors(colors);
            ListPool<Color>.Add(colors);
        }

        if (useUVCoordinates)
        {
            hexMesh.SetUVs(0, uvs);
            ListPool<Vector2>.Add(uvs);
        }

        hexMesh.SetTriangles(triangles, 0);
        ListPool<int>.Add(triangles);

        hexMesh.RecalculateNormals();

        if (useCollider)
            meshCollider.sharedMesh = hexMesh;
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(HexMetrics.Perturb(v1));
        vertices.Add(HexMetrics.Perturb(v2));
        vertices.Add(HexMetrics.Perturb(v3));
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    public void AddTriangleColor(Color c)
    {
        colors.AddRange(new Color[3] { c, c, c });
    }
    public void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.AddRange(new Color[3] { c1, c2, c3 });
    }
    public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.AddRange(new Vector3[4] { HexMetrics.Perturb(v1), HexMetrics.Perturb(v2), HexMetrics.Perturb(v3), HexMetrics.Perturb(v4) });
        triangles.AddRange(new int[6] { vertexIndex, vertexIndex + 2, vertexIndex + 1, vertexIndex + 1, vertexIndex + 2, vertexIndex + 3 });
    }
    public void AddQuadColor(Color c)
    {
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
    }
    public void AddQuadColor(Color c1, Color c2)
    {
        colors.AddRange(new Color[4] { c1, c1, c2, c2 });
    }
    public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.AddRange(new Color[4] { c1, c2, c3, c4 });
    }

    public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
    }
    public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
    {
        uvs.Add(uv1);
        uvs.Add(uv2);
        uvs.Add(uv3);
        uvs.Add(uv4);
    }
    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMin, vMax));
        uvs.Add(new Vector2(uMax, vMax));
    }
    public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
}