using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyProjectile;
    public int enemyMaxHealth;
    [SerializeField] private float enemySpeed;
    [SerializeField] private float enemyAttackPosition;
    [SerializeField] private float enemyDistance;
    [SerializeField] private float enemyStepSize;
    [SerializeField] private float enemyStepInterval;
    [SerializeField] private float enemyAttackRate;
    [SerializeField] private int enemyPoint;
    [SerializeField] private int powerUpLuck;
    [SerializeField] private bool isElite;
    private float timer;
    private bool isReadytoMove = true;

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("SpawnManager").GetComponent<SpawnManager>().isGameOver)
        {
            Destroy(gameObject);
        }
        if (enemyMaxHealth <= 0)
        {
            EnemyKilledbyPlayer();
        }
        
        if(transform.position.z == enemyDistance && isReadytoMove && gameObject.tag != "Boss")
        {
            isReadytoMove = false;
            StartCoroutine(EnemyProgress());
        }

        EnemyMovement();

        if(transform.position.z <= enemyAttackPosition)
        {
            AttackProjectile();
        }

        if(transform.position.z < -3)
        {
            if(GameObject.FindGameObjectWithTag("Player") != null)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().playerMaxHealth -= 30;
                GameObject.Find("Canvas").GetComponent<UIManager>().HealthUpdate(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().playerMaxHealth);
            }     
            Destroy(gameObject);
        }
    }

    IEnumerator EnemyProgress()
    {
        yield return new WaitForSeconds(enemyStepInterval);
        enemyDistance -= enemyStepSize;
        isReadytoMove = true;
    }

    public void EnemyKilledbyPlayer()
    {
        GameObject.Find("Canvas").GetComponent<UIManager>().ScoreSum(enemyPoint);
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnExplosion(transform.position, gameObject.tag);
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayExplodeSFX();
        Destroy(gameObject);
        if (Random.Range(0, 100) <= powerUpLuck)
        {
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnPowerUps(transform.position);
        }
        if(gameObject.tag == "Boss")
        {
            GameObject.Find("Canvas").GetComponent<UIManager>().bossBarField.SetActive(false);
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().EndingMenu();
        }
    }

    private void EnemyMovement()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * enemySpeed);

        EnemyDistance();
    }

    private void EnemyDistance()
    {
        if (transform.position.z < enemyDistance && transform.position.z > 2)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, enemyDistance);
        }
    }

    private void AttackProjectile()
    {
        if(gameObject.CompareTag("Boss"))
        {
            if (timer + enemyAttackRate < Time.time)
            {
                float random = Random.Range(-4, 5);
                float randLocation = random / 2;

                Instantiate(enemyProjectile, (transform.position - enemyProjectile.transform.position) + new Vector3(randLocation, 0, 0), RotateProjectile(randLocation));
                GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayFireSFX();
                timer = Time.time;
            }
        }
        else
        {
            if (isElite)
            {
                if (timer + enemyAttackRate < Time.time)
                {
                    Instantiate(enemyProjectile, (transform.position - enemyProjectile.transform.position) + new Vector3(0.5f, 0, 0), enemyProjectile.transform.rotation);
                    Instantiate(enemyProjectile, (transform.position - enemyProjectile.transform.position) + new Vector3(-0.5f, 0, 0), enemyProjectile.transform.rotation);
                    GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayFireSFX();
                    timer = Time.time;
                }
            }
            else
            {
                if (timer + enemyAttackRate < Time.time)
                {
                    Instantiate(enemyProjectile, transform.position - enemyProjectile.transform.position, enemyProjectile.transform.rotation);
                    GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayFireSFX();
                    timer = Time.time;
                }
            }
        }    
    }

    private Quaternion RotateProjectile(float randLocation)
    {
        Vector3 rotationVector = enemyProjectile.transform.rotation.eulerAngles;
        rotationVector = rotationVector - new Vector3(0, randLocation * 10.5f, 0);
        return Quaternion.Euler(rotationVector);
    }
}
