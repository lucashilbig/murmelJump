using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlatformObjectPool : MonoBehaviour
{
    private static PlatformObjectPool _instance;
    public static PlatformObjectPool Instance => _instance;

    public GameObject[] objectsToPool = new GameObject[Enum.GetNames(typeof(PlatformType)).Length];
    public int amountToPool;
    private Dictionary<PlatformType, ObjectPool<GameObject>> _pooledObjects = new();
    private GameObject _platformParent;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Start()
    {
        _platformParent = new GameObject("Platforms");
        var scaler = GameObject.Find("Scaler");
        if (scaler != null)
            _platformParent.transform.SetParent(scaler.transform);
        foreach (var prefab in objectsToPool)
        {
            var platformType = prefab.GetComponent<Platform>().type;
            if(_pooledObjects.ContainsKey(platformType)) continue;
            var pool = new ObjectPool<GameObject>(() => Instantiate(prefab, _platformParent.transform, true), 
                obj => obj.SetActive(true), obj => obj.SetActive(false), 
                obj => Destroy(obj), false, amountToPool, amountToPool);
            _pooledObjects.Add(platformType, pool);
        }
    }

    public GameObject Get(PlatformType type) => _pooledObjects[type].Get();

    public void Release(GameObject platformObject)
    {
        //destroy items on platform gameobject
        for (int i = 0; i < platformObject.transform.childCount; i++)
            Destroy(platformObject.transform.GetChild(i).gameObject);
        
        var platform = platformObject.GetComponent<Platform>();
        if (platform == null) return;
        _pooledObjects[platform.type].Release(platformObject);
    }

    public List<GameObject> GetActivePlatforms()
    {
        var activePlatforms = new List<GameObject>();
        for (int i = 0; i < _platformParent.transform.childCount; i++)
        {
            var child = _platformParent.transform.GetChild(i);
            if (child.gameObject.activeInHierarchy)
                activePlatforms.Add(child.gameObject);
        }

        return activePlatforms;
    }

    private void OnDestroy()
    {
        foreach (var pair in _pooledObjects) 
            pair.Value.Dispose();
        Destroy(_platformParent);
    }
}
