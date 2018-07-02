using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AssemblyCSharp;

public class touchEvents : MonoBehaviour {
    ParticleSystem plasmaAttack;   // attack particle
    ParticleSystem objectSelected; // circle particle

    // Use this for initialization
    void Start () {
        plasmaAttack = GameObject.Find("Afterburner").GetComponentInChildren<ParticleSystem>();
        //objectSelected = GameObject.Find("").GetComponentInChildren<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {

        if ( (Input.GetTouch(0).phase == TouchPhase.Stationary) || (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(0).deltaPosition.magnitude <1.2f))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if( Physics.Raycast(ray, out hit))
            {
                Debug.Log("##### HITTTTTTTTTTT touch"+ AssemblyCSharp.PlayerInfo.Instance.ObjectsTouched.Count);
                
                AssemblyCSharp.PlayerInfo.Instance.select(hit.transform.gameObject.name);

                if (AssemblyCSharp.PlayerInfo.Instance.ObjectsTouched.Count >=2)
                {
                    Debug.Log("##### ATACK");
                    //plasmaAttack = GameObject.Find("Afterburner").GetComponentInChildren<ParticleSystem>();
                    plasmaAttack.Play();
                    //AssemblyCSharp.PlayerInfo.Instance.attack();
                }
                
            }
            else{
                plasmaAttack.Stop();
            }
        }
    }
}
