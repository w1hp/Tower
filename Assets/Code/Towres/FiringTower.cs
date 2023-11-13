using UnityEngine;

public class FiringTower : TargetingTower
{
    public Transform trans;
    public Transform projectileSpawnPoint;
    public Transform aimer;
    public float fireInterval = .5f;
    public Projectile projectilePrefab;
    public float damege;
    public float projectileSpeed = 60;
    public bool canAttackFlying = true;

    private Enemy targetedEnemy;
    private float lastFireTime = Mathf.NegativeInfinity;
    private void Update()
    {
        if (targetedEnemy != null)
        {
            if (!targetedEnemy.alive || Vector3.Distance(trans.position, targetedEnemy.trans.position) > range)
            {
                GetNextTarget();
            }
            else
            {
                if (canAttackFlying || targetedEnemy is GroundEnemy)
                {
                    AimAtTarget();

                    if (Time.time > lastFireTime + fireInterval)
                    {
                        Fire();
                    }
                }
            }
        }
        else if (targeter.TargetsAreAvailable)
        {
            GetNextTarget();
        }
    }
    private void AimAtTarget()
    {
        if (aimer)
        {
            Vector3 to = targetedEnemy.trans.position;
            to.y = 0;

            Vector3 from = aimer.position;
            from.y = 0;

            Quaternion desiredRotation = Quaternion.LookRotation((to - from).normalized, Vector3.up);
            aimer.rotation = Quaternion.Slerp(aimer.rotation, desiredRotation, .08f);
        }
    }
    private void GetNextTarget()
    {
        targetedEnemy = targeter.GetClosestEnemy(trans.position);
    }

    private void Fire()
    {
        lastFireTime = Time.time;

        var proj = Instantiate<Projectile>(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        proj.Setup(damege, projectileSpeed, targetedEnemy);
    }
}
