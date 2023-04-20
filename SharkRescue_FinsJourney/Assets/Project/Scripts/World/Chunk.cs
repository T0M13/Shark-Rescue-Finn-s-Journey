using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public float MovingSpeed = 5;
    
    void Update()
    {
        gameObject.transform.position += new Vector3(MovingSpeed * Time.deltaTime, 0, 0);
    }



    private void OnTriggerEnter(Collider other)
    {
        
    }
}
