using DG.Tweening;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Model Reference")]
    [SerializeField] private GameObject model;

    [Header("Movement Settings")]
    [SerializeField] private float forwardSpeed = 10f;   
    [SerializeField] private float moveSpeed = 0.02f;    
    [SerializeField] private float xMin = -5f;           
    [SerializeField] private float xMax = 5f;            

    [Header("Shooting Settings")]
    [SerializeField] private Transform firePoint;      
    [SerializeField] private float fireRate = 5f;

    [Header("HP")]
    [SerializeField] private int hpAmount = 3;
    [SerializeField] private int currentlyHP = 3;
    [SerializeField] private SpriteRenderer iFrameImg;

    [Header("Game Points")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    private bool _isTouching = false;
    private float _fireTimer = 0f;
    private float _lastTouchX;
    private bool _isCrashing = false;
    private bool _isStarting = false;
    private bool _isBossComing = false;

    private void OnEnable()
    {
        Subscribe();
        Init();
    }

    private void Subscribe()
    {
        EventBroker.Subscribe(Events.ON_FIRST_TOUCH, OnPlay);
        EventBroker.Subscribe(Events.ON_LEVEL_FAIL, OnFinished);
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, OnFinished);
    }

    private void Init()
    {
        _isCrashing = false;
        _isTouching = false;
        _isStarting = false;
        _isBossComing = false;
        _fireTimer = 0f;
        currentlyHP = hpAmount;
        transform.position = startPoint.position;
        model.transform.position = startPoint.position;
        model.transform.rotation = startPoint.rotation;
        EventBroker.Publish(Events.SET_HP, currentlyHP);
    }

    private void OnPlay()
    {
        _isStarting = true;
        EventBroker.Publish(Events.ON_PLAY_PROGRESS_BAR, CalculatePath());
    }

    private void OnFinished()
    {
        _isStarting = false;
    }

    private void Update()
    {
        if (!_isStarting) return;

        HandleInput();
        HandleShooting();

        MoveForward(); 
    }

    #region Movement
    private void MoveForward()
    {
        if (_isBossComing) return;
        transform.Translate(forwardSpeed * Time.deltaTime * Vector3.forward);
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isTouching = true;
            _lastTouchX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButton(0))
        {
            float deltaX = Input.mousePosition.x - _lastTouchX;
            Move(deltaX * moveSpeed / 1000);

            Tilt(deltaX);
            _lastTouchX = Input.mousePosition.x;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isTouching = false;
        }

        if (!_isTouching)
        {
            ResetTilt();
        }
    }

    private void Move(float deltaX)
    {
        Vector3 pos = model.transform.position;
        pos.x += deltaX;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        model.transform.position = pos;
    }

    private void Tilt(float deltaX)
    {
        float targetY = Mathf.Clamp(deltaX * 2f, -15f, 15f);
        Quaternion targetRot = Quaternion.Euler(0f, targetY, 0f);
        model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, targetRot, Time.deltaTime * 5f);
    }

    private void ResetTilt()
    {
        Quaternion flat = Quaternion.identity;
        model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, flat, Time.deltaTime * 5f);
    }

    #endregion

    #region Shooting
    private void HandleShooting()
    {
        if (_isTouching)
        {
            _fireTimer += Time.deltaTime;
            float interval = 1f / fireRate;

            if (_fireTimer >= interval)
            {
                Fire();
                _fireTimer = 0f;
            }
        }
    }


    private void Fire()
    {
        var projectile = PoolManager.Instance.GetObject(PoolType.Projectile);
        projectile.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
        projectile.GetComponent<BaseProjectile>().TakeEnableValues(Vector3.forward * forwardSpeed);
        SoundManager.Instance.Play("Shoot");
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 left = new(xMin, transform.position.y, transform.position.z);
        Vector3 right = new(xMax, transform.position.y, transform.position.z);
        Gizmos.DrawLine(left + Vector3.up, left + Vector3.down);
        Gizmos.DrawLine(right + Vector3.up, right + Vector3.down);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            if (other.gameObject.activeSelf && !_isCrashing)
            {
                _isCrashing = true;
                if (SetHP(-1))
                {
                    SoundManager.Instance.Play("Damage");
                    StartCoroutine(CrashIFrame());
                    PoolManager.Instance.ReturnObject(other.gameObject);
                }
            }
        }

        if (other.CompareTag("Finish"))
        {
           _isBossComing = true;
        }
    }

    private IEnumerator CrashIFrame()
    {
        iFrameImg.gameObject.SetActive(true);
        iFrameImg.DOFade(1, 0.01f);
        iFrameImg.DOFade(0, 1).SetLoops(3);
        yield return new WaitForSeconds(3);
        _isCrashing = false;
        iFrameImg.gameObject.SetActive(false);
    }

    private bool SetHP(int value)
    {
        currentlyHP += value;
        EventBroker.Publish(Events.SET_HP, currentlyHP);

        if (currentlyHP <= 0)
        {
            EventBroker.Publish(Events.ON_LEVEL_FAIL);
            return false;
        }
        return true;
    }

    private float CalculatePath()
    {
        return Vector3.Distance(startPoint.position, endPoint.position) / forwardSpeed;
    }

    private void Unsubscribe()
    {
        EventBroker.UnSubscribe(Events.ON_FIRST_TOUCH, OnPlay);
        EventBroker.UnSubscribe(Events.ON_LEVEL_FAIL, OnFinished);
        EventBroker.Subscribe(Events.ON_LEVEL_SUCCESS, OnFinished);
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
