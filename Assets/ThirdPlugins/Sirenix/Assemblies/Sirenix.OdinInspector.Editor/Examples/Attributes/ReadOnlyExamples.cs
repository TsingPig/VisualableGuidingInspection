//-----------------------------------------------------------------------
// <copyright file="ReadOnlyExamples.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(ReadOnlyAttribute), "ReadOnly disables properties in the inspector.")]
    internal class ReadOnlyExamples
    {
        [ReadOnly]
        public string MyString = "This is displayed as text";

        [ReadOnly]
        public int MyInt = 9001;

        [ReadOnly]
        public int[] MyIntList = new int[] { 1, 2, 3, 4, 5, 6, 7, };
    }
}
#endif