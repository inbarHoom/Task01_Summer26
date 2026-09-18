using System;
using UnityEngine;

public class Bullet_Controller : MonoBehaviour
{
    private GameObject prefab;
    private float speed;
    private float lifetime;
    private int damage;
    private bool hasReturned = false;
    
    public static event Action<int> OnBulletHit;
    public void Initialize(AmmoSO ammoData)
    {
        prefab = ammoData.BulletPrefab;
        speed = ammoData.BulletSpeed;
        lifetime = ammoData.BulletLifeTime;
        damage = ammoData.Damage;
        hasReturned = false;
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
           // Pool_Manager.Instance.ReturnObject(prefab, gameObject);
           ReturnBullet();
            return;
        }
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hasReturned) return;
         if (other.tag == "Enemies")
         {
             other.gameObject.GetComponent<Enemy_Combat>().TakeDamage(damage);
         }
        // Pool_Manager.Instance.ReturnObject(prefab ,gameObject);
        ReturnBullet();
    }

    private void ReturnBullet()
    {
        if (hasReturned) return;
        hasReturned = true;
        Pool_Manager.Instance.ReturnObject(prefab, gameObject);
    }
  
}
