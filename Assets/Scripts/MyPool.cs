using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPool : MonoBehaviour
{
    private Stack<GameObject> stack = new Stack<GameObject>();
    private GameObject baseObj;
    private GameObject tmp;
    private ReturnToMyPool returnPool;

    public MyPool(GameObject baseObj)
    {
        this.baseObj = baseObj;
    }

    public GameObject Get()
    {
        if (stack.Count > 0)
        {
            tmp = stack.Pop();
            tmp.SetActive(true);
            return tmp;
        }
        tmp = GameObject.Instantiate(baseObj);
        returnPool = tmp.AddComponent<ReturnToMyPool>();
        returnPool.pool = this;
        return tmp;
    }

    public void AddToPool(GameObject obj)
    {
        stack.Push(obj);
    }
}
