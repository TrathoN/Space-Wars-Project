using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    None,
    Health,
    Damage,
    AttackMovementSpeed,
    HomingProjectile,
    Nuke,
}
public class PowerUpBehaviour : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float speed;
    [SerializeField] private float positionLimit;
    public PowerUpType powerUpType;
    // Update is called once per frame
    void Update()
    {
        PowerUpMovement();
        if (transform.position.z < positionLimit)
        {
            Destroy(gameObject);
        }
    }

    private void PowerUpMovement()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.back * Time.deltaTime * speed, Space.World);
    }
}
