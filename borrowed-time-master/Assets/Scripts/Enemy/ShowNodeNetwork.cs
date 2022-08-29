using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowNodeNetwork : MonoBehaviour
{
    public int selfLayerIndex;

    GameObject[] nodeObjects;

    // Start is called before the first frame update
    void Start()
    {
        nodeObjects = GameObject.FindGameObjectsWithTag("NavNode");

        //selfLayerIndex = ~((1 << 9) | (1 << 12));
        //selfLayerIndex = (1 << 9) | (1 << 12);
    }

    private void OnDrawGizmos() {
        nodeObjects = GameObject.FindGameObjectsWithTag("NavNode");
        for (int i = 0; i < nodeObjects.Length; i++) {
            for (int j = 0; j < nodeObjects.Length; j++) {
                if (i != j && IsPathUnoccluded(nodeObjects[i], nodeObjects[j])) {
                    //Debug.DrawLine(nodeObjects[i].transform.position,
                    //        nodeObjects[j].transform.position,
                    //        Color.white,
                    //        10000000f);
                    Gizmos.DrawLine(nodeObjects[i].transform.position, 
                        nodeObjects[j].transform.position);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetObjectDistance(GameObject source, GameObject destination) {
        return Vector2.Distance(source.transform.position, destination.transform.position);
    }

    public Vector2 GetObjectDirection(GameObject source, GameObject destination) {
        return (destination.transform.position - source.transform.position).normalized;
    }

    public bool IsPathUnoccluded(GameObject source, GameObject destination) {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        int layerMask = 1 << selfLayerIndex;
        layerMask = ~layerMask;
        //int layerMask = ~selfLayerIndex;

        RaycastHit2D hit = Physics2D.Raycast(source.transform.position, 
                                            GetObjectDirection(source, destination), 
                                            Mathf.Infinity, 
                                            layerMask);
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
}
