using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSling : MonoBehaviour
{
    // LineRenderer 배열로 새총의 줄을 표현
    public LineRenderer[] lineRenderers;

    // 줄의 양 끝점 Transform 배열 (새총의 위치)
    public Transform[] stripPositions;

    // 새총 중심점 Transform
    public Transform center;

    // 줄이 아무 동작이 없을 때의 기본 위치 Transform
    public Transform idlePosition;

    // 현재 새 위치를 저장하는 변수
    public Vector3 currentPosition;

    // 새총 줄의 최대 길이
    public float maxLength;

    // 새총 줄의 y축 제한 하단값
    public float bottomBoundary;

    // 마우스 클릭 여부를 확인하는 변수
    private bool isMouseDown;

    // 발사할 새의 Prefab
    public GameObject birdPrefab;

    // 새와 줄 사이의 거리 (줄에서 새 위치를 조금 떨어트려 표현)
    public float birdPositionOffset;

    // 새의 Rigidbody2D와 Collider2D
    private Rigidbody2D bird;
    private Collider2D birdCollider;

    // 발사 힘의 크기
    public float force;

    // 게임 시작 시 초기화 작업
    void Start()
    {
        // LineRenderer 초기화
        lineRenderers[0].positionCount = 2; // 두 점을 연결
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position); // 시작점 설정
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        // 새 생성
        CreateBird();
    }

    // 새를 새총에 생성하는 함수
    void CreateBird()
    {
        // 새 Prefab에서 Rigidbody2D 가져오기
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false; // 충돌 비활성화
        
        bird.isKinematic = true; // 물리적 상호작용을 하지 않도록 설정
        
        // 새총 줄과 새의 충돌을 무시
        foreach (Transform stripPosition in stripPositions)
        {
            Collider2D slingshotCollider = stripPosition.GetComponent<Collider2D>();
            if (slingshotCollider)
            {
                Physics2D.IgnoreCollision(birdCollider, slingshotCollider);
            }
        }
        
        // 줄을 초기 위치로 리셋
        ResetStrips();
    }
 
    // 매 프레임 호출되는 함수
    void Update()
    {
        if (isMouseDown)
        {
            // 마우스 위치를 가져옴
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10; // 카메라의 z축 거리
            
            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            
            // 중심으로부터 최대 길이를 넘지 않도록 제한
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);

            // 하단 경계를 넘어가지 않도록 제한
            currentPosition = ClampBoundary(currentPosition);
            
            // 줄을 새 위치로 설정
            SetStrips(currentPosition);

            if (birdCollider)
            {
                birdCollider.enabled = true; // 새의 충돌 활성화
            }
        }
        else
        {
            ResetStrips(); // 마우스가 눌려 있지 않으면 줄을 리셋
        }
    }

    // 마우스 클릭 시작
    private void OnMouseDown()
    {
        isMouseDown = true;
    }

    // 마우스 클릭 해제
    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot(); // 새 발사
    }

    // 새를 발사하는 함수
    void Shoot()
    {
        bird.isKinematic = false; // 물리적 상호작용 활성화
        Vector3 birdForce = (currentPosition - center.position) * force * -1; // 발사 방향과 힘 계산
        bird.velocity = birdForce; // 새에 힘 적용
        
        // 새의 참조 제거
        bird = null;
        birdCollider = null;

        // 2초 후 새를 다시 생성
        Invoke("CreateBird", 2);
    }
    
    // 줄을 초기 위치로 리셋
    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    // 줄의 위치를 설정
    void SetStrips(Vector3 position)
    {
        // LineRenderer의 끝점을 현재 위치로 설정
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (bird)
        {
            // 새 위치와 방향 설정
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset; // 위치 오프셋 적용
            bird.transform.right = -dir.normalized; // 새의 방향 설정
        }
    }

    // 하단 경계 값을 제한하는 함수
    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000); // y축 하한 제한
        return vector;
    }
}
