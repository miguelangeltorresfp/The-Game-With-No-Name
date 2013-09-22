using UnityEngine;
using System.Collections;

public class granadaClass : proyectilClass
{
	/// <summary>
	/// Tiempo de vida del proyectil
	/// </summary>
	public float lifeTime;
	
	protected float velocidadInicial = 100;
	public float anguloTiroGrados = 45;
	protected float anguloTiroRadians = 0;
	
	//La fuerza del disparo que usamos como base
	public float shotPower = 0.1f;
	/// <summary>
	/// The fuerza maxima tiro.
	/// </summary>
	public float fuerzaMaximaTiro = 10;
	public float fuerzaMinimaTiro = 6;
	/// <summary>
	/// The damage area. Esta granada causará daño a los enemigos que se encuentren dentro 
	/// de esta área.
	/// </summary>
	public float DamageArea = 5;
	

	/// <summary>
	/// The debug.
	/// </summary>
	public bool debug = false;
	
	// Use this for initialization
	void Start () 
	{
		//Convertimos el ángulo de tiro a radianes
		anguloTiroRadians = anguloTiroGrados *  Mathf.Deg2Rad;
		//Comenzamos con la coroutine para ver con qué fuerza lanzamos la granada
		StartCoroutine(iniciarContadorIntensidadDisparo());
	}
	
	//Coroutina para ver con qué intensidad lanzamos la granada.
	IEnumerator iniciarContadorIntensidadDisparo()
	{
		//Para saber cuándo soltamos el botón de disparo y efectuar el disparo con una intensidad proporcional
		//al tiempo que hayamos mantenido el botón pulsado.
		bool haSoltadoBoton = false;
		
		// Para saber con qué intensidad lanzamos la granada en modo manual
		float contadorMousePresionado = 0;
		
		while(true)
		{
			if(Input.GetButtonUp("Fire2"))
			{
				haSoltadoBoton = true;
				//Calculamos la intensidad del disparo
				shotPower = shotPower * contadorMousePresionado;
				if(shotPower > fuerzaMaximaTiro ) shotPower = fuerzaMaximaTiro;
				else if(shotPower < fuerzaMinimaTiro ) shotPower = fuerzaMinimaTiro;
				
				if( debug )
				{
					print ( "contadorMousePresionadoDespuesDeSoltar = " + contadorMousePresionado);
					print ( "shotPower = " + shotPower);
				}
				dispararGranada();
				break;
			}
			
			contadorMousePresionado ++;
			
			print ( "contadorMousePresionado = " + contadorMousePresionado);
			
			yield return 0;
		}
	}
	
	void dispararGranada()
	{
		//Hacemos visible la granada
		this.GetComponent<MeshRenderer>().enabled = true;
		//Capturamos el RigidBody
		Rigidbody thisRigidbody = this.GetComponent<Rigidbody>();
		//Habilitamos el efecto de la gravedad
		thisRigidbody.useGravity = true;
		 // apply the firing force
		thisRigidbody.AddForce(transform.forward * shotPower, ForceMode.Impulse);	
		thisRigidbody.AddForce(transform.up * shotPower, ForceMode.Impulse);
	}
	
	
	void OnCollisionEnter(Collision collision) 
	{
		//Instanciamos un prefab de explosión.
		Instantiate(Resources.Load("Prefabs/ExplosionShuriken"), transform.position , Quaternion.identity);
		//Invocamos el método destruir granada
		destruirGranada();
    }	//	OnCollisionEnter()
	
	
	void destruirGranada()
	{
		//Calculamos si hay enemigos en una área alrededor de la granada.
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, DamageArea);
			
		int damage = 10;
		
	    int i = 0;
	    while (i < hitColliders.Length)
		{
			//if( hitColliders[i].tag == "Player" || hitColliders[i].tag == "suelo" )
			if(hitColliders[i].tag == "suelo" )
			{
				
			}
			else
			{
				print ( "nombre enemigo afectado por granada = " + hitColliders[i].tag );
				Destroy(hitColliders[i].gameObject);
			}
			//hitColliders[i].gameObject.SendMessage("decreaseHealth", damage, SendMessageOptions.DontRequireReceiver);
            i++;
	    }
		//Destruimos esta instancia de granada
		Destroy(transform.parent.gameObject);
	}
	
	
	public void dibujarTrayectoria(Vector3 initialPosition, Vector3 finalPosition)
	{
		float tiempoTotalInvertido = 0;
		float anguloInicial = 0;
		
		//tiempoTotalInvertido = velocidadInicial * Mathf.Sin (anguloInicial)
		
//		Vector3 direccionNormalizada = (finalPosition - initialPosition).Normalize();
//		
//		
//		
//		finalPosition = tiempoTotalInvertido * velocidadInicial * Mathf.Cos( anguloInicial );
//		
//		finalPositionY
	}
	

}
