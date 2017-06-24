using UnityEngine;

public class Interaction : MonoBehaviour
{
    public HexMapCamera cameraRig;
    public GameObject marker;

    public Player playerObject;

    public bool buttonDown;

    void Update()
    {
        DebugExtension.DebugPoint(cameraRig.Point, Color.red);
        InputDetection.Interact(() => { HandleInput(); });

        marker.transform.position = cameraRig.Point + (Vector3.up * 10f);
    }

    private void HandleInput()
    {
        SceneManager.Instance.PlayerRef.EndCubeCoordTarget = GetHexAt(cameraRig.Point);
    }

    private CubeCoord GetHexAt(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        return Hex.FromPosition(position);
    }
}