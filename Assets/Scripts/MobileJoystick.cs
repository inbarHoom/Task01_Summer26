using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IDragHandler ,IPointerDownHandler ,IPointerUpHandler
{
    [SerializeField] private RectTransform joystickBase;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float radius = 60f;
    public static Vector2 Direction { get; private set; }
    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
       // joystickHandle.anchoredPosition = localPoint;
       joystickHandle.anchoredPosition = Vector2.ClampMagnitude(localPoint, radius);
       Direction = joystickHandle.anchoredPosition / radius;
        Debug.Log(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
