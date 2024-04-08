using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float zMin;
    [SerializeField] private float zMax;
    private Transform target;
    private bool homing;
    private float aliveTimer = 5.0f;
    public int projectileDamage;

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("SpawnManager").GetComponent<SpawnManager>().isGameOver == true)
        {
            Destroy(gameObject);
        }
        

        if (homing && target != null)
        {
            HomingMovement();
        }
        else
        {
            ProjectileMovement();
        }
    }

    private void ProjectileMovement()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        ProjectileLimit();
    }

    private void HomingMovement()
    {
        Vector3 moveDirection = (target.transform.position - transform.position).normalized;
        transform.position += moveDirection * projectileSpeed * Time.deltaTime;
        transform.LookAt(target);
    }

    private void ProjectileLimit()
    {
        if (transform.position.z < zMin || transform.position.z > zMax)
        {
            Destroy(gameObject);
        }
    }

    public void Fire(Transform newTarget)
    {
        target = newTarget;
        homing = true;
        Destroy(gameObject, aliveTimer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnProjectileExplosion(transform.position);
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayHitSFX();
            Destroy(gameObject);
            collision.gameObject.GetComponent<EnemyManager>().enemyMaxHealth -= projectileDamage;
            if (collision.gameObject.CompareTag("Boss"))
            {
                GameObject.Find("Canvas").GetComponent<UIManager>().SetBossHealthBar(collision.gameObject.GetComponent<EnemyManager>().enemyMaxHealth);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnProjectileExplosion(transform.position);
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayHitSFX();
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayTakingDamageSFX();
            Destroy(gameObject);
            collision.gameObject.GetComponent<PlayerManager>().playerMaxHealth -= projectileDamage;
            GameObject.Find("Canvas").GetComponent<UIManager>().HealthUpdate(collision.gameObject.GetComponent<PlayerManager>().playerMaxHealth);
        }
    }
}
