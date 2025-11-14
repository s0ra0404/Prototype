using System;
using UnityEngine;

namespace Ver2
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string Label { get; }

        public ButtonAttribute(string label = null)
        {
            Label = "Debug : " + label;
        }
    }
}
