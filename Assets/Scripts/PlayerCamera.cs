using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;

    public Transform playerOrientation;

    float rotationX;
    float rotationY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f); //fix rotation to interval

        //cam and orientation
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0); //rotate cam along both axes
        playerOrientation.rotation = Quaternion.Euler(0, rotationY, 0); //rotate player along y axis
    }
}
