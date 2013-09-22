using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour 
{
	Transform getChild(int index)
	{
		foreach(Transform child in transform)
		{
			if(child.GetComponent<PlayerScript>().playerIndex == index)
				return child;
		}
		
		return null;
	}
	// Use this for initialization
	void Start () 
	{
		for(int i=0; i<transform.childCount; i++)
		{
			Transform player = getChild(i);
			if(player != null)
			{
				float viewHeight = (transform.childCount > 1) ? 0.5f : 1.0f;
				print (viewHeight);
				float viewPosY = (i > 0 || transform.childCount == 1) ? 0 : 0.5f;
				Camera playerCam = player.Find("Main Camera").GetComponent<Camera>();
				Rect renderRect = new Rect(0.0f,viewPosY,1.0f,viewHeight);
				playerCam.rect = renderRect;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
