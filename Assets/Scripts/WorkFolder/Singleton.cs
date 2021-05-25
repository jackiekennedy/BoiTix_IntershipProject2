using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Component
{
    static public T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (!Instance)
            Instance = this as T;
        else
            Destroy(this);

        AwakeInit();
    }

    public virtual void AwakeInit()
    {

    }
}
