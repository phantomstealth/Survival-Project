using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extra_FirstController : MonoBehaviour {

    // Use this for initialization
    public GameObject UIAim;
    public GameObject UIStatusBar;

    public GameObject[] PlayerWeapon=new GameObject[3];
	public GameObject[] TypeWeapon=new GameObject[10];
    public GameObject MainGun;

	public AudioClip DropGun;
	AudioSource m_MyAudioSource;

	void Start () 
	{
		m_MyAudioSource = GetComponent<AudioSource> ();
	}

    void Hear_Keyboard()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            MainGun.GetComponent<Gun>().isZoomed = false;
            MainGun.GetComponent<Gun>().Verify_Zoom();
            Drop_Weapon(MainGun.GetComponent<Gun>().NumSlotWeapon);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(2);
        }
    }

    // Update is called once per frame
    void Update () 
	{
        Hear_Keyboard();
	}


	public void ChangeWeapon(int numWeapon)
	{
        if (PlayerWeapon[numWeapon] == null) numWeapon=0;
        //Destroy(GameObject.Find ("CurrentWeapon"));
        Destroy(MainGun);
        MainGun = Instantiate (PlayerWeapon[numWeapon], PlayerWeapon[numWeapon].transform.position,PlayerWeapon[numWeapon].transform.rotation);
		MainGun.name = "CurrentWeapon";
		MainGun.transform.parent = Camera.main.transform;
		MainGun.transform.localPosition = PlayerWeapon[numWeapon].transform.localPosition;
		MainGun.transform.localRotation = PlayerWeapon[numWeapon].transform.localRotation;
		MainGun.SetActive (true);
	}


    public void Drop_Weapon(int numWeapon)
    {
        if (PlayerWeapon[numWeapon].tag == "Hand")
        {
            Debug.Log("Руки нельзя выкинуть)))");
            return;
        }
        if (!PlayerWeapon[numWeapon].GetComponent<Gun>().DropWeapon) return;
        SndDrop_Weapon();
        Rigidbody rb;
        //Создаем оружие которое будет выкидывать.
        GameObject Weapon = Instantiate(PlayerWeapon[numWeapon].GetComponent<Gun>().DropWeapon,MainGun.GetComponent<Gun>().transform.position+gameObject.transform.forward, Camera.main.transform.rotation);
        Weapon.GetComponent<PropertiesObject>().BulletCurrent = MainGun.GetComponent<Gun>().bulletCur;
        rb = Weapon.GetComponent<Rigidbody>();
        //rb.AddForce(Camera.main.transform.up * MainGun.GetComponent<Gun>().forceDropWeapon, ForceMode.Impulse);
        rb.AddForce(gameObject.transform.forward * MainGun.GetComponent<Gun>().forceDropWeapon, ForceMode.Impulse);
        if (numWeapon != 0)
            PlayerWeapon[numWeapon] = null;
        else
            PlayerWeapon[0] = TypeWeapon[0];
        ChangeWeapon(0);
    }

    //public void Take_Weapon(string nameWeapon, string nameDropWeapon)
    public void Take_Weapon(GameObject FullObjectWeapon)
	{
        string nameWeapon = FullObjectWeapon.GetComponent<PropertiesObject>().NameObject;

        for (int i = 0; i <= 9; i++) //проверяем все ячейти "тип оружия" если одна из них совпала то..
		{
			if (TypeWeapon[i]!=null)
				if (nameWeapon == TypeWeapon[i].name) 
				{
                    GameObject DropWeapon = FullObjectWeapon;// узнаем то что мы будем бросать
                    int numWeapon = TypeWeapon [i].GetComponent<Gun> ().NumSlotWeapon;
                    if (PlayerWeapon[numWeapon] != null)
                    {
                        Debug.Log(PlayerWeapon[numWeapon]);
                        ChangeWeapon(numWeapon);
                        Drop_Weapon(numWeapon);
                    }
				    PlayerWeapon[numWeapon] = TypeWeapon [i];
                    PlayerWeapon[numWeapon].GetComponent<Gun>().bulletCur = DropWeapon.GetComponent<PropertiesObject>().BulletCurrent;
                    PlayerWeapon[numWeapon].GetComponent<Gun>().NameWeapon = DropWeapon.GetComponent<PropertiesObject>().NameObject;
                    Destroy(DropWeapon);
                    ChangeWeapon(numWeapon);
				}
		}
	}
		

	void SndDrop_Weapon()
	{
		m_MyAudioSource.PlayOneShot (DropGun);
	}
}
