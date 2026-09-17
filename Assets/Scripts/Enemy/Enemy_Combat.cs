using System;
using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    public static event Action<int> OnHit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            OnHit.Invoke(damage);
        }
    }
}
