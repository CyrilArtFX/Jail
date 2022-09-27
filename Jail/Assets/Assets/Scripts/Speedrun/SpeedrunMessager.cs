using UnityEngine;

using TMPro;

namespace Jail.Speedrun
{
    [RequireComponent(typeof(TMP_Text))]
    public class SpeedrunMessager : MonoBehaviour
    {
        public static SpeedrunMessager instance;

        public bool IsValid { get; protected set; }
        public bool IsVisible { get; protected set; }

        [SerializeField]
        Color validColor = Color.green, invalidColor = Color.red;

        TMP_Text messageTMP;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            messageTMP = GetComponent<TMP_Text>();
        }

        void Start()
        {
            SetVisible(false);
        }

        public void SetVisible(bool is_visible)
        {
            IsVisible = is_visible;

            messageTMP.enabled = is_visible;
        }

        public void SetMessage(string text, bool is_valid)
        {
            IsValid = is_valid;

            messageTMP.text = text;
            messageTMP.color = is_valid ? validColor : invalidColor;

            SetVisible(true);
        }
    }
}