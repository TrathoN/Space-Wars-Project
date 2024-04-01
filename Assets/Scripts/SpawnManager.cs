using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> enemies;
    public List<GameObject> powerUps;
    public bool isGameOver = false;
    public bool isPlayerWon = false;
    public bool isNuked = false;
    public GameObject projectileExplosion;
    public GameObject shipExplosion;
    public GameObject bossExplosion;
    private Vector3 enemySpawnInterval;
    private int waveDifficulty;
    private List<int> possibleLocations;
    private float spawnDelay = 0.5f;
    private Coroutine spawnCoroutine;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip fireSFX;
    [SerializeField] private AudioClip takingDamageSFX;
    [SerializeField] private AudioClip explodeSFX;
    [SerializeField] private AudioClip powerUpSFX;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(SpawnPlayer());
        FillList();
    }

    // Update is called once per frame
    void Update()
    {
        if (isNuked)
        {
            isNuked = false;
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
            }
        }

        if ((GameObject.FindGameObjectsWithTag("Enemy").Length == 0 && GameObject.FindGameObjectsWithTag("Boss").Length == 0) && !isGameOver && !isPlayerWon && GameObject.FindGameObjectsWithTag("Player").Length != 0 && waveDifficulty < 11)
        {
            GameObject.Find("Canvas").GetComponent<UIManager>().SetProgressBar(waveDifficulty);
            waveDifficulty++;
            if (waveDifficulty < 11)
            {              
                spawnCoroutine = StartCoroutine(SpawnEnemy());
            }
            else if (waveDifficulty == 11)
            {
                Instantiate(enemies[2], enemies[2].transform.position, enemies[2].transform.rotation);
                GameObject.Find("Canvas").GetComponent<UIManager>().bossBarField.SetActive(true);
            }
            
        }

        if (isGameOver)
        {
            GameObject.Find("Canvas").GetComponent<UIManager>().GameOver();
        }

        if (isPlayerWon)
        {
            GameObject.Find("Canvas").GetComponent<UIManager>().Victory();
        }
    }

    public IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(2);
        Instantiate(player, player.transform.position, player.transform.rotation);
    }

    public void SpawnProjectileExplosion(Vector3 position)
    {
        Instantiate(projectileExplosion, position, transform.rotation);
        
    }

    public void PlayHitSFX()
    {
        audioSource.PlayOneShot(hitSFX);
    }

    public void PlayTakingDamageSFX()
    {
        audioSource.PlayOneShot(takingDamageSFX);
    }

    public void PlayExplodeSFX()
    {
        audioSource.PlayOneShot(explodeSFX);
    }

    public void PlayFireSFX()
    {
        audioSource.PlayOneShot(fireSFX);
    }

    public void PlayPowerUpSFX()
    {
        audioSource.PlayOneShot(powerUpSFX);
    }

    public void SpawnExplosion(Vector3 position, string tag)
    {
        if(tag == "Boss")
        {
            Instantiate(bossExplosion, position, transform.rotation);
        }
        else
        {
            Instantiate(shipExplosion, position, transform.rotation);
        }
        
    }

    public void SpawnPowerUps(Vector3 position)
    {
        Instantiate(powerUps[Random.Range(0,powerUps.Count)], position, powerUps[0].transform.rotation);
    }

    public void EndingMenu()
    {
        StartCoroutine(EndingDelay());
    }

    private IEnumerator EndingDelay()
    {
        yield return new WaitForSeconds(3);
        if(GameObject.FindGameObjectWithTag("Player") == null)
        {
            isGameOver = true;
        }
        else
        {
            isPlayerWon = true;
        }

    }

    private IEnumerator SpawnEnemy()
    {
        ShuffleList(possibleLocations);
        for (int i = 0; i < waveDifficulty; i++)
        {
            enemySpawnInterval = new Vector3(possibleLocations[i], enemies[0].transform.position.y, enemies[0].transform.position.z);
            if (waveDifficulty > 3 && Random.Range(0, 10) < 1)
            {
                Instantiate(enemies[1], enemySpawnInterval, enemies[1].transform.rotation);
            }
            else
            {
                Instantiate(enemies[0], enemySpawnInterval, enemies[0].transform.rotation);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void FillList()
    {
        possibleLocations = new List<int>();
        for (int i = -10; i <= 10; i = i + 2)
        {
            possibleLocations.Add(i);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
