public enum Gender
{
    Female = 0,
    Male = 1
}

public class PatientInfo
{
    public string patientID;
    public string patientName;
    public Gender patientGender;
    public int patientAge;
    public string patientAddress;
    public string patientPhoneNumber;
}
namespace TsingPigSDK
{

    public static partial class Log
    {
        public static void Info(PatientInfo patientInfo)
        {
            Log.Info(
                "ID：" + patientInfo.patientID,
                "姓名：" + patientInfo.patientName,
                "性别：" + patientInfo.patientGender.ToString(),
                "年龄：" + patientInfo.patientAge.ToString(),
                "地址：" + patientInfo.patientAddress,
                "电话：" + patientInfo.patientPhoneNumber
            );
        }
    }

}