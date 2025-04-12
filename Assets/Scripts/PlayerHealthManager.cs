using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    private float intHealth = 100;
    private float intStamina = 100;
    private GameObject HealthText;
    private GameObject StaminaText;

    public AudioClip humanHurt;
    public AudioClip humadDie;
    private AudioSource m_MyAudioSource;
    private bool bltakeDamage=false;

    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController FPController;

    public float Health
    {
        get { return intHealth; }
        set
        {
            if (bltakeDamage) return;
            if (value < 0)
            {
                value = 0;
                m_MyAudioSource.PlayOneShot(humadDie);
            }
            else if (value < intHealth)
            {
                bltakeDamage = true;
                m_MyAudioSource.PlayOneShot(humanHurt);
                StartCoroutine(IIHaveDamage(1.0f));
            }
            if (value > 100) value = 100;
            intHealth = value;
            HealthText.GetComponent<Slider>().value = intHealth;
        }
    }

    public float Stamina
    {
        get { return intStamina; }
        set
        {
            if (value < 0) value = 0;
            if (value > 100) value = 100;
            intStamina = value;
            StaminaText.GetComponent<Slider>().value = intStamina;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        HealthText = GameObject.FindGameObjectWithTag("HealthText");
        StaminaText = GameObject.FindGameObjectWithTag("StaminaText");
        FPController = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        m_MyAudioSource = GetComponent<AudioSource>();

    }

    IEnumerator IIHaveDamage(float TimeSecond)
    {
        yield return new WaitForSeconds(TimeSecond);
        bltakeDamage = false;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        VerifyMove(FPController.Move_Verify,FPController.Speed_Verify);
    }
    void VerifyMove(Vector3 m_MoveDir, float speed)
    {
        if (m_MoveDir.x != 0 || m_MoveDir.y != -10)
        {
            Stamina = Stamina - speed * 0.005f;
        }
        else
        {
            Stamina = Stamina + 0.05f;
        }
    }
}
