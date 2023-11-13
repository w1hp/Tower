using UnityEngine;
using UnityEngine.AI;

public class GroundEnemy : Enemy
{
    public static NavMeshPath path;
    public float moveSpeed = 22;

    private int currentCornerIndex = 0;
    private Vector3 currentCorner;
    private bool IsCurrentCornerFinal
    {
        get
        {
            return currentCornerIndex == (path.corners.Length - 1);
        }
    }
    private void GetNextCorner()
    {
        currentCornerIndex += 1;
        currentCorner = path.corners[currentCornerIndex];
    }
    protected override void Start()
    {
        base.Start();
        currentCorner = path.corners[0];
    }
    void Update()
    {
        if (currentCornerIndex != 0)
        {
            trans.forward = (currentCorner - trans.position).normalized;
        }
        trans.position = Vector3.MoveTowards(trans.position, currentCorner, moveSpeed * Time.deltaTime);

        if (trans.position == currentCorner)
        {
            if (IsCurrentCornerFinal)
            {
                Leak();
            }
            else
            {
                GetNextCorner();
            }
        }
    }

}
