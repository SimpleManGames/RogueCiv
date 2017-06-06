using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
    public HexObject[] Hexes
    {
        get; private set;
    }

    HexMesh hexMesh;

    public void Refresh()
    {
        enabled = true;
    }

    public void AddHex(int index, HexObject hex)
    {
        Hexes[index] = hex;
        hex.chunk = this;
        hex.transform.SetParent(transform, false);
    }

    void Awake()
    {
        Hexes = new HexObject[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
        hexMesh = GetComponentInChildren<HexMesh>();
    }

    public void LateUpdate()
    {
        hexMesh.Triangulate(Hexes);
        enabled = false;
    }
}
