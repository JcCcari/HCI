using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using AssemblyCSharp;

public class ScenesManager : MonoBehaviour {

	public void changeScene(){
        InputField ipField = (InputField)GameObject.Find("IpInputField").GetComponent<InputField>();
        InputField portField = (InputField)GameObject.Find("PortInputField").GetComponent<InputField>();
        int port;
        Int32.TryParse(portField.text, out port);

        int option = 0;
      
        bool connected = AssemblyCSharp.PlayerInfo.Instance.startConnection(ipField.text, port, option);

        SceneManager.LoadScene("game");
        /* 
         // Descomentar cuando waiting game este completo
        if (connected){
            if (option == 1)
                SceneManager.LoadScene("waiting");
            else
                SceneManager.LoadScene("main");
        }
        else
            Debug.Log("Imposible conectar al servidor, pruebe otra vez PE");
        */
    }
}
