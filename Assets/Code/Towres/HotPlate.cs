using UnityEngine;

public class HotPlate : TargetingTower
{
    public float damagePerSecond = 10;

    private void Update()
    {
        if (targeter.TargetsAreAvailable)
        {
            for (int i = 0; i < targeter.enemies.Count; i++)
            {
                Enemy enemy = targeter.enemies[i];

                if (enemy is GroundEnemy)
                {
                    enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
    }
}
