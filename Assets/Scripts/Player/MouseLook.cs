using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
 [SerializeField] private float sensitivity = 0.1f;
 [SerializeField] private Transform player;
    private float rotationX;
    private float rotationY;

    public static bool lockedScreen = false;
    [SerializeField] private TextMeshProUGUI startText;

    void Start()
    {
        rotationX = transform.eulerAngles.x;
        if (rotationX > 180f)
            rotationX -= 360f;

        rotationY = player.eulerAngles.y;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame )
        {
            Cursor.lockState = CursorLockMode.None;
            lockedScreen = false;
            Cursor.visible = true;
            startText.enabled = true;
        }

        if (Mouse.current == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                lockedScreen = true;
                Cursor.visible = false;
                startText.enabled = false;
            }

            return;
        }

        Vector2 mouseMovement = Mouse.current.delta.ReadValue();

        rotationY += mouseMovement.x * sensitivity;
        rotationX -= mouseMovement.y * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -85f, 85f);

        player.rotation = Quaternion.Euler(0f, rotationY, 0f);
        transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }
}
