using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnemy : MonoBehaviour
{
    public GameObject ParentMonster;
    bool bl_Monster_attak;
    public float takeDamage = 3f;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player"& ParentMonster.GetComponent<Skeleton_IQ>().blMonster_Attack)
        {
            other.gameObject.GetComponent<PlayerHealthManager>().Health -= takeDamage;
        }
    }

}
