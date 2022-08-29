using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
	public int layerMaskIndex;
	public float chaseSpeed;
	public float chaseDistance;
	public float chaseUpdateTime;
	float chaseTimer;

	protected GameObject player;
	Rigidbody2D rb2D;

	Vector2 targetcoord;

	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("Pathfinder is ready and activated!");

		if (GameObject.Find("Player") != null) {
			player = GameObject.Find("Player");
		}

		rb2D = gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;

		targetcoord = AStarFindPath();
		chaseTimer = UnityEngine.Random.value * chaseUpdateTime;
	}

	// Update is called once per frame
	void Update() {

		if(chaseTimer < 0) {
			targetcoord = AStarFindPath();
			chaseTimer = UnityEngine.Random.value * chaseUpdateTime;
        }
		chaseTimer -= Time.deltaTime;
        
		Vector2 sourcecoord = gameObject.transform.position;
        Debug.Log(targetcoord);

		rb2D.velocity = (targetcoord - sourcecoord).normalized * chaseSpeed;

		if(rb2D.velocity.x < 0) {
			gameObject.transform.localScale = new Vector3(-1, 1, 1);
        } else {
			gameObject.transform.localScale = new Vector3(1, 1, 1);
		}

	}

	private Vector2 AStarFindPath() {
		//Debug.Log("Starting Pathfinding!");

		if(IsPathUnoccluded(gameObject, player)) {
			Vector2 target = player.gameObject.transform.position;
			return target;
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
			n.h = GetObjectDistance(node, player);
			navNodes.Add(n);
		}

		// Also add player and this enemy as nodes.
		NavNode playerNode = new NavNode();
		playerNode.gameObject = player;
		playerNode.g = float.PositiveInfinity;
		playerNode.h = 0f;
		navNodes.Add(playerNode);
		//Debug.Log("All nodes:");
		//foreach (NavNode node in navNodes) {
		//	Debug.Log("Node: name= " + node.gameObject.name + ", g= " + node.g + ", h= " + node.h);
		//}

		NavNode rootNode = new NavNode();
		rootNode.gameObject = gameObject;
		rootNode.g = 0; rootNode.h = 0;
		closedList.Add(rootNode);

		//foreach (NavNode node in closedList) {
		//	Debug.Log("Node: name= " + node.gameObject.name + ", g= " + node.g + ", h= " + node.h);
		//}

		NavNode nodeQ = closedList[0];
		//Debug.Log("First Node: name= " + nodeQ.gameObject.name + ", g= " + nodeQ.g + ", h= " + nodeQ.h);
		//Debug.Log("Check Pathfinder to node1: " + IsPathUnoccluded(nodeQ.gameObject, navNodes[5].gameObject));

		// Init openList
		//foreach (NavNode node in navNodes) {
		for(int i = navNodes.Count - 1; i>=0; i--) {
			NavNode node = navNodes[i];

			if(IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
				node.g = GetObjectDistance(nodeQ.gameObject, node.gameObject);
				node.parent = nodeQ;
				openList.Add(node);
				navNodes.Remove(node);
			}
		}
		//Debug.Log("Init'd openList: ");
		//foreach (NavNode node in openList) {
		//	Debug.Log("Node: name= " + node.gameObject.name + ", g= " + node.g + ", h= " + node.h);
		//}
		//Debug.Log("Init'd navNodes: ");
		//foreach (NavNode node in navNodes) {
		//	Debug.Log("Node: name= " + node.gameObject.name + ", g= " + node.g + ", h= " + node.h);
		//}

		while (openList.Count > 0) {
			openList.Sort();
			nodeQ = openList[0];
			openList.Remove(nodeQ);

			//foreach(NavNode node in closedList) {
			for (int i = closedList.Count - 1; i >= 0; i--) {
				NavNode node = closedList[i];

				//Debug.Log(nodeQ.gameObject.name + ", " + node.gameObject.name);
				if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
					if (node.g > nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject)) {
						node.parent = nodeQ;
						node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
						openList.Add(node);
						closedList.Remove(node);
					}
				}
			}

			//foreach(NavNode node in openList) {
			for (int i = openList.Count - 1; i >= 0; i--) {
				NavNode node = openList[i];

				if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
					if(node.g > nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject)) {
						node.parent = nodeQ;
						node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
					}
				}
			}

			//foreach(NavNode node in navNodes) {
			for (int i = navNodes.Count - 1; i >= 0; i--) {
				NavNode node = navNodes[i];

				if (IsPathUnoccluded(nodeQ.gameObject, node.gameObject)) {
					if(node == playerNode) {
						node.parent = nodeQ;
						node.g = nodeQ.g + GetObjectDistance(nodeQ.gameObject, node.gameObject);
						closedList.Add(nodeQ);
						goto FoundPlayer; // Break out of loop
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

		// Break out of loop here
		FoundPlayer: 
		closedList.Add(playerNode);

		NavNode lastNode = playerNode;
		List<NavNode> pathNodes = new List<NavNode>();
		NavNode targetNode = playerNode;

		while(lastNode != null) {
			if (lastNode.parent != null) {
				Debug.DrawLine(lastNode.gameObject.transform.position,
							lastNode.parent.gameObject.transform.position,
							Color.white,
							0.5f);
				targetNode = lastNode;
			}

			pathNodes.Add(lastNode);
			lastNode = lastNode.parent;
		}

		return targetNode.gameObject.transform.position;
	}

	//Get direction to player
	Vector2 GetPlayerDirection() {
		Vector2 pos = this.transform.position;
		Vector2 target = player.transform.position;
		return (target - pos).normalized;
	}

	//Get distance to player
	float GetPlayerDistance() {
		return Vector2.Distance(this.transform.position, player.transform.position);
	}

	//Get angle from one object to another
	Vector2 GetObjectDirection(GameObject source, GameObject destination) {
		Vector2 sourcePos = source.transform.position;
		Vector2 targetPos = destination.transform.position;
		return (targetPos - sourcePos).normalized;
	}

	float GetObjectDistance(GameObject source, GameObject destination) {
		return Vector2.Distance(source.transform.position, destination.transform.position);
	}

	bool IsPathUnoccluded(GameObject source, GameObject destination) {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();

		int layerMask = 1 << layerMaskIndex;
		layerMask = ~layerMask;

		RaycastHit2D hit = Physics2D.Raycast(source.transform.position, GetObjectDirection(source, destination), Mathf.Infinity, layerMask);
		//RaycastHit2D hit = Physics2D.Raycast(source.transform.position, GetObjectDirection(source, destination));
		if (hit.collider == null || hit.collider.gameObject == destination) {
			return true;
		} else {
			if(hit.distance > GetObjectDistance(source, destination)) {
				return true;
			} else {
				return false;
			}
		}
	}

	private class NavNode : IComparable<NavNode> {
		public GameObject gameObject { get; set; }
		public float g { get; set; }
		public float h { get; set; }
		public NavNode parent { get; set; }

		public float Getf() {
			return this.g + this.h;
		}

		public int CompareTo(NavNode other) {
			// throw new NotImplementedException();

			if(this.g + this.h == other.g + other.h) {
				return 0;
			} else if (this.g + this.h > other.g + other.h) {
				return 1;
			} else {
				return -1;
			}
		}
	}

	private void TestNavNode() {
		NavNode n1 = new NavNode();
		NavNode n2 = new NavNode();
		NavNode n3 = new NavNode();
		n1.g = 4f; n1.h = 9f; //13
		n2.g = 2f; n2.h = 5f; //7
		n3.g = 9f; n3.h = 1f; //10

		List<NavNode> navNodes = new List<NavNode>();
		navNodes.Add(n1);
		navNodes.Add(n2);
		navNodes.Add(n3);

		Debug.Log("Pre-sort");
		foreach (NavNode node in navNodes) {
			Debug.Log(node.g);
		}

		navNodes.Sort();

		Debug.Log("Post-sort");
		foreach (NavNode node in navNodes) {
			Debug.Log(node.g);
		}
	}

	private void TestRayCaster() {
		rb2D = GetComponent<Rigidbody2D>();

		RaycastHit2D hit = Physics2D.Raycast(transform.position, GetPlayerDirection());
		if (hit.collider.gameObject == player) {
			Debug.Log("I have you now, scoundrel!");
		} else {
			Debug.Log("Better find the player!");
		}

		Debug.Log("Dist to collision: " + hit.distance);

		Debug.Log("I can see the player: " + IsPathUnoccluded(gameObject, player));
	}
}
