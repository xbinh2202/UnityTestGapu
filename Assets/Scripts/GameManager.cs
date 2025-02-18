using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] BallController _ballPrefab;
    private List<BallController> _ballPoolList = new();
    [SerializeField] GameObject _poolBall;
    [SerializeField] bool _canSpawnBall;
    [SerializeField] float _delay = 0.5f;

    private Coroutine _coroutine;
    //private MyPool pool;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //pool = new MyPool(_ballPrefab.gameObject);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _canSpawnBall = true;
            SpawnBall();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _canSpawnBall = false;
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }
    }

    private void SpawnBall()
    {
        _coroutine = StartCoroutine(WaitSpawnBall());
    }

    private IEnumerator WaitSpawnBall()
    {
        while (_canSpawnBall)
        {
            //BallController ball = getBall();
            //ball.Init();
            //ball.Movement();

            //var ball = pool.Get();

            PoolManager.Instance.GetFromPool(_ballPrefab.gameObject);
            //ball.transform.position = Vector3.zero;
            yield return new WaitForSeconds(_delay);
        }
    }

    private BallController getBall()
    {
        if (_ballPoolList.Count > 0)
        {
            for (int i = 0; i < _ballPoolList.Count; i++)
            {
                if (!_ballPoolList[i].gameObject.activeSelf)
                {
                    _ballPoolList[i].gameObject.SetActive(true);
                    return _ballPoolList[i];
                }
            }
        }
        BallController ball = Instantiate(_ballPrefab, Vector3.zero, Quaternion.identity);
        ball.transform.SetParent(_poolBall.transform);
        _ballPoolList.Add(ball);
        return ball;
    }

    public Vector3 getMousePos()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

}
