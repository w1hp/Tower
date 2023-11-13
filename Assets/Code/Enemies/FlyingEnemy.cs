using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float moveSpeed;

    private Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();

        targetPosition = GroundEnemy.path.corners[GroundEnemy.path.corners.Length - 1];
        targetPosition.y = trans.position.y;
    }
    void Update()
    {
        trans.position = Vector3.MoveTowards(trans.position, targetPosition, moveSpeed * Time.deltaTime);
        if (trans.position == targetPosition)
        {
            Leak();
        }
    }
}
