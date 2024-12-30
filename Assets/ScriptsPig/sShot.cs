using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sShot : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;

    public float pigOffset;
    
    
    
    private Pig currentPig;
    [SerializeField]
    private RVManager rvManager;
    
    
    void Awake()
    {
        // RVManager 초기화
        // rvManager = FindObjectOfType<RVManager>();
        // if (rvManager == null || rvManager.spawnPoint == null)
        // {
        //     Debug.LogError("RVManager 또는 spawnPoint가 설정되지 않았습니다!");
        // }
    }
    
    void Start()
    {
        if (rvManager == null)
        {
            Debug.LogError("RVManager가 연결되지 않았습니다!");
        }
        
        
        lineRenderers[0].positionCount = 2; // 두 점을 연결
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position); // 시작점 설정
        lineRenderers[1].SetPosition(0, stripPositions[1].position);
        
        SetStrips(rvManager.spawnPoint.position);
    }

    
    void Update()
    {
        //Vector3 fixPosition = rvManager.pig.transform.position + new Vector3(-0.4f, 0, 0);
        
        if (rvManager.pig != null) // 새가 연결되어 있을 경우
        {
            SetStrips(rvManager.pig.transform.position);
            //SetStrips(rvManager.pig.transform.position); // 줄의 끝점을 새의 위치로 업데이트
        }
        else
        {
            SetStrips(rvManager.spawnPoint.position); // currentPig가 없으면 spawnPoint로 초기화
        }
    }
    
    
    // void ResetStrips()
    // {
    //     
    // }

    void SetStrips(Vector3 position)
    {
        // Pig 객체의 로컬 좌표 기준으로 왼쪽 -0.3 위치 계산
        Vector3 localOffset = new Vector3(-0.4f, 0, 0);

        if (rvManager.pig != null)
        {
            // Pig 객체의 로컬 좌표에서 글로벌 좌표로 변환
            Vector3 adjustedPosition = rvManager.pig.transform.TransformPoint(localOffset);
        
            // 스트랩 위치 업데이트
            lineRenderers[0].SetPosition(1, adjustedPosition);
            lineRenderers[1].SetPosition(1, adjustedPosition);
        }
        else
        {
            // Pig가 없으면 기본 위치 유지
            lineRenderers[0].SetPosition(1, position);
            lineRenderers[1].SetPosition(1, position);
        }
    }

    
    
}
