using System;
using UnityEngine;

public class PlatformUI : MonoBehaviour
{
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private bool isMobile;
    public static bool UseMobileControls { get; private set;}

    private void Awake()
    {
        if (isMobile || Application.isMobilePlatform)
        {
            UseMobileControls = true;
        }
        else
        {
            UseMobileControls = false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            mobileControls.SetActive(UseMobileControls);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
