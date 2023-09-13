using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Highlighters;
public interface ISelectable {
	Highlighter Highlighter { get; }
	void OnSelected();
	void OffSelected();
}
