public class Beacon : SonarImmediateDrawer
{
    public static Beacon Instance { get; private set; }

    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        base.Awake();
    }
}