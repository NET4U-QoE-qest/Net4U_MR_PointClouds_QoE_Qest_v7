using UnityEngine;
[System.Flags]
public enum PCObjectType
{
[InspectorName("Deselect All")] DeselectAll = 0,
[InspectorName("Select All")] SelectAll = ~0,
BlueSpin_UVG_vox10_25_0_250 = 1 <<0,
flowerDance = 1 <<1,
LongDress = 1 <<2,
Loot = 1 <<3,
pietra = 1 <<4,
Redandblack = 1 <<5,
Soldier = 1 <<6,
Unknown = 1 <<7,
};
