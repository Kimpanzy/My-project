using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyConsumable : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1;
    void Start()
    {
        Destroy(gameObject, lifeTime); 
    }
}
