using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
  [SerializeField] private e_WeaponName weaponName;
  [SerializeField] private List<AmmoSO> ammoOptions;
  [SerializeField] private int capacity;
  [SerializeField] private Sprite icon;
  [SerializeField] private GameObject weaponPrefab;

   public e_WeaponName WeaponName => weaponName;
   
   public int Capacity => capacity;
   public Sprite Icon => icon;

   public GameObject WeaponPrefab
   {
       get { return weaponPrefab ; }
       set {weaponPrefab =  value ; }
   }
   
   public List<AmmoSO> AmmoOptions => ammoOptions;
}

public enum e_WeaponName
{
    Revolver,
    M16,
    Glock
}
