//-----------------------------------------------------------------------
// <copyright file="ToggleLeftExamples.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(ToggleLeftAttribute))]
    internal class ToggleLeftExamples
    {
        [InfoBox("Draws the toggle button before the label for a bool property.")]
        [ToggleLeft]
        public bool LeftToggled;

        [EnableIf("LeftToggled")]
        public int A;

        [EnableIf("LeftToggled")]
        public bool B;

        [EnableIf("LeftToggled")]
        public bool C;
    }
}
#endif