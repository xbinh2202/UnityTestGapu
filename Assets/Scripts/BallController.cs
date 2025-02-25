﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] SpriteRenderer _iconSpr;
    [SerializeField] float _delay = 0.5f;
    [SerializeField] float _forceAmount = 500f;
    [SerializeField] float _forceExplosion = 10f;   // Tốc độ đẩy các quả bóng khi nổ
    [SerializeField] float _detectionRadius = 0.5f; // Bán kính phát hiện va chạm
    [SerializeField] float _detectDestroyRadius = 2f;
    [SerializeField] ParticleSystem _explosionBall;
    private bool _isExploded = false;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        Init();
        Movement();
    }
    // Update is called once per frame
    void Update()
    {
        DetectBounce();
        //LimitSpeed();
        ClampBallPosition();
    }

    public void Init()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        _iconSpr.color = randomColor;

        _isExploded = false;

        transform.position = Vector3.zero;
    }

    private void Hide()
    {
        if (_isExploded)
            return;
        _isExploded = true;
        StartCoroutine(WaitHide());
    }

    private IEnumerator WaitHide()
    {
        var main = _explosionBall.main;
        float timeFx = main.startLifetime.constant;
        yield return new WaitForSeconds(_delay - timeFx);
        _iconSpr.gameObject.SetActive(false);
        _explosionBall.Play();
        DetectDestroy();
        yield return new WaitForSeconds(timeFx);
        _iconSpr.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void Movement()
    {

        if (_rb != null)
        {
            Vector2 direction = (GameManager.Instance.getMousePos() - transform.position).normalized;
            _rb.velocity = direction * _forceAmount;
        }
    }

    void LimitSpeed()
    {
        if (_rb.velocity.magnitude > _forceAmount)
        {
            _rb.velocity = _rb.velocity.normalized * _forceAmount;
        }
    }

    void ClampBallPosition()
    {
        // Lấy các giới hạn màn hình trong không gian thế giới
        Vector3 screenMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Đảm bảo quả bóng không ra ngoài giới hạn màn hình
        float xPos = Mathf.Clamp(transform.position.x, screenMin.x, screenMax.x);
        float yPos = Mathf.Clamp(transform.position.y, screenMin.y, screenMax.y);

        transform.position = new Vector2(xPos, yPos);
    }


    private void DetectBounce()
    {
        //***CHECK CANH MAN HINH***
        // Lấy giới hạn màn hình
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        if (transform.position.x >= screenBounds.x || transform.position.x <= -screenBounds.x)
        {
            _rb.velocity = new Vector2(-_rb.velocity.x, _rb.velocity.y);
            Hide();
        }

        if (transform.position.y >= screenBounds.y || transform.position.y <= -screenBounds.y)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, -_rb.velocity.y);
            Hide();
        }

        //***CHECK VA CHAM GIUA CAC BONG***
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _detectionRadius, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject != this.gameObject)
        {
            // Vector2 direction = (hit.collider.transform.position - transform.position).normalized;
            //  _rb.AddForce(-direction * _forceAmount);
            Rigidbody2D otherRb = hit.collider.GetComponent<Rigidbody2D>();

            if (otherRb != null)
            {
                Vector2 direction1 = (_rb.position - otherRb.position).normalized;

                _rb.velocity = direction1 * _forceAmount;
                otherRb.velocity = -direction1 * _forceAmount;
            }
        }
    }

    void DetectDestroy()
    {
        _explosionBall.Play();
        // Phát hiện các vật thể trong bán kính
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectDestroyRadius);
        Debug.LogError(hits.Count());

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();

                if (otherRb != null)
                {
                    Vector2 explosionDirection = otherRb.position - (Vector2)transform.position;
                    otherRb.velocity = explosionDirection.normalized * _forceExplosion;

                }
            }
        }
    }

    void OnDrawGizmos()
    {
        // Vẽ vòng tròn bán kính phát hiện va chạm (Sphere 1)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);

        // Vẽ bán kính nổ (Explosion Radius)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectDestroyRadius);
    }
}
