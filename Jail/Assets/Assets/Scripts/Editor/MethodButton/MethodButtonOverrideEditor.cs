using UnityEditor;
using UnityEngine;

namespace Jail.Unity.MethodButton
{
    /*
     * We must override the default MonoBehaviour Editor in order to show our
     * custom method buttons since Unity only handles members variables.
     * 
     * Mainly inspired from https://github.com/madsbangh/EasyButtons
     */

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class MethodButtonOverrideEditor : Editor
    {
        MethodButtonHandler handler;

        void OnEnable()
        {
            handler = new MethodButtonHandler(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            handler.Draw(targets);
        }
    }
}