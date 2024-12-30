using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    [Header("References")] // 인스펙터에서 관리할 참조 변수들
    public LineRenderer leftLine; // 왼쪽 라인 렌더러
    public LineRenderer rightLine; // 오른쪽 라인 렌더러
    public Transform leftAnchor; // 왼쪽 줄의 고정 위치
    public Transform rightAnchor; // 오른쪽 줄의 고정 위치
    public Transform center; // 새총의 중심 위치

    [Header("Properties")] // 인스펙터에서 조정할 수 있는 속성들
    public float lineWidth = 0.1f; // 줄의 두께
    public Color normalColor = Color.white; // 줄이 평상시의 색상
    public Color stretchedColor = Color.red; // 줄이 당겨졌을 때의 색상

    private Bird currentBird; // 현재 새총에 연결된 새

    // 초기화 함수
    void Start()
    {
        InitializeLines(); // 줄 렌더러를 초기화
    }

    // 줄 렌더러 초기 설정 함수
    void InitializeLines()
    {
        // 줄의 두께 설정
        leftLine.startWidth = lineWidth;
        leftLine.endWidth = lineWidth;
        rightLine.startWidth = lineWidth;
        rightLine.endWidth = lineWidth;

        // 줄의 위치를 중심 위치로 초기화
        UpdateLines(center.position);
    }

    // 매 프레임마다 호출되는 함수
    void Update()
    {
        if (currentBird != null) // 새가 연결되어 있을 경우
        {
            UpdateLines(currentBird.transform.position); // 줄의 끝점을 새의 위치로 업데이트
            UpdateLineColors(); // 줄의 색상을 업데이트
        }
    }

    // 줄의 위치를 업데이트하는 함수
    void UpdateLines(Vector3 birdPosition)
    {
        // 왼쪽 줄의 시작점과 끝점 설정
        leftLine.SetPosition(0, leftAnchor.position);
        leftLine.SetPosition(1, birdPosition);

        // 오른쪽 줄의 시작점과 끝점 설정
        rightLine.SetPosition(0, rightAnchor.position);
        rightLine.SetPosition(1, birdPosition);
    }

    // 줄의 색상을 업데이트하는 함수
    void UpdateLineColors()
    {
        // 새와 중심점 사이의 거리 계산
        float stretchDistance = Vector3.Distance(center.position, currentBird.transform.position);

        // 최대 당길 수 있는 거리로 정규화
        float normalizedDistance = stretchDistance / currentBird.maxDragDistance;

        // 현재 색상을 평상시 색상과 당긴 색상 사이에서 계산
        Color currentColor = Color.Lerp(normalColor, stretchedColor, normalizedDistance);

        // 줄 색상 설정
        leftLine.startColor = currentColor;
        leftLine.endColor = currentColor;
        rightLine.startColor = currentColor;
        rightLine.endColor = currentColor;
    }

    // 새를 새총에 연결하는 함수
    public void AttachBird(Bird bird)
    {
        currentBird = bird; // 새를 현재 새 변수에 저장
        UpdateLines(bird.transform.position); // 새의 위치로 줄 업데이트
    }

    // 새를 새총에서 분리하는 함수
    public void DetachBird()
    {
        currentBird = null; // 새 변수 초기화
        UpdateLines(center.position); // 줄을 중심 위치로 리셋
    }
}
