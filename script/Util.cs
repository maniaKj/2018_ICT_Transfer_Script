using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    
}

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    static private T s_Instance = null;
    static public T Instance
    {
        get
        {
            if(s_Instance == null)
            {
                s_Instance = FindObjectOfType<T>();
            }

            return s_Instance;
        }
    }
}
