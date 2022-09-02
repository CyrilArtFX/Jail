using UnityEngine;

public class TwoStatesAnim : MonoBehaviour
{
    [SerializeField] Animator anim = default;
    [SerializeField] string boolName = default;

    public void ChangeBool(bool value)
    {
        anim.SetBool(boolName, value);
    }
}
