using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class AttackEvent : MonoBehaviour, IVirtualButtonEventHandler {
    public GameObject attackButton;
    public Animator dragonAnimator;
	// Use this for initialization
	void Start () {
        attackButton = (GameObject)GameObject.Find("AttackVirtualButton");
        attackButton.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);

        dragonAnimator.GetComponent<Animator>();
	}

    public void OnButtonPressed(VirtualButtonBehaviour virtualButton)
    {
        //dragonAnimator.Play("attack_animation_ultimate");
        dragonAnimator.Play("rotation_dragon_animation");

        Debug.Log("START Rotation Dragon!!!");
    }

    public void OnButtonReleased(VirtualButtonBehaviour virtualButton)
    {
        dragonAnimator.Play("none");
        Debug.Log("FINISH Rotation Dragon!!!");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
