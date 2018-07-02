using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clickAnimationObject : MonoBehaviour {
    public Animator animator;
    // Use this for initialization
    void Start () {
        animator.GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                animator.Play("pos_ultimatedragon_animation");
                Debug.Log(" HITTTTTTTTTTT mouse");
            }
            else{
                animator.Play("none");
                Debug.Log(" WRONGGG mouse");
            }
        }
#elif UNITY_ANDROID
        if ( (Input.GetTouch(0).phase == TouchPhase.Stationary) || (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(0).deltaPosition.magnitude <1.2f))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if( Physics.Raycast(ray, out hit))
            {
                animator.Play("pos_ultimatedragon_animation");
                Debug.Log(" HITTTTTTTTTTT touch");
            }else{
                animator.Play("none");
            }
        }
#endif
    }
}
