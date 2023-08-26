using System.Collections.Generic;
using UnityEngine;

public enum Gender
{
    Female = 0,
    Male = 1
}


[CreateAssetMenu(fileName = "Address Config", menuName = "Data/Address")]
public class Address_SO : ScriptableObject
{
    [Header("地址区预设"), SerializeField] private List<string> _districtsLib;

    [Header("地址街道预设"), SerializeField] private List<string> _streetsLib;
    public List<string> DistrictsLib { get => _districtsLib; }
    public List<string> StreetsLib { get => _streetsLib; }
}

[CreateAssetMenu(fileName = "PatientName Config", menuName = "Data/PatientName")]
public class PatientName_SO : ScriptableObject
{
    [Header("姓预设"), SerializeField] private List<string> _firstNameLib;

    [Header("女名预设"), SerializeField] private List<string> _femaleLastNameLib;

    [Header("男名预设"), SerializeField] private List<string> _maleLastNameLib;
    public List<string> FirstNameLib => _firstNameLib;
    public List<string> FemaleLastNameLib => _femaleLastNameLib;
    public List<string> MaleLastNameLib => _maleLastNameLib;
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
