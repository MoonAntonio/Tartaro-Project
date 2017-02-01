using UnityEngine;
using System.Collections;

public class Rota : MonoBehaviour {
    public float velocidad = 3;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, Time.deltaTime * velocidad, 0));
	}
}
