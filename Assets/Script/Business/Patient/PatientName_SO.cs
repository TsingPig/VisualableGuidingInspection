using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PatientName", menuName = "Data/PatientName")]
public class PatientName_SO : ScriptableObject
{
    [Header("姓预设"), SerializeField] private List<string> _firstNameLib;

    [Header("女名预设"), SerializeField] private List<string> _femaleLastNameLib;

    [Header("男名预设"), SerializeField] private List<string> _maleLastNameLib;
    public List<string> FirstNameLib => _firstNameLib;
    public List<string> FemaleLastNameLib => _femaleLastNameLib;
    public List<string> MaleLastNameLib => _maleLastNameLib;
}
