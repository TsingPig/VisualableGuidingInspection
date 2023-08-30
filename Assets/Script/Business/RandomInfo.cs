using TsingPigSDK;
using UnityEngine;

public static partial class RandomInfo
{
    public static string RandomPatientID(int idLength = 8)
    {
        string patientID = System.Guid.NewGuid().ToString().Substring(0, idLength);
        return patientID;
    }
    public static string RandomPatientName(Gender gender = Gender.Male)
    {
        string firstName = DataManager.Instance.PatientNameData.FirstNameLib.GetRandomItem();
        string lastName = string.Empty;
        if (gender == Gender.Male)
        {
            lastName = DataManager.Instance.PatientNameData.MaleLastNameLib.GetRandomItem();
        }
        else if (gender == Gender.Female)
        {
            lastName = DataManager.Instance.PatientNameData.FemaleLastNameLib.GetRandomItem();
        }
        return firstName + lastName;
    }
    public static Gender RandomGender
    {
        get
        {
            var gender = Random.Range(0, 2);
            return (Gender)gender;
        }
    }
    public static int RandomAge
    {
        get
        {
            return Random.Range(18, 35);
        }
    }
    public static string RandomAddress
    {
        get
        {
            string rndDistrict = DataManager.Instance.AddressData.DistrictsLib.GetRandomItem();
            string rndStrees = DataManager.Instance.AddressData.StreetsLib.GetRandomItem();
            return rndDistrict + rndStrees;
        }
    }
    public static string RandomPhoneNumber
    {
        get
        {
            string prefix = "1" + "35789"[Random.Range(0, 5)].ToString();
            string rest = "";
            for (int i = 0; i < 9; i++) rest += Random.Range(0, 10).ToString();
            string phoneNumber = prefix + rest;
            return phoneNumber;
        }
    }
    public static PatientInfo RandomPatientInfo()
    {
        var patientID = RandomInfo.RandomPatientID();
        var patientGender = RandomInfo.RandomGender;
        var patientName = RandomInfo.RandomPatientName(patientGender);
        var patientAge = RandomInfo.RandomAge;
        var patientAddress = RandomInfo.RandomAddress;
        var patientPhoneNumber = RandomInfo.RandomPhoneNumber;
        PatientInfo patientInfo = new PatientInfo()
        {
            patientID = patientID,
            patientGender = patientGender,
            patientName = patientName,
            patientAge = patientAge,
            patientAddress = patientAddress,
            patientPhoneNumber = patientPhoneNumber
        };
        Log.Info(patientInfo);
        return patientInfo;
    }
}

