using System;
using UnityEngine;

namespace Jail.Utility
{
    //[AttributeUsage(AttributeTargets.Method)]
    public class InvokeMethodAttribute : PropertyAttribute 
    {
        public string Text { get; }
        public string MethodName { get; }

        public InvokeMethodAttribute(string text, string method_name)
        {
            Text = text;
            MethodName = method_name;
        }
    }
}