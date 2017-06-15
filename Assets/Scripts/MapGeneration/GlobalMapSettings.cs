using UnityEngine;

[ExecuteInEditMode]
public class GlobalMapSettings : Singleton<GlobalMapSettings>
{
    [SerializeField]
    private int chunkCountX;
    public int ChunkCountX
    {
        get { return chunkCountX; }
        set { chunkCountX = value; }
    }

    [SerializeField]
    private int chunkCountZ;
    public int ChunkCountZ
    {
        get { return chunkCountZ; }
        set { chunkCountZ = value; }
    }

    public int Width
    {
        get
        {
            return ChunkCountX * HexMetrics.Instance.chunkSizeX;
        }
    }

    public int Height
    {
        get
        {
            return ChunkCountZ * HexMetrics.Instance.chunkSizeZ;
        }
    }
}