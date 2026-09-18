using UnityEngine;

public class Replace_Character : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    [SerializeField] private GameObject character1;
    [SerializeField] private GameObject character2;
    [SerializeField] private GameObject character3;
    void Start()
    { 
        Instantiate(character1, playerRef.transform);
    }
    
}
