using UnityEngine;
using System.Collections;

public class misilNormalClass : proyectilClass
{
	/// <summary>
	/// Tiempo de vida del proyectil
	/// </summary>
	public float lifeTime;
	public static Vector3 offsetPlayer;
	
	// Use this for initialization
	void Start () 
	{
		Invoke("destruirProyectil", lifeTime);
	}
	
	void destruirProyectil()
	{
		Destroy(transform.parent.gameObject);
	}

	void OnCollisionEnter(Collision collision) 
	{
		if(collision.transform.tag == "Player" || collision.transform.tag == "suelo")
		{
			Destroy(transform.parent.gameObject);
		}
		else if(collision.transform.tag.Contains("estatico"))
		{
			collision.transform.rigidbody.AddForce(transform.position.normalized * 10, ForceMode.Impulse);
			Destroy(transform.parent.gameObject);
		}
		else
		{
			Instantiate(Resources.Load("Prefabs/explosion"), transform.position , Quaternion.identity);
			Destroy(collision.gameObject);
			Destroy(transform.parent.gameObject);
		}
    }	//	OnCollisionEnter()
	
}
