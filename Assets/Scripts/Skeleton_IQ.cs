using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_IQ : MonoBehaviour
{
    public GameObject[] Inventory = new GameObject[3];

    public string NameObject;
    public float health = 50.0f;
    public float defaultSpeed = 0.1f;
    public float defaultSpeedRun = 0.8f;
    public float DistanceForRun = 6;
    public float DistanceAttak = 2f;
    public float TimeforSleep = 0.2f;


    public AudioClip WalkMonster;
    public AudioClip IdleMonster;
    public AudioClip AttackMonster;
    public AudioClip sndHit;
    public AudioClip sndDie;
    public AudioClip sndDoDamage;
    AudioSource source;


    float gravity = 20f;
    public float speed = 0;
    public bool blMonster_Attack = false;
    bool blMonster_Fall = false;


    bool EnemySeePlayer = false;
    Vector3 direction;
    Transform TargetDirection;
    CharacterController controller;

    Animator animator;
    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        //anim.SetBool("walk", true);     
        PlayAudio(WalkMonster, true);
        GameObject tempObject = GameObject.FindGameObjectWithTag("Player");
        if (tempObject!=null)  TargetDirection = tempObject.transform;
        StartCoroutine(IIMonsterWait(TimeforSleep));
    }

    // Update is called once per frame
    void OnPlayerEnter()
    {
        Debug.Log("Вас заметил " + NameObject);
        speed = defaultSpeedRun;
        EnemySeePlayer = true;
    }

    void OnPlayerExit()
    {
        Debug.Log("Вы вышли из зоны " + NameObject);
        speed = 0;
        animator.SetFloat("speed", speed);
        EnemySeePlayer = false;
    }

    void PlayAudio(AudioClip clip, bool loop)
    {
        source.loop = loop;
        source.clip = clip;
        source.Play();
    }

    //IEnumerator Animation(string NameAnim)
    //{
    //    anim.SetBool(NameAnim,true);
    //    PlayAudio(AttackMonster,false);
    //    yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    //    anim.SetBool(NameAnim, false);
    //}

    void Monster_Attack()
    {
        if (!blMonster_Attack)
        {
            blMonster_Attack = true;
            RotateForPlayer(true);
            speed = 0;
            animator.SetTrigger("attack");
            PlayAudio(AttackMonster, false);
            StartCoroutine(IIMonsterAttack(1.333f));
        }
    }

    IEnumerator IIMonsterAttack(float TimeSecond)
    {
       yield return new WaitForSeconds(TimeSecond);
        blMonster_Attack = false; 
    }

    IEnumerator IIMonsterWait(float TimeSecond)
    {
        while (true)
        {
            MoveMonster();
            yield return new WaitForSeconds(TimeSecond);
        }
    }

    void MoveMonster()
    {
        if (blMonster_Fall)
        {
            RotateForPlayer(false);
            return;
        }
        float Distance = 100f;
        if (TargetDirection != null)
        {
            Distance = Vector3.Distance(transform.position, TargetDirection.transform.position);
        }
        if ((Distance > 50) & (EnemySeePlayer))
            OnPlayerExit();
        else if ((Distance <= 50) & (!EnemySeePlayer))
        {
            OnPlayerEnter();
        }
        if (Distance < 50)
        {
            //Поворачиваемсся на игрока
            RotateForPlayer(false);
            if (Distance > DistanceForRun) speed=defaultSpeedRun;
            if ((Distance<=DistanceForRun-1)&(!blMonster_Attack)) speed=defaultSpeed;
            if (Distance < DistanceAttak) Monster_Attack();
            animator.SetFloat("speed", speed);
        }
        if (speed > 0)
        {
            direction = direction * speed;
            direction.y -= gravity * Time.deltaTime;
            controller.Move(direction * Time.deltaTime);
        }
        else
        {
            if ((animator.GetCurrentAnimatorStateInfo(0).IsName("idle-walk")) & EnemySeePlayer&!blMonster_Attack)
            {
                speed = defaultSpeed;
                animator.SetFloat("speed", speed);
                PlayAudio(WalkMonster, true);
            }
        }
    }

    void RotateForPlayer(bool Instantly)
    {
        direction = TargetDirection.transform.position - transform.position;
        direction.y = 0;
        if (Instantly)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), 3f);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
    }

    public void TakeDamage(float amount)
    {
        if (health < 0f) return;
        if (health < 999999) health -= amount;
        if (health <= 0f)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("hit");
            PlayAudio(sndHit, false);
            speed = 0;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
    }

    public void Die()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("fall"))
        {
            animator.SetTrigger("fall");
            blMonster_Fall = true;
            PlayAudio(sndDie, false);
            Destroy(gameObject, 5f);
            speed = 0;
        }
    }

    private void OnDestroy()
    {
       if (health<=0) DropInventory();
    }

    void DropInventory()
    {
        GameObject DropObject=null;
        int i;
        for (i = 0; i <= 2; i++)
        {
            if (Inventory[i] != null)
                DropObject = Instantiate(Inventory[i], new Vector3(transform.position.x,3,transform.position.z), transform.rotation);
        }
    }

}
