
public class TargetingTower : Tower
{
    public Targeter targeter;
    public int range = 45;

    protected virtual void Start()
    {
        targeter.SetRange(range);
    }
}
