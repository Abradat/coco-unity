using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour
{
    public EventManager eventManager;
    private static ServiceLocator instance;
    public static ServiceLocator Instance;

    private void Awake()
    {
        instance = this;

        if (Instance == null)
            Instance = instance;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SetupInterfaces();
    }

    private void SetupInterfaces()
    {
        eventManager = new EventManager();
    }
}
