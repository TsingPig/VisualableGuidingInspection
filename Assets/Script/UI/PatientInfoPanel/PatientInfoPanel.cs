using TMPro;
using TsingPigSDK;

public class PatientInfoPanel : BasePanel
{
    TextMeshPro _patientID;

    PatientInfo _patientInfo;
    public PatientInfoPanel(PatientInfo patientInfo) 
    {
        _patientInfo = patientInfo;
    }
    public override void OnEntry()
    {
        _patientID = UIManager.Instance.GetOrAddComponentInChilden<TextMeshPro>("ID");
        _patientID.text = _patientInfo.patientID;
    }
}
