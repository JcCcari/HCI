using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AssemblyCSharp;

public class Waiting : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (AssemblyCSharp.PlayerInfo.Instance.currentOption == 8){
            SceneManager.LoadScene("game");
        }
	}
}
