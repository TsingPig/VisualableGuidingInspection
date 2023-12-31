//-----------------------------------------------------------------------
// <copyright file="ExpandedStateExample.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
    using Sirenix.OdinInspector.Editor.Examples.Internal;
    using System.Collections.Generic;

    [AttributeExample(typeof(OnStateUpdateAttribute), "The following example shows how OnStateUpdate can be used to control the expanded state of a list.")]
	[ExampleAsComponentData(Namespaces = new string[] { "System.Collections.Generic" })]
	internal class ExpandedStateExample
	{
		[OnStateUpdate("@$property.State.Expanded = ExpandList")]
		public List<string> list;

		public bool ExpandList;
	}
}
#endif