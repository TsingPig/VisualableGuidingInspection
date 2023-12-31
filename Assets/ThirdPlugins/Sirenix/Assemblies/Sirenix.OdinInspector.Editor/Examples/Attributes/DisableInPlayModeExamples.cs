//-----------------------------------------------------------------------
// <copyright file="DisableInPlayModeExamples.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;

    [AttributeExample(typeof(DisableInPlayModeAttribute))]
    internal class DisableInPlayModeExamples
    {
        [Title("Disabled in play mode")]
        [DisableInPlayMode]
        public int A;

        [DisableInPlayMode]
        public Material B;
    }
}
#endif