using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CubeCoord _currentCubCoord;
    public CubeCoord CurrentCubeCoord
    {
        get { return _currentCubCoord = Hex.FromPosition(transform.position); }
    }

    [SerializeField]
    private CubeCoord startingCubeCoord;
    public Vector3 VectorPosition
    {
        get
        {
            return HexGrid.Instance.FindHexObject(CurrentCubeCoord).Position;
        }
    }

    private CubeCoord _endCubeCoordTarget;
    public CubeCoord EndCubeCoordTarget
    {
        get { return _endCubeCoordTarget; }
        set 
        {
            _endCubeCoordTarget = value;
        }
    }

    private void Start()
    {
        transform.position = HexGrid.Instance.FindHexObject(startingCubeCoord).Position;
    }

    void Update()
    {

    }
}
