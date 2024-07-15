using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace X_Node
{
    public abstract class AIBaseNode : Node
    {
        [Input] public bool input;
        
        public ENodeStatus status { get; protected set; }
        
        // Use this for initialization
        protected override void Init()
        {
            base.Init();
        }

        // Return the correct value of an output port when requested
        public override object GetValue(NodePort port)
        {
            return null; // Replace this
        }

        public override void OnCreateConnection(NodePort from, NodePort to)
        {
            base.OnCreateConnection(from, to);
            
            // event connection
            if (from.fieldName == "output" && to.fieldName == "input")
            {
            }
        }
        
        [ContextMenu("Show Event Chain")]
        public void ShowEventChain()
        {
            Debug.Log(name);
            foreach (AIBaseNode node in GetAllChildNodes())
            {
                Debug.Log(node.name);
                (node).ShowEventChain();
            }
        }
        
        private IEnumerable<AIBaseNode> GetAllChildNodes()
        {
            // Get all connected nodes on the output port
            // using bfs, get all connected nodes
            
            Queue<AIBaseNode> queue = new Queue<AIBaseNode>();
            queue.Enqueue(this);
            
            while (queue.Count > 0)
            {
                AIBaseNode node = queue.Dequeue();
                foreach (NodePort port in node.Outputs)
                {
                    foreach (NodePort connection in port.GetConnections())
                    {
                        //if input port, skip
                        if (connection.fieldName == "output") continue;
                        
                        AIBaseNode connectedNode = (AIBaseNode)connection.node;
                        queue.Enqueue(connectedNode);
                        yield return connectedNode;
                    }
                }
            }
        }

        private IEnumerable<AIBaseNode> GetChildNodes()
        {
            return 
                from port in Outputs 
                from connection in port.GetConnections() 
                where connection.fieldName != "output" 
                select (AIBaseNode)connection.node;
        }

        public IEnumerator Evaluate()
        {
            return null;
        }
    }
    
    public enum ENodeStatus
    {
        SUCCESS,
        FAILURE,
        RUNNING
    }
}