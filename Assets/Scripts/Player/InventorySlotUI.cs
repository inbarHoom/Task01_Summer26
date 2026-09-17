using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour , IPointerEnterHandler
{
    
    [SerializeField] private HUD_Manager hudManager;
    [SerializeField] private int slotIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        hudManager.SetHoveredSlot(slotIndex);
    }
}
