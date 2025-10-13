using UnityEngine;

public class BallProjectile : BaseProjectile
{
    [SerializeField] private int damage = 1;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    private void Update()
    {
        if (Rb != null)
        {
            Rb.linearVelocity = Velocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block"))
        {
            if (other.gameObject.activeSelf)
            {
                other.GetComponent<ITakeDamage>().TakeDamage(damage);
                PoolManager.Instance.ReturnObject(gameObject);
            }
        }

        if (other.CompareTag("BossBlock"))
        {
            other.GetComponent<ITakeDamage>().TakeDamage(damage);
            PoolManager.Instance.ReturnObject(gameObject);
        }
    }
}
