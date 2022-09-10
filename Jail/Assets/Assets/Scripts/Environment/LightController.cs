using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jail.LightControl
{
    enum LightFadeState
    {
        FadeIn,
        FadeOut,
        Off
    }

    public class LightController : MonoBehaviour
    {

        [SerializeField]
        float fadeTime = 0.3f;

        Light lightToControl = default;
        float maxIntensity;

        LightFadeState lightFadeState = LightFadeState.Off;
        float timeSinceFadeStarted = 0.0f;

        [HideInInspector]
        public bool lightOn = false;

        private void Awake()
        {
            lightToControl = GetComponent<Light>();
            maxIntensity = lightToControl.intensity;
        }

        void Update()
        {
            switch (lightFadeState)
            {
                case LightFadeState.Off:
                    return;

                case LightFadeState.FadeIn:

                    timeSinceFadeStarted += Time.deltaTime;

                    if (timeSinceFadeStarted >= fadeTime)
                    {
                        lightToControl.intensity = maxIntensity;
                        lightFadeState = LightFadeState.Off;
                    }
                    else
                    {
                        lightToControl.intensity = timeSinceFadeStarted / fadeTime;
                    }

                    break;

                case LightFadeState.FadeOut:

                    timeSinceFadeStarted += Time.deltaTime;

                    if (timeSinceFadeStarted >= fadeTime)
                    {
                        lightToControl.intensity = 0.0f;
                        lightFadeState = LightFadeState.Off;
                    }
                    else
                    {
                        lightToControl.intensity = 1.0f - timeSinceFadeStarted / fadeTime;
                    }

                    break;
            }
        }

        public void FadeIn()
        {
            lightToControl.intensity = 0.0f;
            lightFadeState = LightFadeState.FadeIn;
            timeSinceFadeStarted = 0.0f;
            lightOn = true;
        }

        public void FadeOut()
        {
            lightToControl.intensity = maxIntensity;
            lightFadeState = LightFadeState.FadeOut;
            timeSinceFadeStarted = 0.0f;
            lightOn = false;
        }

        public void TurnLightOff()
        {
            lightToControl.intensity = 0.0f;
            lightOn = false;
        }
    }
}
