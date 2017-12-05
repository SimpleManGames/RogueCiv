using UnityEngine;

public class Interaction : MonoBehaviour
{
    public enum ActionMode
    {
        None,
        MovePlayer,
        BuildRoad
    }

    public ActionMode currentActionMode;

    public HexMapCamera cameraRig;
    public GameObject marker;

    public Player playerObject;

    public bool buttonDown;

    [SerializeField]
    private bool isDragging;
    private HexDirection dragDirection;
    private HexObject previousCell;

    void Update()
    {
        DebugExtension.DebugPoint(cameraRig.Point, Color.red);
        InputDetection.InteractHold(() => { HandleHoldInput(); });

        marker.transform.position = cameraRig.Point + (Vector3.up * 10f);
    }

    private void HandleHoldInput()
    {
        Ray inputRay = cameraRig.Camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexObject currentCell = HexGrid.Instance.FindHexObject(GetHexAt(hit.point));
            if (previousCell && previousCell != currentCell)
                ValidateDrag(currentCell);
            else
                isDragging = false;

            UpdateHex(currentCell);
            previousCell = currentCell;
        }
        else
        {
            previousCell = null;
        }
    }

    private void ValidateDrag(HexObject cell)
    {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if (Hex.Neighbour(previousCell.Hex, (byte)dragDirection) == cell.Hex)
            {
                isDragging = true;
                return;
            }
        }
        isDragging = false;
    }

    private void UpdateHex(HexObject hex)
    {
        Debug.Log("UpdateHex Func");
        if (hex)
        {
            if (isDragging)
            {
                HexObject otherCell = hex.GetNeighbour(dragDirection.Opposite());

                if (otherCell)
                {
                    if (currentActionMode == ActionMode.BuildRoad)
                        otherCell.AddRoad(dragDirection); // TODO: Add a cost to this for gameplay
                }
            }
        }
    }

    private CubeCoord GetHexAt(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        return Hex.FromPosition(position);
    }
}