using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using AssemblyCSharp; // in ClientData.cs

public class Client : MonoBehaviour {
    bool connected = false;
    InputField ipField = null;
    InputField portField = null;
    Button connectBtn = null;
    int port = 8888;
    int option = 0; // 0:player     1:viewer

    // Use this for initialization
    void Start () {
        ipField = (InputField)GameObject.Find("IpInputField").GetComponent<InputField>();
        ipField.text = "127.0.0.1";
        portField = (InputField)GameObject.Find("PortInputField").GetComponent<InputField>();
        portField.text = "8000";

        connectBtn = (Button)GameObject.Find("ConnectButton").GetComponent<Button>();
        connectBtn.onClick.AddListener(delegate { AssemblyCSharp.PlayerInfo.Instance.startConnection(this.portField.text, this.port, this.option); });
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S) && !connected)
        {
            Int32.TryParse(this.portField.text, out port);
            connected = AssemblyCSharp.PlayerInfo.Instance.startConnection( this.ipField.text, port, option);
        }

        AssemblyCSharp.PlayerInfo.Instance.sendPlay();
    }
}
