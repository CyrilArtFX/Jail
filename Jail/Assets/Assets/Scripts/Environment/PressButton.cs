using UnityEngine;
using UnityEngine.Events;

public class PressButton : MonoBehaviour
{
    [SerializeField] 
    UnityEvent onButtonPress = default;

    void Awake()
    {
        enabled = false;
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Press();
        }
    }

    void Press()
    {
        onButtonPress.Invoke();
        enabled = false;
    }
}
