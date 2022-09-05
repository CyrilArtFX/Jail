using UnityEngine;

[System.Serializable]
public struct TwoStatesAnim
{
    [SerializeField] Animator anim;
    [SerializeField] string boolName;

    public void ChangeBool(bool value)
    {
        anim.SetBool(boolName, value);
    }
}
