using UnityEngine;

[CreateAssetMenu(fileName = "AmmoSO", menuName = "Scriptable Objects/AmmoSO")]
public class AmmoSO : ScriptableObject
{
   [SerializeField] private string caliberName;
   [SerializeField] private int damage;
   [SerializeField] private float bulletSpeed;
   [SerializeField] private float bulletLifeTime;
   [SerializeField] private GameObject bulletPrefab;
   
   public string CaliberName { get => caliberName; set => caliberName = value; }
   public GameObject BulletPrefab { get { return bulletPrefab; } set { bulletPrefab = value; } }
   public int Damage { get { return damage; } set { damage = value; } }
   public float BulletSpeed { get { return bulletSpeed; } set { bulletSpeed = value; } }
   public float BulletLifeTime { get { return bulletLifeTime; } set { bulletLifeTime = value; } }
}
