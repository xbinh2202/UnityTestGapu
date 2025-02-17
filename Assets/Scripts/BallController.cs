using System.Collections;
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

        // Gán màu vào sprite
        _iconSpr.color = randomColor;

        _isExploded = false;
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
        yield return new WaitForSeconds(_delay);
        DetectDestroy();
        this.gameObject.SetActive(false);
    }

    public void Movement()
    {

        if (_rb != null)
        {
            Vector2 direction = (GameManager.Instance.getMousePos() - transform.position).normalized;

            // Thêm lực theo hướng
            // _rb.AddForce(direction * _forceAmount, ForceMode2D.Impulse);
            _rb.velocity = direction * _forceAmount;
        }
    }

    // Giới hạn vận tốc của bóng
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
        Vector3 screenMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)); // Góc dưới bên trái
        Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)); // Góc trên bên phải

        // Đảm bảo quả bóng không ra ngoài giới hạn màn hình
        float xPos = Mathf.Clamp(transform.position.x, screenMin.x, screenMax.x);
        float yPos = Mathf.Clamp(transform.position.y, screenMin.y, screenMax.y);

        // Cập nhật lại vị trí của quả bóng
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
                // Tính hướng phản xạ
                Vector2 direction1 = (_rb.position - otherRb.position).normalized;

                // Đặt lại velocity để bóng bật ra
                _rb.velocity = direction1 * _forceAmount;
                otherRb.velocity = -direction1 * _forceAmount;
            }
        }
    }

    void DetectDestroy()
    {
        // Phát hiện các vật thể trong bán kính
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _detectDestroyRadius);
        Debug.LogError(hits.Count());
        // Vẽ raycast từ vị trí bóng theo hướng vận tốc
        foreach (Collider2D hit in hits)
        {
            // Bỏ qua chính bản thân bóng
            if (hit.gameObject != gameObject)
            {
                Rigidbody2D otherRb = hit.GetComponent<Rigidbody2D>();

                if (otherRb != null)
                {
                    // Tính hướng phản xạ

                    // Đặt lại velocity để bóng bật ra
                    // Tính vector từ quả bóng nổ đến các quả bóng xung quanh
                    Vector2 explosionDirection = otherRb.position - (Vector2)transform.position;

                    // Sử dụng rb.velocity để đẩy các quả bóng
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
