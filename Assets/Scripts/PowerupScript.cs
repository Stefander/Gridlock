using UnityEngine;
using System.Collections;

public class PowerupScript : MonoBehaviour 
{
	private int powerupType;
	public AudioClip pickupSound;
	private float spinSpeed = 15.0f;
	public Texture[] powerTextures;
	
	// Use this for initialization
	void Start () 
	{
		powerupType = Random.Range(0,powerTextures.Length);
		transform.renderer.material.mainTexture = powerTextures[powerupType];
	}
	
	void OnTriggerEnter(Collider other)
	{
		PlayerScript p;
		if((p=other.transform.GetComponent<PlayerScript>()) != null)
		{
			AudioSource.PlayClipAtPoint(pickupSound, transform.position);
			p.Pickup(powerupType);
		}
		Destroy (transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(0,Time.deltaTime*spinSpeed,0,Space.World);
	}
}
