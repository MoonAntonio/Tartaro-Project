using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lucesdentro : MonoBehaviour {
    public GameObject Luzin;
    public GameObject luzex;
    // Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        luzex.SetActive(false);
        Luzin.SetActive(true);
    }
}
