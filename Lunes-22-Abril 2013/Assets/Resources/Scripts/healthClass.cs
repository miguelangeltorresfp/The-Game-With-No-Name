using UnityEngine;
using System.Collections;

/// <summary>
/// Prototipo Sistema básico de salud de un personaje.
/// </summary>
public class healthClass : MonoBehaviour
{
	
	public int maxHealth = 100;
	public int minHealth = 0;
	
	public int health;

	public enum HealthState {live, death};
	public HealthState healthState = HealthState.live;
	
	public bool resurrectable = false;
	public bool invulnerable = false;
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void decreaseHealth(int damage)
	{
		if(invulnerable)
			return;
		
		health -= damage;
		
		if(health <= minHealth)
		{
			health = minHealth;
			healthState = HealthState.death;
		}
		
	}
	
	public void increaseHealth(int healing)
	{
		health += healing;
		
		if(health > maxHealth)
			health = maxHealth;
		
		if(resurrectable && healthState == HealthState.death && health > minHealth)
			healthState = HealthState.live;
		
	}
	
}
