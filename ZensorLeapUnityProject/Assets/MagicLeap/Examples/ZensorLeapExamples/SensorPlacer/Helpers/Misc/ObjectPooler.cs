// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using MagicLeap.Utilities;

namespace MagicKit
{
    ///<summary>
    /// Handles pooled allocation of gameobject instances.
    ///</summary>
    public class ObjectPooler : Singleton<ObjectPooler>
    {
        //----------- Private Members -----------

        [SerializeField] private List<ObjectToPool> _objectsToPool; // here the user can input a lot of different gameobjects, each one will be pooled
        [SerializeField] bool _hideInHierarchy;
        private Dictionary<string, List<GameObject>> _pools; // key = name of GameObject, Value is a the pool of objects

        //----------- MonoBehaviour Methods -----------

        private void Awake()
        {
            _pools = new Dictionary<string, List<GameObject>>();

            //initialize all of the gameobjects
            foreach (ObjectToPool obj in _objectsToPool)
            {
                //if obj.objectsToPool' contains prefabs, we need to instantiate everything, and disable it.
                if (obj.instantiatePool)
                {
                    List<GameObject> list = new List<GameObject>();
                    GameObject folder = new GameObject("[" + obj.name + "]");
                    folder.transform.SetParent(this.transform);
                    for (int i = 0; i < obj.poolSize; i++)
                    {
                        GameObject newObject = (GameObject)Instantiate(obj.GetObject()); // instantiate the gameobject
                        if (_hideInHierarchy)
                        {
                            newObject.hideFlags = HideFlags.HideInHierarchy;
                        }
                        list.Add(newObject);//add it to the list
                        newObject.transform.SetParent(folder.transform);
                        newObject.SetActive(false);//disable the object
                    }
                    //store the list in teh dictionary, with the object.objectToPool's name as a key
                    _pools.Add(obj.name, list);
                    //Debug.Log("Adding key: " + obj.name);
                }
                else //otherwise, these objects are already in the scene. we'll just disable them
                {
                    foreach (GameObject go in obj.objectsToPool)
                    {
                        go.SetActive(false);
                    }
                    _pools.Add(obj.name, obj.objectsToPool);
                }
            }
        }

        //----------- Public Methods -----------

        /// <summary>
        /// Grab an object from the pool.
        /// Note that the object returned will be disabled, this is because the object may have some forces acting on it, or other states that are still active from when it was disabled.
        /// After you grab the object, reset it's state when desired before enabling it.
        /// </summary>
        public GameObject GetObject(string key, bool removeFromPool = false)
        {
            //get the object pool for this object out of the dictionary
            List<GameObject> objectPool = _pools[key];

            //return the first inactive object                        
            for (int i = 0; i < objectPool.Count; i++)
            {
                if (!objectPool[i].activeInHierarchy)
                {
                    GameObject toReturn = objectPool[i];
                    if (removeFromPool)
                    {
                        objectPool.Remove(toReturn);
                    }
                    return toReturn;
                }
            }

            //if we reach this code, the object pool is empty, or all objects in the pool are active
            //grow if we've said we can grow
            foreach (ObjectToPool obj in _objectsToPool) // go through our list of objects
            {
                //find the object with the same name
                if (obj.name == key)
                {
                    //see if it can grow
                    if (obj.canGrow)
                    {
                        //Debug.Log("can grow");
                        GameObject newObject = (GameObject)Instantiate(obj.GetObject()); // instantiate the gameobject
                        if (_hideInHierarchy)
                        {
                            newObject.hideFlags = HideFlags.HideInHierarchy;
                        }

                        if (!removeFromPool)
                        {
                            objectPool.Add(newObject);//add it to the list
                        }

                        _pools[key] = objectPool;//since the list has changed, store it in the dictionary
                        newObject.SetActive(false);
                        return newObject;
                        //TODO: check to see if we even need this last line, I think we do since it's pass by reference not pass by value
                    }
                    break;
                }
            }

            Debug.Log("Couldn't get : " + key);

            return null;
        }

    }

    [System.Serializable]
    public class ObjectToPool
    {
        public string name; // not necessary but nice for inspectors
        public List<GameObject> objectsToPool;
        public bool instantiatePool = true;
        public int poolSize;
        public bool canGrow;
        //public bool canShrink? shouldn't be necessary

        //get a random object form the list of "objectsToPool"
        public GameObject GetObject()
        {
            if (objectsToPool.Count == 0)
            {
                Debug.LogError("Object: " + name + " in the objectpooler doesn't ahve any object prefabs assigned");
            }
            return objectsToPool[Random.Range(0, objectsToPool.Count)];
        }
    }
}

