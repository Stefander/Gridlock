using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
	private GameObject currentCube;
	private Vector3 currentAxis;
	private bool isTransitioning;
	public GameObject bombObject;
	public Texture deadView;
	private bool spacePressed;
	public int bombStrength;
	private Vector3 targetAxis;
	private Vector3 rotationAxis;
	private float moveSpeed = 2.5f;
	public int playerIndex;
	private float powerSpeed = 4.5f;
	private float lookSpeed = 150.0f;
	private int blocksDestroyed;
	private float minX = 50;
	private float maxX = 340;
	private bool isAlive;
	private float rotated = 0;
	private float cubeSize = 1.12f;
	private Transform cam;
	private float rotateSpeed = 200;
	private float castLength = 1.0f;
	
	//Powerups
	private float powerupDuration = 15.0f;
	private float powerupTime = 0.0f;
	private bool hasPowerup = false;
	private bool hasMovePickup = false;
	
	// Use this for initialization
	void Start () 
	{
		isTransitioning = false;
		cam = transform.Find("Main Camera");
		spacePressed = false;
		blocksDestroyed = 0;
		
		RaycastHit hit;
		if(Physics.Raycast(new Ray(transform.position,Vector3.down), out hit, castLength) && hit.collider.GetComponent<CubeScript>())
				currentCube = hit.collider.gameObject;
		
		isAlive = true;
		bombStrength = 0;
		transform.position = new Vector3(currentCube.transform.position.x,currentCube.transform.position.y+cubeSize/2,currentCube.transform.position.z);
		currentAxis = Vector3.up;
	}
	
	void PlantBomb()
	{
		GameObject bomb = Instantiate(bombObject,transform.position,transform.rotation) as GameObject;
		bomb.GetComponent<BombScript>().AttachToCube(this,currentAxis,cubeSize,currentCube);
	}
	
	public void RegisterDestroyed(int amount)
	{
		blocksDestroyed+=amount;
	}
	
	void OnGUI()
	{
		Rect camRect = cam.GetComponent<Camera>().rect;
		Rect screenRect = new Rect(camRect.x*Screen.width,(camRect.y != 0) ? 0 : 0,camRect.width*Screen.width,camRect.height*Screen.height);
		
		if(!isAlive)
			GUI.DrawTexture(screenRect,deadView);
		
		GUI.Label(new Rect(screenRect.x + 10.0f,screenRect.y + 10.0f,200,50),"Blocks destroyed: "+blocksDestroyed);
	}
	
	public void Kill()
	{
		if(!isAlive)
			return;
		
		print ("Killed player.");
		isAlive = false;
	}
	
	public void Pickup(int index)
	{
		print ("Picked up powerup "+index);
		
		if(index == 0)
		{
			hasMovePickup = false;
			bombStrength=(bombStrength < 2) ? bombStrength+1 : bombStrength;
		}
		
		if(index == 1)
		{
			bombStrength = 0;
			hasMovePickup = true;
		}
		
		powerupTime = 0;
		hasPowerup = true;
	}
	
	public void ResetPickup()
	{
		bombStrength = 0;
		hasMovePickup = false;
		hasPowerup = false;
	}
	
	void GroundPlayer()
	{
		float dX = (transform.position.x-currentCube.transform.position.x);
		float dY = (transform.position.y-currentCube.transform.position.y);
		float dZ = (transform.position.z-currentCube.transform.position.z);
		float hCubeSize = cubeSize/2;
		// Clamp position
		transform.position = new Vector3(	(dX < -hCubeSize) ? currentCube.transform.position.x-hCubeSize : (dX > hCubeSize) ? currentCube.transform.position.x+hCubeSize : transform.position.x,
											(dY < -hCubeSize) ? currentCube.transform.position.y-hCubeSize : (dY > hCubeSize) ? currentCube.transform.position.y+hCubeSize : transform.position.y,
											(dZ < -hCubeSize) ? currentCube.transform.position.z-hCubeSize : (dZ > hCubeSize) ? currentCube.transform.position.z+hCubeSize : transform.position.z	);
		
		// Reset all velocities
		transform.rigidbody.angularVelocity = Vector3.zero;
		transform.rigidbody.velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float fire = Input.GetAxis((playerIndex == 0) ? "Jump" : "Jump2");
		
		if(!isAlive)
		{
			if(!spacePressed && fire != 0)
				Application.LoadLevel(0);
			return;
		}
		
		if(!currentCube)
		{
			Kill();
			return;
		}
		
		if(hasPowerup)
		{
			powerupTime+=Time.deltaTime;
			
			if(powerupTime>= powerupDuration)
				ResetPickup();
		}
		
		GroundPlayer();
		
		if(!isTransitioning)
		{
			// Transform and normalize axes
			Vector3 fw = new Vector3((currentAxis.x == 0) ? cam.forward.x : 0, (currentAxis.y == 0) ? cam.forward.y : 0, (currentAxis.z == 0) ? cam.forward.z : 0).normalized;
			//Vector3 le = new Vector3((currentAxis.x == 0) ? cam.right.x : 0, (currentAxis.y == 0) ? cam.right.y : 0, (currentAxis.z == 0) ? cam.right.z : 0).normalized;
			
			// Forward/backward
			float mSpeed = (hasMovePickup) ? powerSpeed : moveSpeed;
			float vert = Input.GetAxis((playerIndex == 0) ? "Vertical" : "Vertical2");//(playerIndex == 0) ? Input.GetAxis("Vertical") : Input.GetAxis("Vertical2");
			float hor = Input.GetAxis((playerIndex == 0) ? "Horizontal" : "Horizontal2");//(playerIndex == 0) ? Input.GetAxis("Horizontal") : Input.GetAxis ("Horizontal2");
			float viewMod = Input.GetAxis((playerIndex == 0) ? "ViewMod" : "ViewMod2");
			
			if(vert != 0)
				transform.Translate(fw*vert*mSpeed*Time.deltaTime,Space.World);
			
			float newX = cam.localEulerAngles.x+viewMod*lookSpeed*Time.deltaTime; 
			// Update euler angles and clamp X between min and max
			cam.localEulerAngles = new Vector3(((newX < 200 && newX > minX) ? minX : (newX > 200 && newX < maxX) ? maxX : newX),cam.localEulerAngles.y+hor*lookSpeed*Time.deltaTime,cam.localEulerAngles.z);
			
			// Fire a bomb :)
			if(!spacePressed && fire != 0)
			{
				spacePressed = true;
				PlantBomb();
			}
			
			if(spacePressed && fire == 0)
				spacePressed = false;
			
			// See if we're over the edge
			float dX = (transform.position.x-currentCube.transform.position.x);
			float dY = (transform.position.y-currentCube.transform.position.y);
			float dZ = (transform.position.z-currentCube.transform.position.z);
			float hCubeSize = cubeSize/2;
			targetAxis = Vector3.zero;
			if(dY > hCubeSize && currentAxis != Vector3.up)
				targetAxis = Vector3.up;
			else if(dY < -hCubeSize && currentAxis != Vector3.down)
				targetAxis = Vector3.down;
			else if(dX < -hCubeSize && currentAxis != Vector3.left)
				targetAxis = Vector3.left;
			else if(dX > hCubeSize && currentAxis != Vector3.right)
				targetAxis = Vector3.right;
			else if(dZ > hCubeSize && currentAxis != Vector3.forward)
				targetAxis = Vector3.forward;
			else if(dZ < -hCubeSize && currentAxis != Vector3.back)
				targetAxis = Vector3.back;
			
			// Decide whether to flip or go to the next cube
			if(targetAxis != Vector3.zero)
			{
				GameObject neighbour = currentCube.GetComponent<CubeScript>().getNeighbour(targetAxis);
				
				if(neighbour != null)
				{
					currentCube = neighbour;
					return;
				}
				
				//print ("Target axis: "+targetAxis);
				
				// Calculate rotation direction
				Vector3 cA = currentAxis;
				Vector3 tA = targetAxis;
				
				if((cA == Vector3.up && tA == Vector3.back) || (cA == Vector3.down && tA == Vector3.forward) || (cA == Vector3.forward && tA == Vector3.up) || (cA == Vector3.back && tA == Vector3.down))
					rotationAxis = new Vector3(-1,0,0);
				if((cA == Vector3.back && tA == Vector3.up) || (cA == Vector3.forward && tA == Vector3.down) || (cA == Vector3.up && tA == Vector3.forward) || (cA == Vector3.down && tA == Vector3.back))
					rotationAxis = new Vector3(1,0,0);
				
				if((cA == Vector3.forward && tA == Vector3.left) || (cA == Vector3.back && tA == Vector3.right) || (cA == Vector3.right && tA == Vector3.forward) || (cA == Vector3.left && tA == Vector3.back))
					rotationAxis = new Vector3(0,-1,0);
				if((cA == Vector3.left && tA == Vector3.forward) || (cA == Vector3.right && tA == Vector3.back) || (cA == Vector3.forward && tA == Vector3.right) || (cA == Vector3.back && tA == Vector3.left))
					rotationAxis = new Vector3(0,1,0);
				
				if((cA == Vector3.left && tA == Vector3.up) || (cA == Vector3.right && tA == Vector3.down) || (cA == Vector3.down && tA == Vector3.left) || (cA == Vector3.up && tA == Vector3.right))
					rotationAxis = new Vector3(0,0,-1);
				if((cA == Vector3.up && tA == Vector3.left) || (cA == Vector3.down && tA == Vector3.right) || (cA == Vector3.left && tA == Vector3.down) || (cA == Vector3.right && tA == Vector3.up))
					rotationAxis = new Vector3(0,0,1);
				
				rotated = 0;
				isTransitioning = true;
			}
		}
		else
		{
			float rotateDelta = Time.deltaTime*rotateSpeed;
			
			if(rotateDelta+rotated >= 90)
			{
				isTransitioning = false;
				if(rotated < 90)
					transform.Rotate(rotationAxis*(90-rotated),Space.World);
				currentAxis = targetAxis;
			}
			else
			{
				transform.Rotate(rotationAxis*rotateDelta,Space.World);
				rotated+=rotateDelta;
			}
		}
	}
}
