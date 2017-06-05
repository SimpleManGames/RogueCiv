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
    private void Triangulate(HexDirection direction, HexObject cell)
    {
        Vector3 center = cell.transform.localPosition;
        Vector3 v1 = center + HexMetrics.GetFirstSolidCorner(direction);
        Vector3 v2 = center + HexMetrics.GetSecondSolidCorner(direction);

        // Fill the center
        AddTriangle(center, v1, v2);
        AddTriangleColor(cell.Color);
        if (direction <= HexDirection.SE)
            TriangulateConnection(direction, cell, v1, v2);
    }
    private void TriangulateConnection(HexDirection direction, HexObject cell, Vector3 v1, Vector3 v2)
    {
        HexObject neighbour = HexGrid.FindHexObject(Hex.Neighbour(cell.Hex, (byte)direction).cubeCoords);

        if (neighbour == null)
            return;

        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;
        v3.y = v4.y = neighbour.Elevation * HexMetrics.elevationStep;

        if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
            TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbour);
        else
        {
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(cell.Color, neighbour.Color);
        }

        HexObject next = HexGrid.FindHexObject(Hex.Neighbour(cell.Hex, (byte)direction.Next()).cubeCoords);
        if (direction <= HexDirection.E && next != null)
        {
            Vector3 v5 = v2 + HexMetrics.GetBridge(direction.Next());
            v5.y = next.Elevation * HexMetrics.elevationStep;

            if (cell.Elevation <= neighbour.Elevation)
            {
                if (cell.Elevation <= next.Elevation)
                    TriangulateCorner(v2, cell, v4, neighbour, v5, next);
                else
                    TriangulateCorner(v5, next, v2, cell, v4, neighbour);
            }
            else if (neighbour.Elevation <= next.Elevation)
                TriangulateCorner(v4, neighbour, v5, next, v2, cell);
            else
                TriangulateCorner(v5, next, v2, cell, v4, neighbour);
        }
    }
    private void TriangulateEdgeTerraces(Vector3 beginLeft, Vector3 beginRight, HexObject beginCell, Vector3 endLeft, Vector3 endRight, HexObject endCell)
    {
        Vector3 v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

        AddQuad(beginLeft, beginRight, v3, v4);
        AddQuadColor(beginCell.Color, c2);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c2;
            v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
            v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
            c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2);
        }

        AddQuad(v3, v4, endLeft, endRight);
        AddQuadColor(c2, endCell.Color);
    }
    private void TriangulateCorner(Vector3 bottom, HexObject bottomCell, Vector3 left, HexObject leftCell, Vector3 right, HexObject rightCell)
    {
        HexEdgeType leftEdgeType = bottomCell.GetEdgeType(leftCell);
        HexEdgeType rightEdgeType = bottomCell.GetEdgeType(rightCell);

        if (leftEdgeType == HexEdgeType.Slope)
        {
            if (rightEdgeType == HexEdgeType.Slope)
            {
                TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                return;
            }
            if (rightEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                return;
            }

            TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
            return;
        }

        if (rightEdgeType == HexEdgeType.Slope)
        {
            if (leftEdgeType == HexEdgeType.Flat)
            {
                TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                return;
            }
        }

        AddTriangle(bottom, left, right);
        AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
    }
    private void TriangulateCornerTerraces(Vector3 begin, HexObject beginCell, Vector3 left, HexObject leftCell, Vector3 right, HexObject rightCell)

    {
        Vector3 v3 = HexMetrics.TerraceLerp(begin, left, 1);
        Vector3 v4 = HexMetrics.TerraceLerp(begin, right, 1);
        Color c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
        Color c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

        AddTriangle(begin, v3, v4);
        AddTriangleColor(beginCell.Color, c3, c4);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v3;
            Vector3 v2 = v4;
            Color c1 = c3;
            Color c2 = c4;
            v3 = HexMetrics.TerraceLerp(begin, left, i);
            v4 = HexMetrics.TerraceLerp(begin, right, i);
            c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
            c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);
            AddQuad(v1, v2, v3, v4);
            AddQuadColor(c1, c2, c3, c4);
        }

        AddQuad(v3, v4, left, right);
        AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
    }
    private void TriangulateCornerTerracesCliff(Vector3 begin, HexObject beginCell, Vector3 left, HexObject leftCell, Vector3 right, HexObject rightCell)
    {
        float b = 1f / (rightCell.Elevation - beginCell.Elevation);
        Vector3 boundary = Vector3.Lerp(begin, right, b);
        Color boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);

        TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

        if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
        {
            TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
        }
        else
        {
            AddTriangle(left, right, boundary);
            AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
        }
    }
    private void TriangulateBoundaryTriangle(Vector3 begin, HexObject beginCell, Vector3 left, HexObject leftCell, Vector3 boundary, Color boundaryColor)
    {
        Vector3 v2 = HexMetrics.TerraceLerp(begin, left, 1);
        Color c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

        AddTriangle(begin, v2, boundary);
        AddTriangleColor(beginCell.Color, c2, boundaryColor);

        for (int i = 2; i < HexMetrics.terraceSteps; i++)
        {
            Vector3 v1 = v2;
            Color c1 = c2;
            v2 = HexMetrics.TerraceLerp(begin, left, i);
            c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
            AddTriangle(v1, v2, boundary);
            AddTriangleColor(c1, c2, boundaryColor);
        }

        AddTriangle(v2, left, boundary);
        AddTriangleColor(c2, leftCell.Color, boundaryColor);
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

    private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.AddRange(new Vector3[4] { v1, v2, v3, v4 });
        triangles.AddRange(new int[6] { vertexIndex, vertexIndex + 2, vertexIndex + 1, vertexIndex + 1, vertexIndex + 2, vertexIndex + 3 });
    }
    private void AddQuadColor(Color c1, Color c2)
    {
        colors.AddRange(new Color[4] { c1, c1, c2, c2 });
    }
    private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
    {
        colors.AddRange(new Color[4] { c1, c2, c3, c4 });
    }
}