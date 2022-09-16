using UnityEditor;
using UnityEngine;

using Jail.Utility;

namespace Jail.Debug
{
    [CustomPropertyDrawer(typeof(InvokeMethodAttribute))]
    public class InvokeMethodPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InvokeMethodAttribute attr = (InvokeMethodAttribute) attribute;
            UnityEngine.Debug.Log(attr.Text + " " + property.propertyType);
            
            if (GUILayout.Button(attr.Text))
            {
                GUILayout.Label("yes!");

            }
        }
    }
}