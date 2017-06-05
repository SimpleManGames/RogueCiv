using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    private Mesh hexMesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    public void Triangulate(HexObject[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < cells.Length; i++)
            Triangulate(cells[i]);

        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.RecalculateNormals();
    }

    private void Triangulate(HexObject cell)
    {
        for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            Triangulate(d, cell);
    }

    void Triangulate(HexDirection direction, HexObject cell)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMatrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMatrics.GetSecondSolidCorner(direction);

        // Fill the center
        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.Color);
        if (direction <= HexDirection.SE)
            TriangulateConnection(direction, cell, v1, v2);
    }

    void TriangulateConnection(HexDirection direction, HexObject cell, Vector3 v1, Vector3 v2)
    {
        HexObject neighbour = HexGrid.FindHexObject(Hex.Neighbour(cell.Hex, (byte)direction).cubeCoords);

        if (neighbour == null)
            return;

        Vector3 bridge = HexMatrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbour.Elevation * HexMatrics.elevationStep;

        // Add the bridge between each center of the cells
        AddQuad(v1, v2, v3, v4);
        AddQuadColor(cell.Color, neighbour.Color);

        HexObject next = HexGrid.FindHexObject(Hex.Neighbour(cell.Hex, (byte)direction.Next()).cubeCoords);

        if (direction <= HexDirection.E && next != null)
        {
            Vector3 v5 = v2 + HexMatrics.GetBridge(direction.Next());
            v5.y = next.Elevation * HexMatrics.elevationStep;
            AddTriangle(v2, v4, v5);
            AddTriangleColor(cell.Color, neighbour.Color, next.Color);
        }
    }

    private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void AddTriangleColor(Color c)
    {
        colors.AddRange(new Color[3] { c, c, c });
    }

    private void AddTriangleColor(Color c1, Color c2, Color c3)
    {
        colors.AddRange(new Color[3] { c1, c2, c3 });
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.AddRange(new Vector3[4] { v1, v2, v3, v4 });
        triangles.AddRange(new int[6] { vertexIndex, vertexIndex + 2, vertexIndex + 1, vertexIndex + 1, vertexIndex + 2, vertexIndex + 3 });
    }

    void AddQuadColor(Color c1, Color c2)
    {
        colors.AddRange(new Color[4] { c1, c1, c2, c2 });
    }

    void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.AddRange(new Color[4] { c1, c2, c3, c4 });
    }
}