using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 2F;
    public float sensitivityY = 2F;
    public float minimumY = -90F;
    public float maximumY = 90F;
    
    public float rotationX = 0F;
    public float rotationY = 0;
    public float cameraSpeed = 0.1f;

    public void Update()
    {
        // MouseInput();
        KeyInput();
    }

    private void KeyInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetKey(KeyCode.W)) MouseWButtonHold();
        else if (Input.GetKey(KeyCode.S))MouseSButtonHold();
        if (Input.GetKey(KeyCode.A))MouseAButtonHold();
        else if (Input.GetKey(KeyCode.D))MouseDButtonHold();
    }

    private void MouseInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (Input.GetMouseButton(0))
        {
            MouseLeftButtonClicked();
        }
        else if (Input.GetMouseButton(1))
        {
            MouseRightButtonClick();
        }
        else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            ShowAndUnlockCursor();
        }
        else
        {
            MouseWheeling();
        }
    }

    private void MouseWButtonHold()
    {
        var pos = transform.position;
        pos.y -= cameraSpeed;
        transform.position = pos;
    }
    private void MouseSButtonHold()
    {
        var pos = transform.position;
        pos.y += cameraSpeed;
        transform.position = pos;
    }
    private void MouseAButtonHold()
    {
        var pos = transform.position;
        pos.x += cameraSpeed;
        transform.position = pos;
    }
    private void MouseDButtonHold()
    {
        var pos = transform.position;
        pos.x -= cameraSpeed;
        transform.position = pos;
    }

    private static void ShowAndUnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private static void HideAndLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MouseLeftButtonClicked()
    {
        HideAndLockCursor();
        var newPos = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
        var pos = transform.position;
        if (newPos.x > 0.0f) pos -= transform.right * cameraSpeed;
        if (newPos.x < 0.0f) pos += transform.right * cameraSpeed;
        if (newPos.z > 0.0f) pos -= transform.up * cameraSpeed;
        if (newPos.z < 0.0f) pos += transform.up * cameraSpeed;
        transform.position = pos ;
    }

    private void MouseRightButtonClick()
    {
        HideAndLockCursor();
        switch (axes)
        {
            case RotationAxes.MouseXAndY:
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
                break;
            case RotationAxes.MouseX:
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
                break;
            default:
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                break;
        }
    }

    private void MouseWheeling()
    {
        var pos = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            pos = pos - transform.forward;
            transform.position = pos;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            pos = pos + transform.forward;
            transform.position = pos;
        }
    }
}
