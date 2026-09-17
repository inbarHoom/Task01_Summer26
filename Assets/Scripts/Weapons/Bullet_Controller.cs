using System;
using UnityEngine;

public class Bullet_Controller : MonoBehaviour
{
    private float speed;
    private float lifetime;
    private int damage;
    public void Initialize(AmmoSO ammoData)
    {
        speed = ammoData.BulletSpeed;
        lifetime = ammoData.BulletLifeTime;
        damage = ammoData.Damage;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
         if (other.tag == "Enemies")
         {
             other.gameObject.GetComponent<Enemy_HealthTest>().TakeDamage(damage);
         }
         Destroy(gameObject);
    }

  
}
