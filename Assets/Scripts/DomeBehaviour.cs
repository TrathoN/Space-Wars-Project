using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DomeBehaviour : MonoBehaviour
{
    [SerializeField] private float domeSpeed;

    // Update is called once per frame 
    void Update()
    {
        transform.Rotate(Vector3.right * Time.deltaTime * domeSpeed);
    }
}
