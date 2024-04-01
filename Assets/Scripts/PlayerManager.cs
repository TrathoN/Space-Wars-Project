using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerProjectile;
    public int playerMaxHealth;
    public List<GameObject> indicators;

    private float horizontalInput;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    [SerializeField] private float playerStartSpeed;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float playerAttackRate;
    [SerializeField] private int playerHealingRate;
    
    private float timer;

    private Coroutine powerUpCoroutine;
    private int playerInitialDamage;
    private int playerPoweredDamage;
    private float playerInitialSpeed;
    private float playerPoweredSpeed;
    private float playerInitialAttackRate;
    private float playerPoweredAttackRate;
    private PowerUpType powerUpType;
    private GameObject tmpProjectile;

    private void Start()
    {
        playerProjectile.GetComponent<Projectile>().projectileDamage = 50;
        playerInitialDamage = playerProjectile.GetComponent<Projectile>().projectileDamage;
        playerInitialSpeed = playerSpeed;
        playerInitialAttackRate = playerAttackRate;

        playerPoweredDamage = playerInitialDamage + 50;
        playerPoweredSpeed = playerInitialSpeed + 10;
        playerPoweredAttackRate = playerInitialAttackRate / 2;

        indicators[0].SetActive(false);
        indicators[1].SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMaxHealth <= 0)
        {
            PlayerDestroyed();
        }

        if (transform.position.z != 0)
        {
            StartingPlace();
        }

        MovePlayer();

        if (Input.GetKey(KeyCode.Space))
        {
            if (powerUpType == PowerUpType.HomingProjectile && GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
            {
                HomingProjectile();
            }
            AttackProjectile();
        }

        if (powerUpType == PowerUpType.Damage)
        {
            DamagePowerUp(playerPoweredDamage);
        }
        else
        {
            DamagePowerUp(playerInitialDamage);
        }

        if (powerUpType == PowerUpType.AttackMovementSpeed)
        {
            AttackMovementPowerUp(playerPoweredAttackRate, playerPoweredSpeed);
        }
        else
        {
            AttackMovementPowerUp(playerInitialAttackRate, playerInitialSpeed);
        }

    }

    private void PlayerDestroyed()
    {
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().SpawnExplosion(transform.position, gameObject.tag);
        GameObject.Find("SpawnManager").GetComponent<SpawnManager>().EndingMenu();
        Destroy(gameObject);
    }

    private void MovePlayer()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * playerSpeed);

        SetPlayerLimit();
    }

    private void SetPlayerLimit()
    {
        if (transform.position.x < minX)
        {
            transform.position = new Vector3(minX, transform.position.y, transform.position.z);
        }

        if (transform.position.x > maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
    }

    private void StartingPlace()
    {
        if (transform.position.z < 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * playerStartSpeed);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }

    private void AttackProjectile()
    {
        if (timer + playerAttackRate < Time.time)
        {
            Instantiate(playerProjectile, transform.position - playerProjectile.transform.position, playerProjectile.transform.rotation);
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayFireSFX();
            timer = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayPowerUpSFX();
            if(collision.gameObject.GetComponent<PowerUpBehaviour>().powerUpType != PowerUpType.Health && collision.gameObject.GetComponent<PowerUpBehaviour>().powerUpType != PowerUpType.Nuke)
            {
                powerUpType = collision.gameObject.GetComponent<PowerUpBehaviour>().powerUpType;
            }

            Destroy(collision.gameObject);

            if (collision.gameObject.GetComponent<PowerUpBehaviour>().powerUpType == PowerUpType.Health)
            {
                Healing();
            }
            else if (collision.gameObject.GetComponent<PowerUpBehaviour>().powerUpType == PowerUpType.Nuke)
            {
                GameObject.Find("SpawnManager").GetComponent<SpawnManager>().isNuked = true;
                Nuke();
            }
            else 
            {
                if (powerUpCoroutine != null)
                {
                    StopCoroutine(powerUpCoroutine);
                }
                powerUpCoroutine = StartCoroutine(PowerupCountdownRoutine());
                indicators[0].GetComponent<IndicatorRotator>().SetIndicatorColor(powerUpType);
                indicators[1].GetComponent<IndicatorRotator>().SetIndicatorColor(powerUpType);
                indicators[0].SetActive(true);
                indicators[1].SetActive(true);
            }
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(7);
        indicators[0].SetActive(false);
        indicators[1].SetActive(false);
        powerUpType = PowerUpType.None;
    }

    private void Healing()
    {
        playerMaxHealth += playerHealingRate;
        GameObject.Find("Canvas").GetComponent<UIManager>().HealthUpdate(playerMaxHealth);
    }
    private void DamagePowerUp(int damage)
    {
        playerProjectile.GetComponent<Projectile>().projectileDamage = damage;
    }    

    private void AttackMovementPowerUp(float attackRate, float movementSpeed)
    {
        playerAttackRate = attackRate;
        playerSpeed = movementSpeed;
    }

    private void HomingProjectile()
    {
        if (timer + playerAttackRate < Time.time)
        {
            GameObject.Find("SpawnManager").GetComponent<SpawnManager>().PlayFireSFX();
            foreach (var enemy in FindObjectsOfType<EnemyManager>())
            {
                tmpProjectile = Instantiate(playerProjectile, transform.position + Vector3.up, Quaternion.identity);
                tmpProjectile.GetComponent<Projectile>().Fire(enemy.transform);
            }
            timer = Time.time;
        }

    }

    private void Nuke()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy").Length; i++)
            {
                GameObject.FindGameObjectsWithTag("Enemy")[i].GetComponent<EnemyManager>().EnemyKilledbyPlayer();                
            }
        }
    }


}
