using System;

namespace Jail.Utility
{
    public class MethodButtonAttribute : Attribute 
    {
        public string Text { get; }

        public MethodButtonAttribute(string text)
        {
            Text = text;
        }
    }
}