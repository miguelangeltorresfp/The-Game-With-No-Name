using UnityEngine;
using System.Collections;

public class DisplayClass : MonoBehaviour 
{

	public GUIText Texto_01;
	public GUIText Texto_02;
	protected int liveEnemies;
	protected int deadEnemies;
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	void liveEnemy()
	{
		liveEnemies++;
		Texto_01.text = "Enemigos vivos: " + liveEnemies.ToString();
	}
		
	void deadEnemy()
	{
		deadEnemies++;
		Texto_02.text = "Enemigos muertos: " + deadEnemies.ToString();
		liveEnemies--;
		Texto_01.text = "Enemigos vivos: " + liveEnemies.ToString();
	}
		
		
}
