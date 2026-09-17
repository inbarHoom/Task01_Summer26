using UnityEngine;

public class Replace_Character : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject character1;
    [SerializeField] private GameObject character2;
    [SerializeField] private GameObject character3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        Instantiate(character1, playerRef.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
