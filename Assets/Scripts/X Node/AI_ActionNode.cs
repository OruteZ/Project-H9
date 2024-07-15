using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using X_Node;
using XNode;

namespace XNode
{
	[CreateNodeMenu("Node : ActionNode")]
	public class AIActionNode : AIBaseNode {

		// Use this for initialization
		protected override void Init() {
			base.Init();
		
		}

		// Return the correct value of an output port when requested
		public override object GetValue(NodePort port) {
			return null; // Replace this
		}
	}
}

