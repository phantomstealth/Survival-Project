using UnityEngine;

public class Target : MonoBehaviour {

	public float health=50.0f;
    public string NameObject = "";

	public GameObject Explosive;
	public GameObject ImpactEffect;

	AudioSource m_MyAudioSource;
	public AudioClip ExplosiveGround;
	public AudioClip ExplosiveMetall;
	public AudioClip SoundHit;

	void Start()
	{
		//m_MyAudioSource = GetComponent<AudioSource> ();
		m_MyAudioSource=gameObject.AddComponent<AudioSource>();
	}
	

	public void TakeDamage (float amount)
	{
		if (health<999999) health -= amount;
		m_MyAudioSource.PlayOneShot (SoundHit);
		if (health <= 0f) 
		{
			Die ();
		}
	}

	public void Die()
	{
		Instantiate (Explosive, transform.position,transform.rotation);
		m_MyAudioSource.PlayOneShot (ExplosiveMetall);
        health = 50.0f;
		//Destroy (gameObject);
	}
}
