using UnityEngine;
using UnityEngine.Events;

public class PressButton : MonoBehaviour
{
    [SerializeField] UnityEvent onButtonPress = default;

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact")) Press();
    }

    private void Press()
    {
        onButtonPress.Invoke();
        enabled = false;
    }
}
