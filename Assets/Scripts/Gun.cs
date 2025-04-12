using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class Gun : MonoBehaviour {

    public Texture2D aimBlack;
    public Texture2D aimSniper;
    public int zoomCamera = 10;
    int normalCamera = 60;
    //float smooth = 5;
    public bool isZoomed = false;

    public string NameWeapon;
    public int NumSlotWeapon;
    public float damage = 10f;
    public float range = 100f;
    public int bulletMax;
    public int bulletCur;
    public int bulletTotal;
    public float BulletForce = 1f;
    public float FireRate = 12f; //(Скорострельность)
    public float forceDropWeapon = 4f;
    public bool Auto;
    public bool SniperAim;
    private GameObject UIAim;
    private GameObject UIStatusBar;
    public float WaitForSecondShot = 0.6f;

    private bool shot = false;
    private bool reload = false;
    private bool draw = false;
    private int CurrentPunch = 1;

    private float nextTimeToFire = 0f;

    private Camera fpsCam;

    public GameObject DropWeapon;
    //public GameObject ImpactEffect;
    public GameObject MuzzleFlash;
    public Transform trMuzzleFlash;
    public GameObject dfltImpactEffect;

    string TagTakeObject;
    GameObject FullTakeObject;

    public AudioClip Fire;
    public AudioClip ClipOut;
    public AudioClip ClipIn;
    public AudioClip BullNull;
    AudioSource m_MyAudioSource;

    private Text TextNameObject;
    private Text txtBulletCurrent;
    private Animation animGo;
    private float SensitivityX;
    private float SensitivityY;
    public string NameTakeObject;
    private PlayerHealthManager PHM;

    void Start()
    {
        PHM = FindObjectOfType<PlayerHealthManager>();
        fpsCam = Camera.main;
        animGo = GetComponent<Animation>();
        GameObject tmpObject = GameObject.Find("Text_Name_Object");
        TextNameObject = tmpObject.GetComponent<Text>();
        tmpObject = GameObject.Find("CountBullet");
        txtBulletCurrent = tmpObject.GetComponent<Text>();
        //bulletCur = bulletMax;

        UIAim = FindObjectOfType<Extra_FirstController>().UIAim;
        UIStatusBar = FindObjectOfType<Extra_FirstController>().UIStatusBar;
        ChangeCurBullets();
        m_MyAudioSource = GetComponent<AudioSource>();
        AnimationIdle();
    }

    void EmptyTextNameObject()
    {
        TextNameObject.text = "";
        TagTakeObject = "";
        FullTakeObject = null;
    }

    void Verify_GameObject_Canvas()
    {
        int LayerMask = 1 << 5;
        LayerMask = ~LayerMask;
        RaycastHit hit;
        float Distance;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, 1000, 1))
        {
            Distance = Vector3.Distance(fpsCam.transform.position, hit.transform.position);
            if ((Distance <= 2.5f) & (hit.transform.tag == "Weapon"))
            {
                FullTakeObject = hit.transform.gameObject;
                TagTakeObject = hit.transform.GetComponent<PropertiesObject>().NameObject;
                TextNameObject.text = "Нажмите T чтобы взять " + hit.transform.GetComponent<PropertiesObject>().NameTextObject;

                NameTakeObject = hit.transform.name;
            }
            else if (hit.transform.tag == "Object")
            {
                TextNameObject.text = hit.transform.GetComponent<Target>().NameObject + " Дистанция " + (int)Distance + " метров. " + "хитов " + hit.transform.GetComponent<Target>().health;
                TagTakeObject = "";
            }
            else if (hit.transform.tag == "Monster")
            {
                TextNameObject.text = hit.transform.GetComponent<Skeleton_IQ>().NameObject + " Дистанция " + (int)Distance + " метров. " + "хитов " + hit.transform.GetComponent<Skeleton_IQ>().health;
                TagTakeObject = "";
            }
            else if (hit.transform.tag != "Untagged")
            {
                TextNameObject.text = hit.transform.GetComponent<PropertiesObject>().NameTextObject + " Дистанция " + (int)Distance + " метров.";
                TagTakeObject = "";
            }
            else
                EmptyTextNameObject();
        }
    }

    void OnGUI()
    {
        if (isZoomed)
        {
            GUI.DrawTexture(new Rect(Screen.width/2-Screen.height/2, 0, Screen.height, Screen.height), aimSniper);
            GUI.DrawTexture(new Rect(0, 0, Screen.width / 2 - Screen.height/2, Screen.height), aimBlack);
            GUI.DrawTexture(new Rect(Screen.width/2+Screen.height/2, 0, Screen.width/2-Screen.height/2, Screen.height), aimBlack);
        }
    }

    public void Verify_Zoom()
    {
        if (isZoomed)
        {
            fpsCam.GetComponent<Camera>().fieldOfView = zoomCamera;
            FindObjectOfType<FirstPersonController>().m_MouseLook.XSensitivity = 0.1f;
            FindObjectOfType<FirstPersonController>().m_MouseLook.YSensitivity = 0.1f;
            UIAim.SetActive(false);
            UIStatusBar.SetActive(false);
        }
        else
        {
            fpsCam.GetComponent<Camera>().fieldOfView = normalCamera;
            FindObjectOfType<FirstPersonController>().m_MouseLook.XSensitivity = 2;
            FindObjectOfType<FirstPersonController>().m_MouseLook.YSensitivity = 2;
            UIAim.SetActive(true);
            UIStatusBar.SetActive(true);
        }
    }

    void Verify_Controller_Player()
    {
        float FOV_Wheel=0;
        if (isZoomed)
            if (Input.GetAxis("Mouse ScrollWheel")!=0)
            {
                FOV_Wheel = fpsCam.fieldOfView - Input.GetAxis("Mouse ScrollWheel");
                if (FOV_Wheel>0.2f&FOV_Wheel<(normalCamera))
                    fpsCam.fieldOfView = fpsCam.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * 2;
            }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (TagTakeObject != "")
            {
                //FindObjectOfType<Extra_FirstController>().Take_Weapon(TagTakeObject, NameTakeObject);
                FindObjectOfType<Extra_FirstController>().Take_Weapon(FullTakeObject);

            }
        }

        if (gameObject.tag == "Hand")//Если текущая рука никаких действий не предпринимает рука пока ничего не может
        {
            if (Input.GetButtonDown("Fire1")) Fist_Punch();
            return;
        }
        else if (gameObject.tag == "Hand Weapon")
        {
            if (Input.GetButtonDown("Fire1")) Splash_Weapon();
            return;
        }

        if (SniperAim & Input.GetButtonDown("Fire2"))
        {
            isZoomed = !isZoomed;
            Verify_Zoom();    
        }

        if (gameObject.tag == "Auto")
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && bulletCur > 0)
            {
                nextTimeToFire = Time.time + 1f / FireRate;
                Shot();
                txtBulletCurrent.text = bulletCur+"";
            }
        }

        else if (Input.GetButtonDown("Fire1") && bulletCur > 0)
        {
            Shot();
            ChangeCurBullets();
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && bulletCur == 0)
        {
            m_MyAudioSource.PlayOneShot(BullNull);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Press R");
            Reload_Weapon();
            m_MyAudioSource.PlayOneShot(ClipOut);
            m_MyAudioSource.PlayOneShot(ClipIn);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if ((Auto) & (gameObject.tag != "Auto"))
            {
                gameObject.tag = "Auto";
            }
            else if ((Auto) & (gameObject.tag == "Auto"))
            {
                gameObject.tag = "Single";
            }

        }
    }

	void Update () 
	{
		Verify_GameObject_Canvas ();
        Verify_Controller_Player();
	}
    
    void ChangeCurBullets()
    {
        GameObject PlayerWeapon = FindObjectOfType<Extra_FirstController>().PlayerWeapon[NumSlotWeapon];
        PlayerWeapon.GetComponent<Gun>().bulletCur = bulletCur;
        if (bulletMax == 0)
            txtBulletCurrent.text = "";
        else
            txtBulletCurrent.text = bulletCur+"";
    }

    void emptyWeapon()
    { 
    
    }

	void Reload_Weapon()
	{
        if (bulletTotal == 0)
        {
            emptyWeapon();
            return;
        }
        int bullCache = 0;
        bullCache = bulletMax - bulletCur;
        bulletCur += bullCache;
        bulletTotal -= bullCache;
		txtBulletCurrent.text =bulletCur+"";
	}

    IEnumerator WaitShot(float TimeSecond)
    {
        yield return new WaitForSeconds(TimeSecond);
        shot = false;
        AnimationIdle();
    }

    void AnimationIdle()
    {
        animGo.CrossFadeQueued("idle", 0.08f, QueueMode.PlayNow);
    }

    void Splash_Weapon()
    {
        if (shot||PHM.Stamina<5) return;
        m_MyAudioSource.PlayOneShot(Fire);
        if (CurrentPunch == 1)
        {
            CurrentPunch = 2;
            animGo.CrossFade("splash");
        }
        else
        {
            CurrentPunch = 1;
            animGo.CrossFade("splash");
        }
        shot = true;
        PHM.Stamina -= 5f;
        StartCoroutine(WaitShot(WaitForSecondShot));
    }

    //Вызывается из анимации
    public void Raycast_Weapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {

        }
        if (hit.transform == null)
            return;
        if (hit.transform.tag == "Object")
        {
            hit.transform.GetComponent<Target>().TakeDamage(damage);
        }
        else if (hit.transform.tag == "Monster")
        {
            hit.transform.GetComponent<Skeleton_IQ>().TakeDamage(damage);
        }

        GameObject ImpactEffect;
        if (hit.rigidbody != null)
        {
            hit.rigidbody.AddForce(-hit.normal * BulletForce);
        }
        if (hit.transform.GetComponent<Target>() != null)
        {
            ImpactEffect = hit.transform.GetComponent<Target>().ImpactEffect;
        }
        else ImpactEffect = dfltImpactEffect;
        GameObject ImpactGO = Instantiate(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(ImpactGO, 2f);
    }


    void Fist_Punch()
    {
        if (shot||PHM.Stamina<5) return;
        m_MyAudioSource.PlayOneShot(Fire);
        if (CurrentPunch == 1)
        {
            CurrentPunch = 2;
            animGo.CrossFadeQueued("fist_punch", 0.08f, QueueMode.PlayNow);
        }
        else
        {
            CurrentPunch = 1;
            animGo.CrossFadeQueued("fist_punch2", 0.08f, QueueMode.PlayNow);
        }
        shot = true;
        StartCoroutine(WaitShot(WaitForSecondShot));
        PHM.Stamina -= 3f;
    }

    void Shot()
	{
        if (shot || reload || draw) return;

        animGo.CrossFadeQueued("fire", 0.08f, QueueMode.PlayNow);
        shot = true;
        StartCoroutine(WaitShot(WaitForSecondShot));

        bulletCur -= 1;
        RaycastHit hit;

        m_MyAudioSource.PlayOneShot(Fire);
		GameObject instMuzzleFlash = Instantiate (MuzzleFlash, trMuzzleFlash.position,trMuzzleFlash.rotation);
		instMuzzleFlash.transform.parent = transform;
		if (Physics.Raycast (fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
		{
			
		}
		if (hit.transform==null) 
			return;
        if (hit.transform.tag == "Object")
        {
            hit.transform.GetComponent<Target>().TakeDamage(damage);
        }
        else if (hit.transform.tag == "Monster")
        {
            hit.transform.GetComponent<Skeleton_IQ>().TakeDamage(damage);
        }

        GameObject ImpactEffect;
		if (hit.rigidbody != null) 
		{
			hit.rigidbody.AddForce (-hit.normal*BulletForce);
		}
		if (hit.transform.GetComponent<Target> ()!= null) 
		{
			ImpactEffect = hit.transform.GetComponent<Target> ().ImpactEffect;
		}
		else ImpactEffect=dfltImpactEffect;
		GameObject ImpactGO = Instantiate (ImpactEffect, hit.point, Quaternion.LookRotation (hit.normal));
		Destroy (ImpactGO, 2f);
	}
}
