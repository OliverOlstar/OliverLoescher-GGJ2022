using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolElement : MonoBehaviour
{
    private string poolKey = "";
    private Transform parent = null;

    public virtual void Init(string pKey, Transform pParent)
    {
        poolKey = pKey;
        parent = pParent;
    }

    public virtual void ReturnToPool()
    {
        if (poolKey != "")
        {
            ObjectPoolDictionary.dictionary[poolKey].CheckInObject(gameObject, this, parent);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnExitPool()
    {

    }

    private void OnDestroy() 
    {
        if (poolKey != "" && ObjectPoolDictionary.dictionary.ContainsKey(poolKey))
        {
            ObjectPoolDictionary.dictionary[poolKey].ObjectDestroyed(gameObject, this);
        }
    }
}