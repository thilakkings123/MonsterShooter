using UnityEngine;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour
{

    public static ObjectPooling instance;

    public GameObject bulletPrefab, bulletEffect, poisonBullet, poisonEffect, boomPrefab;  //ref to bullet prefab
    public int count = 3; //total clones of each object to be spawned

    List<GameObject> SpawnBullet = new List<GameObject>();          //list to add them
    List<GameObject> SpawnPoisonBullet = new List<GameObject>();    //list to add them
    List<GameObject> SpawnBulletEffect = new List<GameObject>();    //list to add them
    List<GameObject> SpawnPoisonEffect = new List<GameObject>();    //list to add them
    List<GameObject> SpawnBoom = new List<GameObject>();            //list to add them

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //Bullet
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnBullet.Add(obj);
        }

        //PoisonBullet
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(poisonBullet);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnPoisonBullet.Add(obj);
        }

        //BulletEffect
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(bulletEffect);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnBulletEffect.Add(obj);
        }

        //PoisonEffect
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(poisonEffect);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnPoisonEffect.Add(obj);
        }

        //Boom
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(boomPrefab);
            obj.transform.parent = gameObject.transform;
            obj.SetActive(false);
            SpawnBoom.Add(obj);
        }

    }

    //method which is used to call from other scripts to get the clone object
    //Bullet
    public GameObject GetBullet()
    {
        for (int i = 0; i < SpawnBullet.Count; i++)
        {
            if (!SpawnBullet[i].activeInHierarchy)
            {
                return SpawnBullet[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(bulletPrefab);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnBullet.Add(obj);
        return obj;
    }

    //PosionBullet
    public GameObject GetPoisonBullet()
    {
        for (int i = 0; i < SpawnPoisonBullet.Count; i++)
        {
            if (!SpawnPoisonBullet[i].activeInHierarchy)
            {
                return SpawnPoisonBullet[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(poisonBullet);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnPoisonBullet.Add(obj);
        return obj;
    }

    //BulletEffect
    public GameObject GetBulletEffect()
    {
        for (int i = 0; i < SpawnBulletEffect.Count; i++)
        {
            if (!SpawnBulletEffect[i].activeInHierarchy)
            {
                return SpawnBulletEffect[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(bulletEffect);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnBulletEffect.Add(obj);
        return obj;
    }

    //PoisonEffect
    public GameObject GetPoisonEffect()
    {
        for (int i = 0; i < SpawnPoisonEffect.Count; i++)
        {
            if (!SpawnPoisonEffect[i].activeInHierarchy)
            {
                return SpawnPoisonEffect[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(poisonEffect);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnPoisonEffect.Add(obj);
        return obj;
    }

    //Boom
    public GameObject GetBoom()
    {
        for (int i = 0; i < SpawnBoom.Count; i++)
        {
            if (!SpawnBoom[i].activeInHierarchy)
            {
                return SpawnBoom[i];
            }
        }
        GameObject obj = (GameObject)Instantiate(boomPrefab);
        obj.transform.parent = gameObject.transform;
        obj.SetActive(false);
        SpawnBoom.Add(obj);
        return obj;
    }

}//class