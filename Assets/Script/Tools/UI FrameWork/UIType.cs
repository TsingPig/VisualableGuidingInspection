namespace TsingPigSDK
{
    public class UIType
    {
        public string Name { get; private set; }
        public bool FloatingPanel {  get; private set; }
        public UIType(string name, bool floatingPanel)
        {
            Name = name;
            FloatingPanel = floatingPanel;
        }
    }
}