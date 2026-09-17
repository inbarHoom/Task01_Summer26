using UnityEngine;
using System.Collections.Generic;

public class Pool_Manager : MonoBehaviour
{
    private Dictionary<GameObject, Queue<GameObject>> pool = new();
    public static Pool_Manager Instance { get; private set; }
    [SerializeField] GameObject prefab;
    [SerializeField] private int amount;

    private void CreatePool(GameObject prefab)
    {
        if (pool.ContainsKey(prefab)) return;
        pool.Add(prefab, new Queue<GameObject>());
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (pool.ContainsKey(prefab))
        {
            if (pool[prefab].Count > 0)
            {
                GameObject instance = pool[prefab].Dequeue();
                instance.transform.SetPositionAndRotation(position, rotation);
                instance.SetActive(true);
                return instance;
            }

            return Instantiate(prefab, position, rotation);
        }

        CreatePool(prefab);
        return Instantiate(prefab, position, rotation);
    }

    public void ReturnObject(GameObject prefab, GameObject instance)
    {
        if (!pool.ContainsKey(prefab)) CreatePool(prefab);
        instance.SetActive(false);
        pool[prefab].Enqueue(instance);
    }

    void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }

            return;
        }

        Instance = this;
    }

    void Start()
    {
        CreatePool(prefab);
        for (int i = 0; i < amount; i++)
        {
            ReturnObject(prefab,Instantiate(prefab));
        }
    }

void Update()
    {
        
    }
}
