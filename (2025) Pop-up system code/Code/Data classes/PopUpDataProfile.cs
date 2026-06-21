using UnityEngine;

[CreateAssetMenu(fileName = "PUp_Profile", menuName = "Pop-up Data Profile", order = 1)]
public class PopUpDataProfile : ScriptableObject
{
    [SerializeField] private PopUpData popUpData;
    public PopUpData PopUpData => popUpData;
}
