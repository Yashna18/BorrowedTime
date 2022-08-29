using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcRenderer : MonoBehaviour
{
    public bool useLinearArcElseQuadratic;

    public int arcSegments;
    public int throwPower;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float GetQuadraticCoordinates(float t) {
        float p0 = GameObject.Find("Player").transform.position.y;
        float p1 = (Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        float difference = -(GameObject.Find("Player").transform.position.y - Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

        if (difference < 0) {
            difference = 0;
        }
 
        float c0 = GameObject.Find("Player").transform.position.y + 3.0f + difference;

        return Mathf.Pow(1-t, 2)*p0 + 2*t*(1-t)*c0 + Mathf.Pow(t, 2)*p1;
    }

    void RenderQuadraticArc(LineRenderer lineRenderer) {
        lineRenderer.positionCount = arcSegments;

        for (int i = 0; i < arcSegments; i++) {
            float t = (float)(i)/(arcSegments);
            
            float playerY = GameObject.Find("Player").transform.position.y;
            float playerX = GameObject.Find("Player").transform.position.x;

            float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            float distanceToPlayer = (GameObject.Find("Player").transform.position.x - mouseX) * -1;

            float x = GameObject.Find("Player").transform.position.x + distanceToPlayer/arcSegments*(i);
            float finalX = Mathf.Clamp(x, playerX-throwPower, playerX+throwPower);
            float y = GetQuadraticCoordinates(t);
            float finalY = Mathf.Clamp(y, playerY-throwPower, playerY+throwPower);

            lineRenderer.SetPosition(i, new Vector3(finalX, finalY, -5));
        }
    }

    void RenderLinearArc(LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, GameObject.Find("Player").transform.position);

        Vector3 lineEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lineRenderer.SetPosition(1,  new Vector3(lineEnd.x, lineEnd.y, -5));
    }

    // Update is called once per frame
    void Update()
    {
        LineRenderer lineRenderer = this.GetComponent<LineRenderer>();

        if (PlayerManager.isThrowingTimeBomb == true) {
           this.GetComponent<Renderer>().enabled = true;

           if (useLinearArcElseQuadratic) {
               RenderLinearArc(lineRenderer);
           } else {
               RenderQuadraticArc(lineRenderer);
           }
        } else {
            this.GetComponent<Renderer>().enabled = false;
        }
    }
}
