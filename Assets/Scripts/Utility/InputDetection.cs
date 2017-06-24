using UnityEngine;

public static class InputDetection
{
    public delegate void InputAction();

    public static float Horizontal { get { return Input.GetAxisRaw("Horizontal"); } }
    public static float Vertical { get { return Input.GetAxisRaw("Vertical"); } }

    public static bool interactDown;
    public static bool Interact(InputAction action)
    {
        if (Input.GetAxisRaw("Interact") != 0f)
        {
            if (interactDown == false)
            {
                action.Invoke();
                interactDown = true;
            }
        }

        if (Input.GetAxisRaw("Interact") == 0f)
            interactDown = false;

        return interactDown;
    }

    public static bool Any { get { return Horizontal != 0 || Vertical != 0 || Interact(() => { }); } }
}