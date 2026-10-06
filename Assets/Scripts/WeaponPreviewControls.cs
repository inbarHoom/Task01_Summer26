using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Unity.Mathematics;

public class WeaponPreviewControls : MonoBehaviour , IPointerDownHandler, IPointerUpHandler , IDragHandler
{
    [SerializeField] private HUD_Manager hudManager;

    [Header("Weapon")]
    [SerializeField] private Transform previewPivot;
    private Vector3 originalScale;
    [SerializeField] private Transform previewCamera;
    [SerializeField] private float moveSensitivity = 0.005f;
    [Header("Finger Gestures")]
    private bool isHolding;
    private bool tapCancelled;
    private float pressStartTime;
    [SerializeField] private float holdDuration = 0.3f;
    [SerializeField] private float rotationSensitivity = 0.3f;
    private Dictionary<int, Vector2> activePointers = new();
    private float pinchStartDistance;
    private Vector3 pinchStartScale;
    private Vector2 moveStartMidpoint;
    private Vector3 moveStartPosition;
    private Vector3 originalPosition;
    [SerializeField] private float doubleTapWindow = 0.35f;
    private float lastTapTime = float.NegativeInfinity;
    
    void Start()
    {
        originalScale = previewPivot.localScale;
        originalPosition = previewPivot.localPosition;
    }

    void Update()
    {
        
    }

    private void RegisterTap()
    {
        if (Time.unscaledTime - lastTapTime <= doubleTapWindow)
        {
            lastTapTime = float.NegativeInfinity;
            hudManager.ChoosePreviewWeapon();
        }
        else
        {
            lastTapTime = Time.unscaledTime;
        }
    }

    private float GetPinchDistance()
    {
        var positions = new List<Vector2>(activePointers.Values);
        return Vector2.Distance(positions[0], positions[1]);
    }

    private Vector2 GetFingerMidPoint()
    {
        var  positions = new List<Vector2>(activePointers.Values);
        return (positions[0] + positions[1]) / 2f;
    }

 

    public void OnDrag(PointerEventData eventData)
    {
        tapCancelled = true;
        activePointers[eventData.pointerId] = eventData.position;
        if (activePointers.Count == 2)
        {
            if (pinchStartDistance <= 0f) return;

            float ratio = GetPinchDistance() / pinchStartDistance;
            float sizeMultiplier = (pinchStartScale.x / originalScale.x) * ratio;
            sizeMultiplier = Mathf.Clamp(sizeMultiplier, 0.5f, 2f);
            previewPivot.localScale = originalScale * sizeMultiplier;
            if (Time.unscaledTime - pressStartTime >= holdDuration)
            {
                Vector2 delta = GetFingerMidPoint() - moveStartMidpoint;

                Vector3 worldOffset =
                    (previewCamera.right * delta.x +
                     previewCamera.up * delta.y) * moveSensitivity;

                Vector3 localOffset =
                    previewPivot.parent.InverseTransformVector(worldOffset);

                previewPivot.localPosition = moveStartPosition + localOffset;
            }
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
            tapCancelled = false;
        }

        if (activePointers.Count == 2)
        {
            pinchStartDistance = GetPinchDistance();
            pinchStartScale = previewPivot.localScale;
            moveStartMidpoint = GetFingerMidPoint();
            moveStartPosition = previewPivot.localPosition;
            tapCancelled  = true;
        }
        //Debug.Log(activePointers.Count);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        activePointers.Remove(eventData.pointerId);
        if(activePointers.Count == 0)
        {
            isHolding = false;
            previewPivot.localPosition = originalPosition;
            if(!tapCancelled && Time.unscaledTime - pressStartTime < holdDuration) RegisterTap();
        }
    }
}
