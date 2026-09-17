using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    float x , z  , speed = 5f;
    
    void Update()
    {
        GetDirection();
    }

    public void Move(float x , float z)
    {
            transform.Translate(x * speed * Time.deltaTime,0,z * speed * Time.deltaTime);
    }

    void GetDirection()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            z = 1f;
            x = 0f;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            z = 0f;
            x = -1f;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            z = -1;
            x = 0f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            z = 0;
            x = 1f;
        }

        if (!Keyboard.current.anyKey.isPressed)
        {
            z = 0f;
            x = 0f;
        }
        Move(x,z);
    }
}
