using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScr : MonoBehaviour
{
    [SerializeField]
    GameObject test;

    // Start is called before the first frame update
    void Start()
    {
        test.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
