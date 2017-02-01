using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrluzfuera : MonoBehaviour {

    public GameObject Luzin;
    public GameObject luzex;
    // Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        luzex.SetActive(true);
        Luzin.SetActive(false);
    }
}
