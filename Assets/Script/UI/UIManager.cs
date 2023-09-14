using TsingPigSDK;

public class UIManager : UISystem
{
    private new void Awake()
    {
        base.Awake();
        _panelBuffer = new PanelBuffer();
        Log.Info("UIManager");
    }
    private void Start()
    {
    }
}