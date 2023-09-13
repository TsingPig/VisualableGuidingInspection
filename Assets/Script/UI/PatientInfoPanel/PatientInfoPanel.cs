using TMPro;
using TsingPigSDK;

public class PatientInfoPanel : BasePanel
{
    TMP_Text _patientID;
    TMP_Text _patientName;
    TMP_Text _patientAge;
    TMP_Text _patientGender;
    TMP_Text _patientAddress;
    TMP_Text _patientPhone;

    PatientInfo _patientInfo;
    public PatientInfoPanel(PatientInfo patientInfo) 
    {
        _patientInfo = patientInfo;
    }
    public override void OnEntry()
    {
        _patientID = UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("ID");
        _patientName = UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("姓名");
        _patientAge = UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("年龄");
        _patientAddress = UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("地址");
        _patientGender = UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("性别");
        _patientPhone = UIManager.Instance.GetOrAddComponentInChilden<TMP_Text>("电话");
        
        
        _patientID.text = _patientInfo.patientID;
        _patientName.text= _patientInfo.patientName;
        _patientAddress.text= _patientInfo.patientAddress;
        _patientGender.text = _patientInfo.patientGender == Gender.Male ? "男" : "女";
        _patientPhone.text = _patientInfo.patientPhoneNumber;
        _patientAge.text= _patientInfo.patientAge.ToString();   
    }
}
