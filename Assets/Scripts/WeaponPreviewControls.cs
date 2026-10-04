using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class WeaponPreviewControls : MonoBehaviour , IPointerDownHandler, IPointerUpHandler , IDragHandler
{
    [Header("Weapon")]
    [SerializeField] private Transform previewPivot;
    private Vector3 originalScale;
    [Header("Finger Gestures")]
    private bool isHolding;
    private float pressStartTime;
    [SerializeField] private float holdDuration = 0.3f;
    [SerializeField] private float rotationSensitivity = 0.3f;
    private Dictionary<int, Vector2> activePointers = new();
    private float pinchStartDistance;
    private Vector3 pinchStartScale;
    void Start()
    {
        originalScale = previewPivot.localScale;
    }

    void Update()
    {
        
    }

    private float GetPinchDistance()
    {
        var positions = new List<Vector2>(activePointers.Values);
        return Vector2.Distance(positions[0], positions[1]);
    }

    public void OnDrag(PointerEventData eventData)
    {
        activePointers[eventData.pointerId] = eventData.position;
        if (activePointers.Count == 2)
        {
            if (pinchStartDistance <= 0f) return;

            float ratio = GetPinchDistance() / pinchStartDistance;
            float sizeMultiplier = (pinchStartScale.x / originalScale.x) * ratio;
            sizeMultiplier = Mathf.Clamp(sizeMultiplier, 0.5f, 2f);
            previewPivot.localScale = originalScale * sizeMultiplier;
            return;
        }
        if(!isHolding || Time.unscaledTime - pressStartTime < holdDuration) return;
        if(activePointers.Count != 1)return;
        previewPivot.Rotate(Vector3.up, -eventData.delta.x * rotationSensitivity, Space.World);
      
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        activePointers[eventData.pointerId] = eventData.position;
        if (activePointers.Count == 1)
        {
            isHolding = true;
            pressStartTime = Time.unscaledTime;
        }

        if (activePointers.Count == 2)
        {
            pinchStartDistance = GetPinchDistance();
            pinchStartScale = previewPivot.localScale;
        }
        Debug.Log(activePointers.Count);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        activePointers.Remove(eventData.pointerId);
        if(activePointers.Count == 0)
        {
            isHolding = false;
        }
    }
}
