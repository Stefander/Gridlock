using UnityEngine;
using System.Collections;

public class BombScript : MonoBehaviour 
{
	private CubeScript target;
	private float explodeTime = 3.0f;
	private float currentTime;
	private PlayerScript player;
	private float cubeSize;
	private int strength;
	private Ray[] checkRays;
	private Vector3 bombAxis;
	
	// Use this for initialization
	void Start () 
	{
		checkRays = new Ray[4];
		Vector3 deltaPos = bombAxis/2;
		Vector3 rayPosition = new Vector3(transform.position.x+deltaPos.x,transform.position.y+deltaPos.y,transform.position.z+deltaPos.z);
		checkRays[0] = new Ray(rayPosition,transform.forward);
		checkRays[1] = new Ray(rayPosition,-transform.forward);
		checkRays[2] = new Ray(rayPosition,transform.right);
		checkRays[3] = new Ray(rayPosition,-transform.right);
	}
	
	void OnTriggerExit()
	{
		transform.GetComponent<SphereCollider>().isTrigger = false;
	}
	
	public void AttachToCube(PlayerScript p, Vector3 axis, float cSize, GameObject cube)
	{
		cubeSize = cSize;
		bombAxis = axis;
		player = p;
		strength = player.bombStrength;
		target = cube.GetComponent<CubeScript>();
		currentTime = 0;
	}
	
	void Explode()
	{
		Destroy(gameObject);
		
		if(target != null)
		{
			player.RegisterDestroyed(target.DestroyCubes(strength));
			
			for(int i=0; i<4; i++)
			{
				RaycastHit hit;
				if(Physics.Raycast(checkRays[i],out hit,cubeSize))
				{
					PlayerScript p;
					CubeScript c;
					if((p=hit.collider.GetComponent<PlayerScript>()) != null)
						p.Kill();
					
					if((c=hit.collider.GetComponent<CubeScript>()) != null)
						player.RegisterDestroyed(c.DestroyCube());
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTime+=Time.deltaTime;
		float bombScale = (Mathf.Sin(currentTime)+1)/2;
		transform.localScale = Vector3.one*bombScale;
		
		if(currentTime >= explodeTime)
			Explode();
	}
}
