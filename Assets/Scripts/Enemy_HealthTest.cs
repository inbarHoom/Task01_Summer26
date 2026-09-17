using TMPro;
using UnityEngine;

public class Enemy_HealthTest : MonoBehaviour
{
   [SerializeField]private int health = 100;
    [SerializeField] private TextMeshPro healthText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowHealth();
    }

    void ShowHealth()
    {
        healthText.text =health.ToString();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
    }
}
