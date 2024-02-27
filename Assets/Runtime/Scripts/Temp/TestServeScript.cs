using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestServeScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Serve.serveID = 1;
        }
        else if (Input.GetKeyDown("2"))
        {
            Serve.serveID = 2;
        }
    }
}
