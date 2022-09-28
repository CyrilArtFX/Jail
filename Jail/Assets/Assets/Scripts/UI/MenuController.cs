using System.Collections.Generic;
using UnityEngine;

using Jail.UI.Glyphs;
using Cinemachine;

namespace Jail.UI
{
    public class MenuController : MonoBehaviour
    {
        public CinemachineVirtualCamera VirtualCamera => camera;

        public GlyphBaseUI CurrentButton => (currentButtonID >= 0 && currentButtonID < buttons.Count) ? buttons[currentButtonID] : null;
        public List<GlyphBaseUI> buttons;

        public GlyphButtonUI backButton;

        [SerializeField]
        CinemachineVirtualCamera camera;

        int currentButtonID = 0;

        public void NextButton()
        {
            SelectButtonByID((currentButtonID + 1) % buttons.Count);
        }

        public void PreviousButton()
        {
            SelectButtonByID(currentButtonID - 1 < 0 ? buttons.Count - 1 : currentButtonID - 1);
        }

        public virtual void Select()
        {
            camera.Priority = 11;
        }

        public virtual void UnSelect()
        {
            camera.Priority = 10;
        }

        public void Back()
		{
            if (backButton == null) return;

            backButton.DoClick();
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