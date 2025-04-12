using UnityEngine;
using System.Collections;


[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
//AudioSource m_MyAudioSource;
//public AudioClip ExplosiveGround;
//public AudioClip ExplosiveMetall;


	public bool OnlyDeactivate;

	void Start()
	{
		//m_MyAudioSource=gameObject.AddComponent<AudioSource>();
		//m_MyAudioSource.PlayOneShot (ExplosiveGround);
	}

	void OnEnable()
	{
		StartCoroutine("CheckIfAlive");
	}
	
	IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
				if(OnlyDeactivate)
				{
					#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
					#else
						this.gameObject.SetActive(false);
					#endif
				}
				else
					GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
