using System.Collections;
using System.Collections.Generic;
using TMPro; // TextMeshPro를 사용하기 위한 네임스페이스
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴 구현
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")] // 게임 설정 관련 변수들
    public int maxBirds = 3; // 최대 사용 가능한 새의 수
    public float resetDelay = 3f; // 레벨 종료 후 대기 시간

    [Header("Prefabs")] // 프리팹 관련 변수들
    public GameObject birdPrefab; // 새 프리팹
    public Transform spawnPoint; // 새가 생성되는 위치

    [Header("UI References")] // UI 관련 변수들
    public TextMeshPro birdsLeftText; // 남은 새 수를 표시하는 텍스트
    public GameObject gameOverPanel; // 게임 오버 패널
    public GameObject levelCompletePanel; // 레벨 완료 패널

    // 게임 상태를 저장하는 변수들
    private int birdsLeft; // 남은 새의 수
    private int obstaclesLeft; // 남은 장애물 수
    private bool isGameActive = false; // 게임 활성화 상태

    // 게임 시작 시 실행되는 함수
    void Awake()
    {
        // 싱글톤 패턴을 통해 GameManager 인스턴스 생성
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 중복 인스턴스 제거
        }
    }

    // 게임 초기화
    void Start()
    {
        InitializeGame(); // 게임 초기 설정
    }

    void InitializeGame()
    {
        birdsLeft = maxBirds; // 남은 새 수를 최대값으로 초기화
        obstaclesLeft = GameObject.FindGameObjectsWithTag("Obstacle").Length; // 장애물 개수를 태그로 검색하여 초기화
        isGameActive = true; // 게임 활성화 상태로 설정

        UpdateUI(); // UI 업데이트
        SpawnNewBird(); // 첫 번째 새 생성
    }

    // 장애물이 파괴되었을 때 호출되는 함수
    public void OnObstacleDestroyed()
    {
        obstaclesLeft--; // 남은 장애물 수 감소
        CheckWinCondition(); // 승리 조건 확인
    }

    // 승리 또는 패배 조건 확인
    void CheckWinCondition()
    {
        // 장애물이 모두 제거되었을 때
        if (obstaclesLeft <= 0)
        {
            StartCoroutine(LevelComplete()); // 레벨 완료 코루틴 실행
        }
        // 새가 모두 소진되고, 남아 있는 새가 없을 때
        else if (birdsLeft <= 0 && GameObject.FindGameObjectWithTag("Bird") == null)
        {
            StartCoroutine(GameOver()); // 게임 오버 코루틴 실행
        }
    }

    // 레벨 완료 처리
    IEnumerator LevelComplete()
    {
        yield return new WaitForSeconds(resetDelay); // 대기 시간 후
        levelCompletePanel.SetActive(true); // 레벨 완료 패널 활성화
        isGameActive = false; // 게임 비활성화
    }

    // 게임 오버 처리
    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(resetDelay); // 대기 시간 후
        gameOverPanel.SetActive(true); // 게임 오버 패널 활성화
        isGameActive = false; // 게임 비활성화
    }

    // 새 생성
    void SpawnNewBird()
    {
        // 남은 새가 있고, 게임이 활성화된 상태일 경우
        if (birdsLeft > 0 && isGameActive)
        {
            GameObject newBird = Instantiate(birdPrefab, spawnPoint.position, Quaternion.identity); // 새 프리팹 생성
            birdsLeft--; // 남은 새 수 감소
            UpdateUI(); // UI 업데이트
        }
    }

    // UI 업데이트
    void UpdateUI()
    {
        if (birdsLeftText != null)
        {
            birdsLeftText.text = $"Birds Left: {birdsLeft}"; // 남은 새 수 표시
        }
    }

    // // 레벨 재시작
    // public void RestartLevel()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 다시 로드
    // }
    //
    // // 다음 레벨로 이동
    // public void NextLevel()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); // 다음 씬 로드
    // }
}
