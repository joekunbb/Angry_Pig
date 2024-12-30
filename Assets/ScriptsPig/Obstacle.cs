using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Obstacle Properties")]
    public float health = 100f; // 장애물의 초기 체력
    public float damageMultiplier = 1f; // 충격량에 따라 데미지를 조정하는 배율
    public float minimumDamageThreshold = 5f; // 데미지가 발생하기 위한 최소 충격량 임계값

    [Header("Effects")]
    public GameObject destroyEffect; // 파괴 시 생성할 파티클 효과
    public AudioClip hitSound; // 충돌 시 재생할 사운드
    public GameObject hitSounds; // 충돌 시 재생할 사운드
    public AudioClip destroySound; // 파괴 시 재생할 사운드
    public GameObject destroySounds; // 파괴 시 재생할 사운드
    //public ParticleSystem destroyParticles;

    private AudioSource audioSource; // 오디오 재생을 위한 AudioSource 컴포넌트
    
    
    public RVManager manager;

    void Start()
    {
        
       
        // AudioSource 컴포넌트를 가져오거나 없으면 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 속도를 기반으로 충격량 계산 시작
        float impactForce = collision.relativeVelocity.magnitude; // 충돌 속도 크기 계산
        float otherMass = 0f; // 충돌 상대의 질량 초기화

        // 충돌한 객체에 Rigidbody2D가 있는 경우 질량을 가져옴
        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb != null)
        {
            otherMass = otherRb.mass; // 상대 객체의 질량 저장
        }

        // 충격량 = 충돌 속도 * 상대 질량
        float impact = impactForce * otherMass;

        // 충격량이 최소 임계값보다 작으면 아무 일도 하지 않음
        if (impact < minimumDamageThreshold) return;

        // 최종 데미지 계산: 충격량 * 데미지 배율
        float damage = impact * damageMultiplier;
        TakeDamage(damage); // 계산된 데미지 적용

        // 충돌 사운드 재생
        PlayHitSound();
    }

    void TakeDamage(float damage)
    {
        // 장애물 체력 감소
        health -= damage;

        // 데미지를 입었을 때 시각적 효과 실행
        StartCoroutine(DamageEffect());

        // 체력이 0 이하가 되면 장애물 파괴
        if (health <= 0)
        {
            DestroyObstacle();
        }
    }

    IEnumerator DamageEffect()
    {
        // SpriteRenderer를 사용해 색상을 빨간색으로 변경하여 데미지 시각화
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = sprite.color; // 원래 색상 저장
        sprite.color = Color.red; // 색상을 빨간색으로 변경

        // 0.1초 대기
        yield return new WaitForSeconds(0.1f);

        // 원래 색상으로 복구
        sprite.color = originalColor;
    }

    void DestroyObstacle()
    {
        // 파괴 효과 생성
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            //effect.transform.localScale = transform.localScale; // 크기 조정
            effect.transform.rotation = Quaternion.Euler(-90, 0, 0);

            // Particle System 강제 재생
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
        
        
        // 프리팹화해서 적용하기 (응용)
        if (destroySounds != null)
        {
            GameObject sounds = Instantiate(destroySounds, transform.position, Quaternion.identity);
            AudioSource AS = sounds.GetComponent<AudioSource>();
            if (AS != null)
            {
                AS.Play();
            }
        }

        manager.score += 100;


        // 장애물 오브젝트 삭제
        Destroy(gameObject);
    }

    void PlayHitSound()
    {
        // 충돌 사운드 재생
        // if (hitSound != null && audioSource != null)
        // {
        //     audioSource.PlayOneShot(hitSound);
        // }
        
        if (hitSounds != null && audioSource != null)
        {
            Debug.Log("Woowkk");
            GameObject soundss = Instantiate(hitSounds, transform.position, Quaternion.identity);
            AudioSource ASs = soundss.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                ASs.Play();
            }
        }
    }
}
