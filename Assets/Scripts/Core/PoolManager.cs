using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    [System.Serializable]
    public class PoolConfig
    {
        public GameObject prefab;
        public int initialSize = 10;
    }

    [SerializeField] private PoolConfig[] configs;

    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();
    private readonly Dictionary<GameObject, GameObject> prefabLookup = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        foreach (var c in configs)
            if (c.prefab != null) Prewarm(c.prefab, c.initialSize);
    }

    public void Prewarm(GameObject prefab, int count)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();
        for (int i = 0; i < count; i++)
        {
            var obj = CreateNew(prefab);
            obj.SetActive(false);
            pools[prefab].Enqueue(obj);
        }
    }

    private GameObject CreateNew(GameObject prefab)
    {
        var obj = Instantiate(prefab);
        prefabLookup[obj] = prefab;
        return obj;
    }

public GameObject Get(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();
        var obj = pools[prefab].Count > 0 ? pools[prefab].Dequeue() : CreateNew(prefab);
        obj.SetActive(true);
        foreach (var p in obj.GetComponents<IPoolable>()) p.OnGetFromPool();
        return obj;
    }

    public T Get<T>(GameObject prefab) where T : Component
        => Get(prefab).GetComponent<T>();

public void Return(GameObject obj)
    {
        if (obj == null) return;
        foreach (var p in obj.GetComponents<IPoolable>()) p.OnReturnToPool();
        obj.SetActive(false);
        if (prefabLookup.TryGetValue(obj, out var prefab))
            pools[prefab].Enqueue(obj);
        else
            Destroy(obj);
    }
}
