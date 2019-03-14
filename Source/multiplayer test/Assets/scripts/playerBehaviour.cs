using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class playerBehaviour : NetworkBehaviour
{
    void Update()
    {
        if (isLocalPlayer == true)
        {
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.Translate(Vector3.right * Time.deltaTime * 3f);
            
            }
        }
    }
}
