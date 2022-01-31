using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ObjectPoolDictionary : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        [DisableInPlayMode] [Required] public string key = "";
        [AssetsOnly] public GameObject prefab = null;
        [Tooltip("What returns when all items are already checked out")]
        [SerializeField] private PoolReturnType returnType = PoolReturnType.Expand;

        [Space]
        [HideInPlayMode] [SerializeField] private int startingCopies = 15;
        
        private List<PoolItem> itemsIn = new List<PoolItem>();
        private List<PoolItem> itemsOut = new List<PoolItem>();

        private Transform transform = null;

        public void Init(Transform pTransform)
        {
            transform = pTransform;

            while (startingCopies > 0)
            {
                itemsIn.Add(InstiateNewObject());
                startingCopies--;
            }
        }

        private PoolItem InstiateNewObject()
        {
            PoolItem item = new PoolItem();

            item.gameObject = Instantiate(prefab, transform);
            // item.gameObject.transform.SetParent(transform);

            item.element = item.gameObject.GetComponentInChildren<PoolElement>();
            if (item.element != null)
            {
                item.parent = item.gameObject.transform;
                item.gameObject = item.element.gameObject;
                item.element.Init(key, item.parent);
            }
            else
            {
                item.parent = transform;
            }

            item.gameObject.SetActive(false);
            return item;
        }

        public GameObject CheckOutObject(bool pEnable = false)
        {
            PoolItem item;
            // Grab Next
            if (itemsIn.Count > 0) 
            {
                item = itemsIn[0];
                itemsOut.Add(item);
                itemsIn.RemoveAt(0);
            }
            // Expand
            else if (returnType == PoolReturnType.Expand)
            {
                item = InstiateNewObject();
            }
            // Grab first out
            else if (returnType == PoolReturnType.Loop) 
            {
                if (itemsOut.Count > 0)
                {
                    // Return first to pool, then take it back out
                    item = itemsOut[0];
                    if (item.element != null)
                        item.element.ReturnToPool();
                    itemsIn.Remove(item);
                }
                else
                {
                    // If none out create new
                    item = InstiateNewObject();
                }
                itemsOut.Add(item);
            }
            else // (returnType == PoolReturnType.Null)
            {
                return null;
            }

            item.gameObject.SetActive(pEnable);
            if (item.element != null)
                item.element.OnExitPool();
            return item.gameObject;
        }

        public void CheckInObject(GameObject pObject, PoolElement pElement, Transform pParent, bool pDisable = true)
        {
            pObject.transform.SetParent(pParent);
            pObject.SetActive(!pDisable);

            PoolItem item = new PoolItem();
            item.gameObject = pObject;
            item.element = pElement;
            item.parent = pParent;

            itemsOut.Remove(item);
            itemsIn.Add(item);
        }

        public void ObjectDestroyed(GameObject pObject, PoolElement pElement)
        {
            PoolItem item = new PoolItem();
            item.gameObject = pObject;
            item.element = pElement;

            if (!itemsIn.Remove(item))
                itemsOut.Remove(item);
        }
    }

    public struct PoolItem
    {
        public Transform parent;
        public GameObject gameObject;
        public PoolElement element;
    }

    public enum PoolReturnType
    {
        Expand,
        Loop,
        Null
    }

    // Dictionary
    public static Dictionary<string, Pool> dictionary = new Dictionary<string, Pool>();
    [SerializeField] private Pool[] pools = new Pool[1];

    private void Awake() 
    {
        foreach (Pool p in pools)
        {
            // Add to Dictionary
            if (p.key != "")
            {
                if (dictionary.ContainsKey(p.key))
                {
                    Debug.LogError("[ObjectPoolDictionary] Dictionary already contains key, destroying self", gameObject);
                    p.key = ""; // Prevent OnDestroy from being called
                    Destroy(this);
                }
                else
                {
                    dictionary.Add(p.key, p);
                }
            }

            // Init
            Transform poolT = new GameObject("Pool " + p.key).transform;
            poolT.SetParent(transform);
            p.Init(poolT);
        }
    }

    private void OnDestroy() 
    {
        foreach (Pool p in pools)
        {
            // Remove to Dictionary
            if (p.key != "")
            {
                dictionary.Remove(p.key);
            }
        }
    }
}
