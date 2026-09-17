using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    [SerializeField] private GameObject playerReferance;
    [SerializeField] private EnemyData enemyData;
    private Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction = playerReferance.transform.position - transform.position;
        transform.position += direction.normalized * enemyData.Speed * Time.deltaTime;
    }
    
}
