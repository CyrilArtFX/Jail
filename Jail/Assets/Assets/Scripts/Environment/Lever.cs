using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    [SerializeField] UnityEvent onLeverUp = default, onLeverDown = default;
    bool down = false;

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact")) ChangeValue();
    }

    private void ChangeValue()
    {
        down = !down;
        if (down) onLeverDown.Invoke();
        else onLeverUp.Invoke();
    }
}
