using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp; // in ClientData.cs

public class ClientScript : MonoBehaviour {
    bool connected = false;
    InputField ipField = null;
    InputField portField = null;

    // Use this for initialization
    void Start () {
        ipField = (InputField)GameObject.Find("IpInputField").GetComponent<InputField>();
        IPField.text = "127.0.0.1";
        portField = (InputField)GameObject.Find("PortInputField").GetComponent<InputField>();
        PuertoField.text = "8000";
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S) && !connected)
        {
            int port;
            Int32.TryParse(PortInputField.text, out port);
            int option = 0;
            connected = AssemblyCSharp.PlayerInfo.Instance.startConnection( ipField.text, port, option);
        }

        AssemblyCSharp.PlayerInfo.Instance.sendPlay();
    }
}
