using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveShadows : MonoBehaviour
{
    private void Awake() {
        GetComponent<UnityEngine.Tilemaps.TilemapRenderer>().receiveShadows = true;
        GetComponent<UnityEngine.Tilemaps.TilemapRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
