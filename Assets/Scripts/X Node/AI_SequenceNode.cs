using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace X_Node
{
    [CreateNodeMenu("Node : SequenceNode")]
    public class AISequenceNode : AIBaseNode
    {
        [Output(dynamicPortList = true)] public List<bool> output;
        [SerializeField] private int _currentChildIndex = 0; 
        
        // Use this for initialization
        protected override void Init()
        {
            base.Init();
            
            //set color
            
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            switch (port.fieldName)
            {
                case "input":
                    _currentChildIndex = -1;
                    break;
                case "output":
                    // 동적 포트 리스트의 값을 반환
                    return output;
            }

            return null; // Replace this
        }
    }
}