using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[System.Serializable]
public class Challenge
{
    public bool oneRound = true;
    public int coins;
    public int score;
    public int cops;
}

[System.Serializable]
public class Save
{
    public int currentChallenge;
    public Challenge actualChallenge;
    public int coins;
}

public class GameManager : MonoBehaviour
{
    public GameObject[] hearts;
    public GameObject[] Smokes;
    public GameObject prefabExplosion;
    public GameObject prefabShieldHit;
    public GameObject bonus;
    public GameObject ChallengePopUp;
    public Transform bonusSpawn;
    public GameObject StartPlayBtn;
    public GameObject controlMenu;
    public Text challengeTxt;
    public Text TitleChallengeTxt;
    public Text scoreText;
    public Text coinsText;
    public GameObject RetryMenu;
    public ComboZone comboZone;
    public Challenge[] challenges;

    private int currentChallenge = 0;
    private int score = 0;
    private int coins = 0;
    private int playerLife = 3;
    private const int maxLife = 3;
    private float nextTime = 0.0f;
    private bool isInvincible = false;
    private bool isDead = false;
    private float interval = 1.0f;
    private bool startMenu = true;
    private bool shield = false;

    private Challenge actualChallenge;
    private GameObject canvas;
    private GameObject player;
    private GameObject mainCamera;
    private EnemySpawner enemySpawner;
    private AudioSource[] audioSources;

    public static Save savedGame = new Save();
             
    //it's static so we can call it from anywhere
    public void Save() {
        Save save = new Save();
        save.coins = coins;
        save.currentChallenge = currentChallenge;
        if (!challenges[currentChallenge].oneRound)
            save.actualChallenge = actualChallenge;
        else
            save.actualChallenge = new Challenge();
        savedGame = save;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGame.gd");
        bf.Serialize(file, GameManager.savedGame);
        file.Close();
    }   
     
