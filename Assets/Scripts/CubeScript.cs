using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeScript : MonoBehaviour 
{
	public Vector3 cubePos;
	public Vector3 localPos;
	private GameObject[] neighbours;
	public GameObject explosion;
	public GameObject powerupObject;
	private Ray[] rays;
	private float castLength = 1.0f;
	public bool invincible;
	
	// Use this for initialization
	void Start ()
	{
		rays = new Ray[8];
		cubePos = transform.position;
		rays[0] = new Ray(cubePos,Vector3.left);
		rays[1] = new Ray(cubePos,Vector3.right);
		rays[2] = new Ray(cubePos,Vector3.forward);
		rays[3] = new Ray(cubePos,Vector3.back);
		rays[4] = new Ray(cubePos,Vector3.down);
		rays[5] = new Ray(cubePos,Vector3.up);
	
		neighbours = new GameObject[6];
		
		CheckNeighbours();
	}
	
	public void destroyNeighbour(Vector3 axis)
	{
		if(axis == Vector3.right)
			neighbours[0] = null;
		
		else if(axis == Vector3.left)
			neighbours[1] = null;
		
		else if(axis == Vector3.back)
			neighbours[2] = null;
		
		else if(axis == Vector3.forward)
			neighbours[3] = null;
		
		else if(axis == Vector3.up)
			neighbours[4] = null;
		
		else if(axis == Vector3.down)
			neighbours[5] = null;
		else
			print ("Error, not a valid neighbour to destroy: "+axis);
	}
	
	public GameObject getNeighbour(Vector3 axis)
	{
		if(axis == Vector3.left)
			return neighbours[0];
		if(axis == Vector3.right)
			return neighbours[1];
		if(axis == Vector3.forward)
			return neighbours[2];
		if(axis == Vector3.back)
			return neighbours[3];
		if(axis == Vector3.down)
			return neighbours[4];
		if(axis == Vector3.up)
			return neighbours[5];
		
		print ("Error, not a valid neighbour: "+axis);
		return null;
	}
	
	public void SetPos(int i, int j, int k)
	{
		localPos = new Vector3(i,j,k);
	}
	
	public int BigDestroy()
	{
		int cubesDestroyed = 0;
		// Destroy the links in the neighbours to this cube
		for(int i=0; i<6; i++)
		{
			if(neighbours[i] != null)
				cubesDestroyed+=neighbours[i].GetComponent<CubeScript>().DestroyCube();
		}
		
		// Also, destroy self
		cubesDestroyed+=DestroyCube();
		
		return cubesDestroyed;
	}
	
	public int DestroyCubes(int strength)
	{
		int cubesDestroyed = 0;

		if(strength > 0)
		{
			for(int i=0; i<6; i++)
			{
				if(neighbours[i] != null)
				{
					if(strength == 1)
						cubesDestroyed+=neighbours[i].GetComponent<CubeScript>().DestroyCube();
					if(strength == 2)
						cubesDestroyed+=neighbours[i].GetComponent<CubeScript>().BigDestroy();
				}
			}
		}
		
		cubesDestroyed += DestroyCube();
		return cubesDestroyed;
	}
	
	public int DestroyCube()
	{
		if(invincible)
			return 0;
		
		// Destroy the links in the neighbours to this cube
		for(int i=0; i<6; i++)
		{
			if(neighbours[i] != null)
				neighbours[i].GetComponent<CubeScript>().destroyNeighbour((i == 0) ? Vector3.left : (i == 1) ? Vector3.right : (i == 2) ? Vector3.forward : (i == 3) ? Vector3.back : (i ==4) ? Vector3.down : Vector3.up);
		}
		
		// Make a pretty explosion
		GameObject exp = Instantiate (explosion, transform.position, Quaternion.identity) as GameObject;
		exp.GetComponent<Detonator>().detail = 1.0f;
		
		// Decide if it spawns a powerup
		if(Random.Range(0.0f,1.0f) < 0.2f)
			Instantiate(powerupObject,transform.position,Quaternion.identity);
		
		Destroy(transform.gameObject);
		
		return 1;
	}
	
	private void CheckNeighbours()
	{
		transform.GetComponent<BoxCollider>().enabled = false;
		
		RaycastHit hit;
		for(int i=0; i<6; i++)
		{
			if(Physics.Raycast(rays[i], out hit, castLength) && hit.collider.GetComponent<CubeScript>())
				neighbours[i] = hit.collider.gameObject;
			else
				neighbours[i] = null;
		}
		
		transform.GetComponent<BoxCollider>().enabled = true;
	}
	
	void Destroy()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
