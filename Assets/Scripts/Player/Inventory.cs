using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    [SerializeField] private Transform hands;
    private GameObject equippedWeaponObject;
    [SerializeField] private List <WeaponSO>  weapons = new List<WeaponSO>(3);
    
    public static event System.Action<Weapon_Behavior> OnEquipped;
public Weapon_Behavior equipedWeapon { get; private set; }

   public void EquipWeapon(int hoveredSlotIndex)
    {
        WeaponSO selectedWeapon = weapons[hoveredSlotIndex];
        if (equipedWeapon != null &&
            equipedWeapon.WeaponData == selectedWeapon)
        {
            return;
        }
        if (selectedWeapon is not WeaponSO weaponSo) return;

        if (equippedWeaponObject != null)
        {
            Destroy(equippedWeaponObject);
        }

        equippedWeaponObject = Instantiate(selectedWeapon.WeaponPrefab, hands);
        equippedWeaponObject.transform.localPosition = Vector3.zero;
         equipedWeapon = equippedWeaponObject.GetComponent<Weapon_Behavior>();
         OnEquipped.Invoke(equipedWeapon);
    }
}
