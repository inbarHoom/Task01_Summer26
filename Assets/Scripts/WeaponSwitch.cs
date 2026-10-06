using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponSwitch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector2 swipeStartPosition;
    [SerializeField] private float minSwipeDistance = 30f;
    [SerializeField] private HUD_Manager hud_Manager;
    public void OnPointerDown(PointerEventData eventData)
    {
       swipeStartPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 swipe = eventData.position - swipeStartPosition;
        if(Mathf.Abs(swipe.x) < minSwipeDistance ||  Mathf.Abs(swipe.y) >= Mathf.Abs(swipe.x)) return;
        switch (swipe.x)
        {
            case >0:
                hud_Manager.SwitchPreview(-1);
                break;
            case <0:
                hud_Manager.SwitchPreview(1);
                break;
        }
        
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
