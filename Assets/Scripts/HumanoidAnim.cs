using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidAnim : MonoBehaviour {

    private Animator animGo;

    // Use this for initialization
    void Start () {
        animGo = GetComponent<Animator>();
    }

    void Animation_Attack()
    {
        animGo.SetTrigger("attack");
    }

    void Animation_Run()
    {
        animGo.SetTrigger("run");
    }
    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Animation_Attack();
        if (Input.GetKeyDown(KeyCode.R))
            Animation_Run();
    }

}
