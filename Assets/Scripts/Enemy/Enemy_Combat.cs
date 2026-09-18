using System;
using UnityEngine;
using System.Collections;

public class Enemy_Combat : MonoBehaviour
{
    private GameObject prefab;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private int health;
    [SerializeField] private float duration;
    [SerializeField] private float attackDuration = 5f;
    [SerializeField] private float nextAttackTime = 0f;
    private bool hasReturned = false;
    public static event Action<int> OnHit;
    public  event Action OnDeath;
 

   public void Initialize(GameObject originalPrefab)
    {
        prefab =  originalPrefab;
        health = enemyData.Health;
        nextAttackTime = 0f;
        hasReturned = false;
        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    void ReturnEnemy()
    {
        if (hasReturned) return;
        hasReturned = true;
        OnDeath?.Invoke();
        Pool_Manager.Instance.ReturnObject(prefab, gameObject);
    }
   
    void Update()
    {
        CheckHealth();
    }

    void CheckHealth()
    {
        if(health <= 0)
        {
           ReturnEnemy();
        }
    }
    public void TakeDamage(int damage)
    {
        if(hasReturned) return;
        health -= damage;
        StartCoroutine(HurtEnemy());
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Attack();
        }
    }
    private IEnumerator HurtEnemy()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = Color.darkRed;

        yield return new WaitForSeconds(duration);

        gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            OnHit.Invoke(enemyData.Damage);
            nextAttackTime =  Time.time + attackDuration;
        }
    }
}
