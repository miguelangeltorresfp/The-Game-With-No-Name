using UnityEngine;
using System.Collections;

public class balaClass : proyectilClass 
{
	
	/// <summary>
	/// Tiempo de vida del proyectil
	/// </summary>
	public float lifeTime;
	//public static Vector3 offsetPlayer = new Vector3(0, 0.5f, 0.5f);
	public int damage = 10;
	
	// Use this for initialization
	void Start () 
	{
		Invoke("destruirProyectil", lifeTime);
	}
	
	void destruirProyectil()
	{
		Destroy(gameObject);
	}

	void OnCollisionEnter(Collision collision) 
	{
		if(collision.transform.tag == "Player" || collision.transform.tag == "suelo")
		{
			Destroy(gameObject);
		}
		else if(collision.transform.tag.Contains("estatico"))
		{
			collision.transform.rigidbody.AddForce(transform.position.normalized * 5, ForceMode.Impulse);
			Destroy(gameObject);
		}
		else
		{
			Instantiate(Resources.Load("Prefabs/Chispas"), collision.transform.position , Quaternion.identity);
			collision.gameObject.SendMessage("decreaseHealth", damage, SendMessageOptions.DontRequireReceiver);
			//Destroy(collision.gameObject);
			Destroy(gameObject);
		}
    }	
	
	
	
}
