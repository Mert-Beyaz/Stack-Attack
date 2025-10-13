using System.Collections;
using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    [SerializeField] private float speed;
    public Rigidbody Rb;
    public Vector3 Velocity;

    protected virtual void OnEnable()
    {
        StartCoroutine(Destroy(destroyTime));
    }

    IEnumerator Destroy(float destroyTime)
    {
        yield return new WaitForSeconds(destroyTime);

        if (gameObject.activeSelf) 
        {
            PoolManager.Instance.ReturnObject(gameObject);
        }
    }

    public void TakeEnableValues(Vector3 _velocity)
    {
        Velocity = _velocity * speed;
    }
}
