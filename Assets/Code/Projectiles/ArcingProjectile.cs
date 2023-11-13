using UnityEngine;

public class ArcingProjectile : Projectile
{
    public Transform trans;
    public LayerMask enemyLayerMask;
    public float explosionRadius = 25;
    public AnimationCurve curve;

    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private float xzDistanceToTravel;
    private float spawnTime;
    private float FractionOfDistanceTraveled
    {
        get
        {
            float timeSinceSpawn = Time.time - spawnTime;
            float timeToReachDestination = xzDistanceToTravel / speed;

            return timeSinceSpawn / timeToReachDestination;
        }
    }
    protected override void OnSetup()
    {
        initialPosition = trans.position;
        targetPosition = targetEnemy.trans.position;

        targetPosition.y = 0;

        xzDistanceToTravel = Vector3.Distance(new Vector3(trans.position.x, targetPosition.y, trans.position.z), targetPosition);

        spawnTime = Time.time;
    }

    void Update()
    {
        Vector3 currentPosition = trans.position;
        currentPosition.y = 0;

        currentPosition = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        currentPosition.y = Mathf.Lerp(initialPosition.y, targetPosition.y, curve.Evaluate(FractionOfDistanceTraveled));

        trans.position = currentPosition;

        if (currentPosition == targetPosition)
        {
            Explode();
        }
    }
    void Explode()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(trans.position, explosionRadius, enemyLayerMask.value);

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            var enemy = enemyColliders[i].GetComponent<Enemy>();

            if (enemy != null)
            {
                float distToEnemy = Vector3.Distance(trans.position, enemy.trans.position);
                float damageToDeal = damage * (1 - Mathf.Clamp(distToEnemy / explosionRadius, 0f, 1f));
                enemy.TakeDamage(damageToDeal);
            }
        }
        Destroy(gameObject);
    }
}