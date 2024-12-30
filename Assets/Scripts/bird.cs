using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [Header("Bird Properties")] // 새의 속성 관련 변수들
    public float maxDragDistance = 2f; // 최대 드래그 거리
    public float launchPower = 500f; // 발사 힘
    public float maxLaunchSpeed = 30f; // 발사 속도의 최대값

    [Header("Components")] // 컴포넌트 관련 변수들
    private Rigidbody2D rb; // 새의 Rigidbody2D
    private SpringJoint2D sj; // 새총과 연결을 위한 SpringJoint2D
    private TrailRenderer tr; // 발사 후 궤적을 보여주는 TrailRenderer

    [Header("State")] // 새의 상태를 나타내는 변수들
    private bool isDragging = false; // 드래그 중인지 확인
    private Vector2 initialPosition; // 초기 위치 저장
    private bool isLaunched = false; // 발사 여부 확인

    // 초기화
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 컴포넌트 가져오기
        sj = GetComponent<SpringJoint2D>(); // SpringJoint2D 컴포넌트 가져오기
        tr = GetComponent<TrailRenderer>(); // TrailRenderer 컴포넌트 가져오기
        initialPosition = rb.position; // 새의 초기 위치 저장

        // TrailRenderer 비활성화
        if (tr != null) tr.enabled = false;
    }

    // 매 프레임 호출되는 함수
    void Update()
    {
        if (isLaunched) return; // 발사된 경우 업데이트 중단

        if (isDragging)
        {
            DragBird(); // 드래그 중이면 새의 위치 업데이트
        }
    }

    // 새를 드래그하는 함수
    void DragBird()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 월드 좌표로 변환
        Vector2 desiredPos = mousePos;

        // 드래그 거리를 최대 드래그 거리로 제한
        float distance = Vector2.Distance(desiredPos, initialPosition);
        if (distance > maxDragDistance)
        {
            Vector2 direction = (desiredPos - initialPosition).normalized; // 방향 계산
            desiredPos = initialPosition + direction * maxDragDistance; // 제한된 위치 계산
        }

        transform.position = desiredPos; // 새의 위치를 업데이트
    }

    // 마우스 클릭 시 호출
    void OnMouseDown()
    {
        if (isLaunched) return; // 발사된 경우 동작하지 않음
        isDragging = true; // 드래그 시작
        rb.isKinematic = true; // 물리적 상호작용 비활성화
    }

    // 마우스 클릭 해제 시 호출
    void OnMouseUp()
    {
        if (isLaunched) return; // 발사된 경우 동작하지 않음
        isDragging = false; // 드래그 종료
        rb.isKinematic = false; // 물리적 상호작용 활성화

        LaunchBird(); // 새 발사
    }

    // 새를 발사하는 함수
    void LaunchBird()
    {
        isLaunched = true; // 발사 상태로 변경

        // 발사 힘 계산
        Vector2 force = (initialPosition - rb.position) * launchPower;

        // 발사 속도를 최대값으로 제한
        if (force.magnitude > maxLaunchSpeed)
        {
            force = force.normalized * maxLaunchSpeed;
        }

        rb.AddForce(force, ForceMode2D.Impulse); // 힘을 새에 적용

        // TrailRenderer 활성화
        if (tr != null) tr.enabled = true;

        // SpringJoint 제거 (새총과 연결 해제)
        Destroy(sj, 0.1f);

        // 일정 시간 후 새 제거
        StartCoroutine(DestroyAfterDelay());
    }

    // 일정 시간 후 새를 제거하는 코루틴
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(5f); // 5초 대기
        if (gameObject != null)
        {
            Destroy(gameObject); // 새 제거
        }
    }
}