    public void Load() {
        if(File.Exists(Application.persistentDataPath + "/savedGame.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGame.gd", FileMode.Open);
            savedGame = (Save)bf.Deserialize(file);
            coins = savedGame.coins;
            coinsText.text = coins.ToString();
            actualChallenge = savedGame.actualChallenge;
            currentChallenge = savedGame.currentChallenge;
            file.Close();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSources = GetComponents<AudioSource>();
        RetryMenu.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        enemySpawner = GetComponent<EnemySpawner>();
        nextTime = interval;
        scoreText.text = "0";
        actualChallenge = new Challenge();
        Load();
        Setchallenge();
    }

    private void Setchallenge()
    {
        if (currentChallenge >= challenges.Length)
            currentChallenge = 0;

        TitleChallengeTxt.text = "CHALLENGE : " + (currentChallenge + 1);

        challengeTxt.text = "GET ";
        if (challenges[currentChallenge].coins > 0)
            challengeTxt.text += challenges[currentChallenge].coins + " COINS ";

        if (challenges[currentChallenge].score > 0)
            challengeTxt.text += challenges[currentChallenge].score + " SCORE ";

        if (challenges[currentChallenge].cops > 0)
            challengeTxt.text += challenges[currentChallenge].cops + " COPS DESTROYED ";

        if (challenges[currentChallenge].oneRound)
        {
            challengeTxt.text += " IN A GAME";
            actualChallenge = new Challenge();
        }
    }

    private void EndGame()
    {
        if (!startMenu)
        {
            audioSources[1].Stop();
            startMenu = true;
            StartPlayBtn.SetActive(true);
            controlMenu.SetActive(true);
        }
    }

    public void ChangeColor()
    {
        PlayerBtnSound();
        MeshRenderer playerMesh = player.GetComponentInChildren<MeshRenderer>();
        playerMesh.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }

    public void AddCopDestroyActualChallenge()
    {
        actualChallenge.cops++;
    }

    public bool SetShield()
    {
        //Return if shield was changed or not
        bool ret = !shield;
        shield = true;
        return ret;
    }

    public void StartGame()
    {
        if (startMenu && Time.time > 1f)
        {
            audioSources[1].Play();
            startMenu = false;
            StartPlayBtn.SetActive(false);
            controlMenu.SetActive(false);
        }
    }

    public bool IsShield()
    {
        return shield;
    }

    void CheckChallenge()
    {
        if (challenges[currentChallenge].coins <= actualChallenge.coins
         && challenges[currentChallenge].score <= actualChallenge.score
         && challenges[currentChallenge].cops <= actualChallenge.cops)
        {
            currentChallenge++;
            ChallengePopUp.GetComponentInChildren<Text>().text = challengeTxt.text;
            ChallengePopUp.SetActive(false);
            ChallengePopUp.SetActive(true);
            Setchallenge();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextTime) 
        {
            IncreaseScore();
            CheckChallenge();
            nextTime += interval;
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public int IncreaseCoins(int nb = 1)
    {
        coins += nb;
        coinsText.text = coins.ToString();
        actualChallenge.coins += nb;
        return coins;
    }

    public int GetCoins()
    {
        return coins;
    }

    public bool GetStartMenu()
    {
        return startMenu;
    }

    public int IncreaseScore(int nb = 1)
    {
        if (startMenu || isDead)
            return score;

        if (nb > 1)
        {
            GameObject bonusText = Instantiate(bonus, bonusSpawn);
            bonusText.transform.SetParent(canvas.transform);
            bonusText.GetComponent<Text>().text = "+" + nb;
            Destroy(bonusText, 1f);
        }

        score += nb;
        actualChallenge.score += nb;
        scoreText.text = score.ToString();
        return score;
    }

    public void PlayerBtnSound()
    {
        audioSources[4].Play();
    }

    public int GetScore()
    {
        return score;
    }

    public void PlayExplosion()
    {
        audioSources[2].Play();
    }

    public int DecreaseLife(int nb = 1)
    {
        if (isInvincible)
            return playerLife;

        mainCamera.GetComponent<Animation>().Play();
        if (shield)
        {
            Instantiate(prefabShieldHit, player.transform.position, Quaternion.identity);
            shield = false;
            StartCoroutine("InvincibleTimer");
            return playerLife;
        }

        audioSources[3].Play();
        player.GetComponent<DriveCar>().RemoveSkill(nb);
        playerLife -= nb;
        
        if (playerLife > 0)
        {
            StartCoroutine("InvincibleTimer");
        }
        else
        {
            isDead = true;
            player.SetActive(false);
            player.transform.rotation = Quaternion.identity;
            player.GetComponent<DriveCar>().ResetSkills();
            comboZone.ResetCombo();
            GameObject expl = Instantiate(prefabExplosion, player.transform.position, Quaternion.identity);
            PlayExplosion();
            Destroy(expl, 3f);
            RetryMenu.SetActive(true);
            Save();
        }

        UpdateHearts();
        return playerLife;
    }

    IEnumerator InvincibleTimer()
    {
        isInvincible = true;
        MeshRenderer playerMesh = player.GetComponentInChildren<MeshRenderer>();
        playerMesh.material.color = new Color(playerMesh.material.color.r,playerMesh.material.color.g, playerMesh.material.color.b, 0.5f);
        yield return new WaitForSeconds(2);
        playerMesh.material.color = new Color(playerMesh.material.color.r,playerMesh.material.color.g, playerMesh.material.color.b, 1f);
        isInvincible = false;
    }

    public int IncreaseLife(int nb = 1)
    {
        playerLife += nb;

        if (playerLife > maxLife)
            playerLife = maxLife;

        UpdateHearts();
        return playerLife;
    }

    public void Reset()
    {
        PlayerBtnSound();
        RetryMenu.SetActive(false);
        player.SetActive(true);
        playerLife = maxLife;
        score = 0;
        scoreText.text = "0";
        enemySpawner.DestroyEnemies();
        GetComponent<ChunkLoader>().ResetChunks();
        UpdateHearts();
        isDead = false;
        EndGame();

        if (challenges[currentChallenge].oneRound)
            Setchallenge();
    }

    public void SecondChance()
    {
        PlayerBtnSound();
        RetryMenu.SetActive(false);
        StartCoroutine("InvincibleTimer");
        player.SetActive(true);
        playerLife = maxLife;
        UpdateHearts();
        isDead = false;
    }

    public int GetLife()
    {
        return playerLife;
    }

    void UpdateHearts()
    {
        if (playerLife == 0)
        {
            hearts[0].SetActive(false);
            hearts[1].SetActive(false);
            hearts[2].SetActive(false);
            Smokes[0].SetActive(false);
            Smokes[1].SetActive(false);

        }
        else if (playerLife == 1)
        {
            hearts[0].SetActive(true);
            hearts[1].SetActive(false);
            hearts[2].SetActive(false);
            Smokes[0].SetActive(false);
            Smokes[1].SetActive(true);
        }
        else if (playerLife == 2)
        {
            hearts[0].SetActive(true);
            hearts[1].SetActive(true);
            hearts[2].SetActive(false);
            Smokes[0].SetActive(true);
            Smokes[1].SetActive(false);
        }
        else
        {
            hearts[0].SetActive(true);
            hearts[1].SetActive(true);
            hearts[2].SetActive(true);
            Smokes[0].SetActive(false);
            Smokes[1].SetActive(false);
        }
    }
}
