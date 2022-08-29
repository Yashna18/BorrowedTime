using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    public int selfLayerIndex;

    public Vector2 GetTargetCoord(GameObject target) {
        if (IsPathUnoccluded(gameObject, target)) {
            // If target visible, return target coord. 
            return target.transform.position;
        }

        GameObject[] nodeObjects = GameObject.FindGameObjectsWithTag("NavNode");

        List<NavNode> navNodes = new List<NavNode>();
        List<NavNode> openList = new List<NavNode>();
        List<NavNode> closedList = new List<NavNode>();

        // List of all nodes
        foreach (GameObject node in nodeObjects) {
            NavNode n = new NavNode();
            n.gameObject = node;
            n.g = float.PositiveInfinity;
            n.h = GetObjectDistance(node, target);
            navNodes.Add(n);
        }

        // Also add target and this enemy as nodes.
        NavNode targetNode = new NavNode();
        targetNode.gameObject = target;
        targetNode.g = float.PositiveInfinity;
        targetNode.h = 0f;
        navNodes.Add(targetNode);

        // This gameobject as a node
        NavNode rootNode = new NavNode();
        rootNode.gameObject = gameObject;
        rootNode.g = 0; rootNode.h = 0;
        closedList.Add(rootNode); // Pre-added to closed list

        NavNode nodeQ = closedList[0];

        // Init open list
        for (int i = navNodes.Count - 1; i >= 0; i--) {
            NavNode node = navNodes[i];

            if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
                node.g = GetObjectDistance(nodeQ.gameObject, node.gameObject);
                node.parent = nodeQ;
                openList.Add(node);
                navNodes.Remove(node);
            }
        }

        // As long as there are nodes in the open list
        while (openList.Count > 0) {
            openList.Sort();
            nodeQ = openList[0];
            openList.Remove(nodeQ);

            // Check each already in closedList in case
            for (int i = closedList.Count - 1; i >= 0; i--) {
                NavNode node = closedList[i];

                if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
                    if (node.g > nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject)) {
                        node.parent = nodeQ;
                        node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
                        openList.Add(node);
                        closedList.Remove(node);
                    }
                }
            }

            // Go through openList and update f
            for (int i = openList.Count - 1; i >= 0; i--) {
                NavNode node = openList[i];

                if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
                    if (node.g > nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject)) {
                        node.parent = nodeQ;
                        node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
                    }
                }
            }

            // Go through unopened nodes
            for (int i = navNodes.Count - 1; i >= 0; i--) {
                NavNode node = navNodes[i];

                if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
                    if (node == targetNode) {
                        node.parent = nodeQ;
                        node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
                        closedList.Add(nodeQ);
                        goto FoundTarget; // If target found, leave
                    } else {
                        node.parent = nodeQ;
                        node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
                        openList.Add(node);
                        navNodes.Remove(node);
                    }
                }
            }

            closedList.Add(nodeQ);
        }

        FoundTarget:
        closedList.Add(targetNode);

        NavNode lastNode = targetNode;
        List<NavNode> pathNodes = new List<NavNode>();
        NavNode nextNode = targetNode;

        while (lastNode != null) {
            if (lastNode.parent != null) {
                Debug.DrawLine(lastNode.gameObject.transform.position,
                            lastNode.parent.gameObject.transform.position,
                            Color.white,
                            0.1f);
                nextNode = lastNode;
            }

            pathNodes.Add(lastNode);
            lastNode = lastNode.parent;
        }

        // Debug.Log(pathNodes[pathNodes.Count - 2].gameObject.name);
        return nextNode.gameObject.transform.position;
    }

    public GameObject GetNearestShelter(GameObject self, GameObject player) {
        GameObject[] nodeObjects = GameObject.FindGameObjectsWithTag("NavNode");

        if (!IsPathUnoccluded(gameObject, player) || 
            nodeObjects.Length <= 0) {
            return self;
        }

        List<GameObject> nodeList = new List<GameObject>();

        foreach(GameObject node in nodeObjects) {
            if(!IsPathUnoccluded(node, player)) {
                nodeList.Add(node);
            }
        }

        nodeList.Sort(delegate (GameObject a, GameObject b) {
            return Vector2.Distance(self.transform.position, a.transform.position)
                .CompareTo(Vector2.Distance(self.transform.position, b.transform.position));
        });

        return nodeList[0];
    }

    //Get angle from one object to another
    public Vector2 GetObjectDirection(GameObject source, GameObject destination) {
        return (destination.transform.position - source.transform.position).normalized;
    }

    public Vector2 GetObjectDirection(Vector2 source, Vector2 destination) {
        return (destination - source).normalized;
    }

    public float GetObjectDistance(GameObject source, GameObject destination) {
        return Vector2.Distance(source.transform.position, destination.transform.position);
    }

    public float GetObjectDistance(Vector2 source, Vector2 destination) {
        return Vector2.Distance(source, destination);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns>Returns true if unoccluded. </returns>
    public bool IsPathUnoccluded(GameObject source, GameObject destination) {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        int layerMask = 1 << selfLayerIndex;
        layerMask = ~layerMask;

        RaycastHit2D hit = Physics2D.Raycast(source.transform.position, GetObjectDirection(source, destination), Mathf.Infinity, layerMask);
        if (hit.collider == null || hit.collider.gameObject == destination) {
            return true;
        } else {
            if (hit.distance > GetObjectDistance(source, destination)) {
                return true;
            } else {
                return false;
            }
        }
    }

    private class NavNode : System.IComparable<NavNode> {
        public GameObject gameObject { get; set; }
        public float g { get; set; }
        public float h { get; set; }
        public NavNode parent { get; set; }

        public float Getf() {
            return this.g + this.h;
        }

        public int CompareTo(NavNode other) {
            if (this.g + this.h == other.g + other.h) {
                return 0;
            } else if (this.g + this.h > other.g + other.h) {
                return 1;
            } else {
                return -1;
            }
        }
    }
}
