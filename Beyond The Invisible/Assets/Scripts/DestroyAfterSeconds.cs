using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float lifeTime = 1f;
    float count = 0;

    private void Start()
    {
        count = lifeTime;
    }
    void Update()
    {
        count-= Time.deltaTime;
        if (count <= 0) 
        {
        Destroy(gameObject);
        }
    }
}
