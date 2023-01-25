using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public string moveVAxisName = "Vertical";
    public string moveHAxisName = "Horizontal";
    public string PushButtonName = "Push";

    public float moveV { get; private set; }
    public float moveH { get; private set; }
    public bool push { get; private set; }

    public Vector3 mousePos { get; private set; }

    private void Update()
    {
        moveV = Input.GetAxis(moveVAxisName);
        moveH = Input.GetAxis(moveHAxisName);
        push = Input.GetButton(PushButtonName);

        mousePos = Input.mousePosition;
    }
}