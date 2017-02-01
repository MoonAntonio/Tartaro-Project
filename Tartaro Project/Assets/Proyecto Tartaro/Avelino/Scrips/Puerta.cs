using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puerta : MonoBehaviour {
   
    public Animator Animacion;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0)) Animacion.SetBool("abierta", true);
        if (Input.GetKeyUp(KeyCode.Mouse0)) Animacion.SetBool("abierta", false);
    }
}
