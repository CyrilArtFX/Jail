using System.Collections.Generic;
using UnityEngine;

using Jail.UI.Glyphs;

namespace Jail.UI
{
    public class MenuController : MonoBehaviour
    {
        public GlyphBaseUI CurrentButton => (currentButtonID >= 0 && currentButtonID < buttons.Count) ? buttons[currentButtonID] : null;
        public List<GlyphBaseUI> buttons;

        int currentButtonID = 0;

        public void NextButton()
        {
            SelectButtonByID((currentButtonID + 1) % buttons.Count);
        }

        public void PreviousButton()
        {
            SelectButtonByID(currentButtonID - 1 < 0 ? buttons.Count - 1 : currentButtonID - 1);
        }

        public void SelectButtonByID(int id)
        {
            if (CurrentButton != null)
            {
                CurrentButton.IsHovered = false;
            }

            currentButtonID = id;
            CurrentButton.IsHovered = true;
        }
    }
}