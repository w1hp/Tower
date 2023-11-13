using UnityEngine;

public class SeekingProjectile : Projectile
{
    [Header("References")]
    public Transform trans;

    private Vector3 targetPosition;

    private void Update()
    {
        if (targetEnemy != null)
        {
            targetPosition = targetEnemy.projectileSeekPoint.position;
        }

        trans.forward = (targetPosition - trans.position).normalized;

        trans.position = Vector3.MoveTowards(trans.position, targetPosition, speed * Time.deltaTime);
        
        if (trans.position == targetPosition)
        {
            if (targetEnemy != null)
            {
                targetEnemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
    protected override void OnSetup(){}
}
