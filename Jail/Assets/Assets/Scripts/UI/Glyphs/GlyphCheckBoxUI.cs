using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Jail.UI.Glyphs
{
    public class GlyphCheckBoxUI : GlyphBaseUI
    {
        
        public UnityEvent<bool> OnToggleEvent;

        public bool IsPressed => isPressed;
        bool isPressed = false;

        [Header("References"), SerializeField]
        TMPro.TextMeshPro textMesh;
        [SerializeField]
        SpriteRenderer checkSprite, backgroundSprite;

        public override void DoClick()
        {
            SetPressed(!isPressed);
        }

        public override void SetColor(Color color)
        {
            checkSprite.color = color;
            backgroundSprite.color = color;
            textMesh.color = color;
        }

        public void SetPressed(bool is_pressed, bool no_event = false)
        {
            isPressed = is_pressed;
            
            //  toggle check sprite
            checkSprite.enabled = IsPressed;

            if (!no_event)
            {
                //  invoke events
                if (OnToggleEvent != null)
                {
                    OnToggleEvent.Invoke(isPressed);
                }
            }
        } 
    }
}