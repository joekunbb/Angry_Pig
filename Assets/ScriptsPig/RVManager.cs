using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using TMPro;
using Unity.VisualScripting; // TextMeshPro 사용을 위한 네임스페이스
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class RVManager : MonoBehaviour
{
    public Pig pig;
    public GameObject PigPrefab;
    public Obstacle obstacle;
    public Transform spawnPoint;
    
    private sShot sshot;

    public int score;
    public bool isOver;

    public GameObject startGroup;
    public GameObject endGroup;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxScoreText;
    public TextMeshProUGUI subScoreText;
    




    void Awake()
    {
        Application.targetFrameRate = 60;


        // if (!PlayerPrefs.HasKey("MaxScore"))
        // {
        //     PlayerPrefs.SetInt("MaxScore", 0);
        // }
        maxScoreText.text = PlayerPrefs.GetInt("MaxScore", 0).ToString();
        
    }
    
    public void GameStart()
    {
        //Obstacle BP = instantObstacle.GetComponent<Obstacle>();
        //BP.manager = This;
        obstacle = FindObjectOfType<Obstacle>();
        obstacle.manager = this;
        
        startGroup.SetActive(false);
        
        Invoke("NextPig", 1.5f);
        
    }

    Pig GetPig()
    {
        GameObject instant = Instantiate(PigPrefab, spawnPoint);
        Pig instantPig= instant.GetComponent<Pig>();
        return instantPig;
        
    }

    void NextPig()
    {
        Pig newPig = GetPig();
        pig = newPig;

        StartCoroutine(WaitNext());
    }

    IEnumerator WaitNext()
    {
        while (pig != null)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(4f);

        Search();


    }
    
    

    public void Search()
    {
        GameObject[] birds = GameObject.FindGameObjectsWithTag("Bird");


        if (birds.Length <= 0)
        {
            GameOver();
        }
        else 
            NextPig();
    }
    
    public void GameOver()
    {
        if (isOver)
        {
            return;
        }
        
        isOver = true;
        Debug.Log("Game Over!!");
        StartCoroutine("GameOverCo");
            
    }

    IEnumerator GameOverCo()
    {
        yield return new WaitForSeconds(1f);
        
        int maxScore = Mathf.Max(score, PlayerPrefs.GetInt("MaxScore", 0));
        PlayerPrefs.SetInt("MaxScore", maxScore);

        subScoreText.text = "SCORE : " + scoreText.text;
        endGroup.SetActive(true);
        
        
        
    }

    public void Reset()
    {
        StartCoroutine(ResetCo());
    }

    IEnumerator ResetCo()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
    
    
    public void TouchDown()
    {
        if (pig == null)
            return;
        pig.Drag();
    }

    public void TouchUp()
    {
        if (pig == null)
            return;
        pig.Drop();
        pig = null;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Application.Quit();
        }
    }
    void LateUpdate()
    {
        scoreText.text = score.ToString();
    }
    
}
