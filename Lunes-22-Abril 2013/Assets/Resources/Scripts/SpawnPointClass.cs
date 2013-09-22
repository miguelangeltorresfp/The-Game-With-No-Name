using UnityEngine;
using System.Collections;

public class SpawnPointClass : MonoBehaviour
{
	public float intervalo = 20.0f;
	public int maximo_enemigos = 1;
	public Transform player;
	public GameObject[] tipo_enemigo;
	public float velocidad;
	public float rotationSpeed;
	public float distanciaVision;
	//Se traza (o no) en el log
	public bool trace = false;
	
	private GameObject enemigo_llamado;
	private float intervalo_actual;
	private int cuenta = 0;
	
	public enum Dificultad {facil, normal, dificil, custom};
	public Dificultad dificultad = Dificultad.normal;
	
	// Use this for initialization
	void Start () 
	{
		if(dificultad == Dificultad.facil)
		{
			velocidad = 2;
			rotationSpeed = 400;
			distanciaVision = 5;
		}
		else if (dificultad == Dificultad.normal)
		{
			velocidad = 4;
			rotationSpeed = 600;
			distanciaVision = 9;
		}
		else if (dificultad == Dificultad.dificil)
		{
			velocidad = 6;
			rotationSpeed = 800;
			distanciaVision = 13;
		}
		
		//Parámetros por defecto
		//Generar primer enemigo
		enemigo_llamado = tipo_enemigo[Random.Range(0, tipo_enemigo.GetLength(0))];
		Inic(enemigo_llamado);		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(cuenta < maximo_enemigos)
		{
			intervalo_actual -= Time.deltaTime;
			if(intervalo_actual <= 0)
			{
				enemigo_llamado = tipo_enemigo[Random.Range(0, tipo_enemigo.GetLength(0))];
				Inic(enemigo_llamado);
			}
		}
	}
	
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
	
	void Inic(GameObject enemigo)
	{
		GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/" + enemigo.name) as GameObject, transform.position, transform.rotation);	
		go.GetComponent<enemigoClass>().trPlayer = player;
		go.GetComponent<enemigoClass>().maxSpeed = velocidad;
		go.GetComponent<enemigoClass>().rotationSpeed = rotationSpeed;
		go.GetComponent<enemigoClass>().distanciaVision = distanciaVision;
		go.name = go.name + cuenta.ToString();
		go.GetComponent<enemigoClass>().trace = trace;
		
		if(trace)
			Debug.Log("Enemigo a escena: " + go.name);
		intervalo_actual = intervalo;
		cuenta++;
	}
	
	
}
