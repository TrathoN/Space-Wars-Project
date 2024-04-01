using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorRotator : MonoBehaviour
{
    private float speed = 75;
    public Material material;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "IndicatorRing_01")
        {
            transform.Rotate(Vector3.up * Time.deltaTime * speed);
        }

        if (gameObject.name == "IndicatorRing_03")
        {
            transform.Rotate(Vector3.down * Time.deltaTime * speed);
        }
    }

    public void SetIndicatorColor(PowerUpType powerUpType)
    {
        if(powerUpType == PowerUpType.Damage)
        {
            material.color = new Color(100, 0, 0, 255);
        }

        if(powerUpType == PowerUpType.AttackMovementSpeed)
        {
            material.color = new Color(100, 100, 0, 255);
        }

        if(powerUpType == PowerUpType.HomingProjectile)
        {
            material.color = new Color(100, 0, 100, 255);
        }
    }

}
