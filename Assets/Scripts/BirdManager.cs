using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour
{
    [Header("Bird Settings")]
    public GameObject birdPrefab; // 새 프리팹
    public Transform slingAnchor; // 새총의 고정점
    public Transform idlePosition; // 새의 초기 대기 위치
    public LineRenderer[] lineRenderers; // 새총 고무줄 라인 렌더러 배열
    public float maxDragDistance = 2f; // 새를 드래그할 수 있는 최대 거리
    public float launchPower = 500f; // 발사 힘

    [Header("Path Settings")]
    public GameObject pathPointPrefab; // 경로 점 프리팹
    public int maxPathPoints = 20; // 최대 경로 점 개수
    public float pathPointInterval = 0.1f; // 경로 점 생성 간격 (초)

    private List<GameObject> pathPoints; // 생성된 경로 점 리스트
    private Rigidbody2D currentBirdRb; // 현재 새의 Rigidbody2D
    private bool isDragging = false; // 새가 드래그 중인지 여부

    void Start()
    {
        // 경로 점 리스트 초기화
        pathPoints = new List<GameObject>();

        // 라인 렌더러 초기화
        InitializeLineRenderers();

        // 새 생성
        SpawnBird();

        // 경로 점 초기화
        InitializePathPoints();
    }

    void Update()
    {
        // 드래그 중일 때만 새의 위치를 업데이트
        if (isDragging)
        {
            DragBird();
        }
    }

    void InitializeLineRenderers()
    {
        // 라인 렌더러 설정
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 2; // 라인 렌더러의 점 개수 (고정점 + 새 위치)
            lineRenderer.SetPosition(0, slingAnchor.position); // 고정점 설정
        }
    }

    void InitializePathPoints()
    {
        // 최대 경로 점 개수만큼 비활성화된 경로 점 생성
        for (int i = 0; i < maxPathPoints; i++)
        {
            var point = Instantiate(pathPointPrefab, idlePosition.position, Quaternion.identity, transform);
            point.SetActive(false); // 초기 상태에서는 비활성화
            pathPoints.Add(point);
        }
    }

    void SpawnBird()
    {
        // 새 프리팹 생성 및 Rigidbody2D 컴포넌트 가져오기
        GameObject bird = Instantiate(birdPrefab, idlePosition.position, Quaternion.identity);
        currentBirdRb = bird.GetComponent<Rigidbody2D>();
        currentBirdRb.isKinematic = true; // 드래그 중일 때 위치 고정

        // 라인 렌더러의 끝점 위치를 새의 초기 위치로 설정
        UpdateLineRenderers(bird.transform.position);
    }

    void DragBird()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 desiredPos = mousePos;

        // 최대 드래그 거리 제한
        float distance = Vector3.Distance(desiredPos, slingAnchor.position);
        if (distance > maxDragDistance)
        {
            Vector3 direction = (desiredPos - slingAnchor.position).normalized;
            desiredPos = slingAnchor.position + direction * maxDragDistance; // 최대 거리 내로 제한
        }

        // 새의 위치 업데이트
        currentBirdRb.position = desiredPos;

        // 라인 렌더러 끝점 위치 업데이트
        UpdateLineRenderers(desiredPos);
    }

    void OnMouseDown()
    {
        // 드래그 시작
        isDragging = true;
        currentBirdRb.isKinematic = true; // 물리 엔진 비활성화
    }

    void OnMouseUp()
    {
        // 드래그 중지
        isDragging = false;
        currentBirdRb.isKinematic = false; // 물리 엔진 활성화

        // 새 발사
        LaunchBird();
    }

    void LaunchBird()
    {
        // 발사 방향 및 힘 계산
        Vector3 launchDirection = (Vector2)slingAnchor.position - currentBirdRb.position;
        Vector3 launchForce = launchDirection * launchPower;

        // 힘을 새에 적용
        currentBirdRb.AddForce(launchForce, ForceMode2D.Impulse);

        // 경로 점 업데이트 시작
        StartCoroutine(UpdatePathPoints());

        // 라인 렌더러 비활성화
        DisableLineRenderers();
    }

    IEnumerator UpdatePathPoints()
    {
        // 경로 점을 주기적으로 업데이트
        for (int i = 0; i < maxPathPoints; i++)
        {
            if (currentBirdRb == null) yield break; // 새가 제거되면 중단

            pathPoints[i].transform.position = currentBirdRb.position; // 경로 점 위치 업데이트
            pathPoints[i].SetActive(true); // 경로 점 활성화

            yield return new WaitForSeconds(pathPointInterval); // 간격 대기
        }
    }

    void UpdateLineRenderers(Vector3 birdPosition)
    {
        // 라인 렌더러 끝점 위치를 새의 위치로 설정
        lineRenderers[0].SetPosition(1, birdPosition);
        lineRenderers[1].SetPosition(1, birdPosition);
    }

    void DisableLineRenderers()
    {
        // 모든 라인 렌더러 비활성화
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.enabled = false;
        }
    }
}
