using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Address", menuName = "Data/Address")]
public class Address_SO : ScriptableObject
{
    [Header("地址区预设"), SerializeField] private List<string> _districtsLib;

    [Header("地址街道预设"), SerializeField] private List<string> _streetsLib;
    public List<string> DistrictsLib { get => _districtsLib; }
    public List<string> StreetsLib { get => _streetsLib; }
}