using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jail.UI
{
    public class MenuControllerButton : MonoBehaviour
    {
        Button button;

        [SerializeField]
        Color normalColor, highlightedColor, pressedColor;

        public MenuControllerButton buttonAtLeft, buttonAtUp, buttonAtRight, buttonAtDown;

        void Awake()
        {
            button = GetComponent<Button>();

            ColorBlock buttonColors = new ColorBlock();
            buttonColors.normalColor = normalColor;
            buttonColors.highlightedColor = highlightedColor;
            buttonColors.pressedColor = pressedColor;
            buttonColors.selectedColor = normalColor;
            buttonColors.colorMultiplier = 2.0f;
            button.colors = buttonColors;

            button.targetGraphic.color = normalColor;
        }

        public void GoNormal()
        {
            button.targetGraphic.color = normalColor;
        }

        public void GoHighlight()
        {
            button.targetGraphic.color = highlightedColor;
        }

        public void Press()
        {
            button.targetGraphic.color = pressedColor;
            button.onClick.Invoke();
        }
    }
}
