using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public Collider col;
    [HideInInspector] public List<Enemy> enemies = new List<Enemy>();
    public bool TargetsAreAvailable
    {
        get
        {
            return enemies.Count > 0;
        }
    }
    public void SetRange(int range)
    {
        if (col is BoxCollider)
        {
            (col as BoxCollider).size = new Vector3(range * 2, 30, range * 2);
            (col as BoxCollider).center = new Vector3(0, 15, 0);
        }
        else if (col is SphereCollider)
        {
            (col as SphereCollider).radius = range;
        }

        // BoxCollider boxCol = col as BoxCollider;
        // if (boxCol != null)
        // {
        //     boxCol.size = new Vector3(range * 2, 30, range * 2);
        //     boxCol.center = new Vector3(0, 15, 0);
        // }
        // else
        // {
        //     SphereCollider SphereCol = col as SphereCollider;
        //     if (SphereCol != null)
        //     {
        //         SphereCol.radius = range;
        //     }
        // }
    }
    public Enemy GetClosestEnemy(Vector3 point)
    {
        float lowestDistance = Mathf.Infinity;

        Enemy enemyWithLowestDistance = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i];

            if (enemy == null || !enemy.alive)
            {
                enemies.RemoveAt(i);
                // i--;
                i -= 1;
            }
            else
            {
                float dist = Vector3.Distance(point, enemy.trans.position);

                if (dist < lowestDistance)
                {
                    lowestDistance = dist;
                    enemyWithLowestDistance = enemy;
                }
            }
        }
        return enemyWithLowestDistance;
    }
    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemies.Add(enemy);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemies.Remove(enemy);
        }
    }
}
