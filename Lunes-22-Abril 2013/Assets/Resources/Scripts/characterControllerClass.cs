using UnityEngine;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Implementa un sistema rudimentario de character de combate
/// 
///////////////////////////////////////////////////////////////////////////////////////////////////


/// <summary>
/// Prototipo de un character de combate
/// </summary>
public class characterControllerClass : MonoBehaviour 
{
	public bool trace;
	public float impulsoBala = 1;
	public enum TipoProyectil {misil, bala};
	[HideInInspector]
	public TipoProyectil tipoProyectil;
	public enum ModoDisparo {auto, manual};
	public ModoDisparo modoDisparo;
	public Vector3 offsetPlayerBalas = new Vector3(0, 0.75f, 0.5f);
	public Vector3 offsetPlayerMisiles = new Vector3(0, 0.75f, 1);
	public Vector3 offsetTarget = new Vector3(0, 0.75f, 0);
	protected Transform tr;
	protected float umbralPrecision = 0.05f;
	protected int rotationSpeed = 800;
	public GUIText TextoMunicion;
	public GUIText TextoModoDisparo;
	protected CharacterController controller;
	public float rangoBala = 20;
	protected string nombreLayerSuelo = "suelo";
	protected string nombreLayerScenaryStatic = "scenary_static";
	protected string nombreLayerPlayer = "Player";
	protected LayerMask layerSuelo;
	protected LayerMask layerMask_IgnorePlayer;
	protected LayerMask layerScenaryStatic;
	protected LayerMask layerMask_IgnoreSphereCast;
	
	// Use this for initialization
	void Start () 
	{
		tr = transform;
		
		controller = GetComponent<CharacterController>();
		controller.detectCollisions = false;
		
		if(tipoProyectil == TipoProyectil.bala)
			TextoMunicion.text = "Munici\u00F3n: Bala";
		else if(tipoProyectil == TipoProyectil.misil)
			TextoMunicion.text = "Munici\u00F3n: Misil";
		
		if(modoDisparo ==  ModoDisparo.auto)
			TextoModoDisparo.text = "Modo disparo: autom\u00E1tico";
		if(modoDisparo ==  ModoDisparo.manual)
			TextoModoDisparo.text = "Modo disparo: manual";
		
		layerSuelo = 1 << LayerMask.NameToLayer(nombreLayerSuelo);
		layerScenaryStatic = 1 << LayerMask.NameToLayer(nombreLayerScenaryStatic);
		
		layerMask_IgnoreSphereCast = layerSuelo | layerScenaryStatic;
		layerMask_IgnoreSphereCast = ~layerMask_IgnoreSphereCast;
		
		layerMask_IgnorePlayer = 1 << LayerMask.NameToLayer(nombreLayerPlayer);
		layerMask_IgnorePlayer = ~layerMask_IgnorePlayer;
		
	}	//end Start()
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Fire1"))
		{
			Vector3 raton;
			Vector3 view;
			raton = Input.mousePosition;
			view = Camera.main.ScreenToViewportPoint (raton);
			Ray ray = Camera.main.ViewportPointToRay(view);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				Transform target;
				if(trace)
					print("Apuntando a: " + hit.transform.name);
				
				//*********************************************************************************************
				//
				// Sección que facilita el disparo
				//
				if(hit.transform.tag.Contains("suelo"))
				{
					Collider[] colliders = Physics.OverlapSphere(hit.point, 3, layerMask_IgnoreSphereCast);
					
					if(colliders.GetLength(0) > 0)
					{
						int index = 0;
						for(index = 0; index < colliders.GetLength(0); index++)
						{
							if(colliders[index].transform.tag.Contains("enemigo"))
								break;
						}
						if(index < colliders.GetLength(0))
						{
							Vector3 direccion = (colliders[index].transform.position + (colliders[index].transform.forward * offsetTarget.z) + (colliders[index].transform.right * offsetTarget.x) + (colliders[index].transform.up * offsetTarget.y)) - 
												(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y));
							ray.direction = direccion;
							ray.origin = tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y);
							if(!Physics.Raycast(ray, out hit, direccion.magnitude, layerMask_IgnorePlayer))
							{
								Debug.DrawRay(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), direccion, Color.magenta);
								return;
							}
							else 
							{
								if(trace)
									print ("Enemigo " + hit.transform.name + " facilitado");
									
							}
						}
					}
				}
				//
				//*********************************************************************************************
				
				
				if(hit.transform.tag.Contains("enemigo"))
				{
					Vector3 tempOffset = offsetTarget;
					target = hit.transform;
					
					if(modoDisparo == ModoDisparo.auto)
					{
						if(target.GetComponent<healthClass>().health > 10)				//dirigido por salud
						//if((target.position - tr.position).magnitude <= rangoBala)	//dirigido por distancia
						{
							tipoProyectil = TipoProyectil.bala;
							TextoMunicion.text = "Munici\u00F3n: Bala";
						}
						else
						{
							tipoProyectil = TipoProyectil.misil;
							TextoMunicion.text = "Munici\u00F3n: Misil";
						}
					}

					if(target.CompareTag("enemigo_estatico"))
						tempOffset = Vector3.zero;
					
					if(tipoProyectil == TipoProyectil.bala)
					{
						//Bala
						
						StopAllCoroutines();
						StartCoroutine(dispararBala(target, tempOffset));
					}
					else if(tipoProyectil == TipoProyectil.misil)
					{
						//Misil
						StopAllCoroutines();
						StartCoroutine(dispararMisil(target, tempOffset));
					}
				}
			}
		}
	}	//	end Update()
	
	
	
	
	IEnumerator dispararBala(Transform target, Vector3 offsetTarget)
    {
		//Calculo la dirección del proyectil hacia el target
		Vector3 direction = (target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)) - 
			(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y));
		
		if(controller.velocity.x == 0 && controller.velocity.z == 0)
		{
			//Rotación que debo adoptar para mirar hacia direction
			Quaternion rotation = Quaternion.LookRotation(direction);
		
	        while (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecision)
			{
				//Rotamos descartando cualquier rotación que no sea del eje Y
				tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
				yield return 0;
			}
		}
		
		//Comprueba si el eenemigo está todavía vivo
		if(target != null)
		{
			//Instancio bala en mi posición + offsetLocal. Quaternion.Identity significa "sin rotación"
			GameObject bala = (GameObject) Instantiate(Resources.Load("Prefabs/Bala"), tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), Quaternion.identity);
			
			//Re-calculo la dirección del proyectil hacia el target, ya que el enemigo está en movimiento y se pueden haber producido variaciones 
			direction = (target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)) - 
				(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y));
			
			
			if(trace)
			{
				Debug.DrawRay(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), direction, Color.green);
				//Debug.DrawLine(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), posicionTarget + offset, Color.green);
				Debug.Break();
			}
			
			//Nos ahorramos la siguiente línea modificando la matriz de colisiones en el editor
			//Physics.IgnoreCollision(bala.collider, collider);
			
			//Aplicamos impulso a la bala
			bala.rigidbody.AddForce (direction * impulsoBala, ForceMode.Impulse);
			//Habilitamos la estela
			bala.GetComponent<TrailRenderer>().enabled = true;
		}
		else
		{
			if(trace)
				print("target nulo");
		}
		
    } // end dispararBala()
	
	
	
	
	IEnumerator dispararMisil(Transform target, Vector3 offsetTarget)
    {
		//Calculo la dirección del proyectil hacia el target
		Vector3 direction = (target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)) - 
			(tr.position + (transform.forward * offsetPlayerMisiles.z) + (transform.right * offsetPlayerMisiles.x) + (transform.up * offsetPlayerMisiles.y));
		
		if(controller.velocity.x == 0 && controller.velocity.z == 0)
		{
			//Rotación que debo adoptar para mirar hacia direction
			Quaternion rotation = Quaternion.LookRotation(direction);
			
	        while (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecision)
			{
				//Rotamos descartando cualquier rotación que no sea del eje Y
				tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
				yield return 0;
			}
		}

		//Comprueba si ele enemigo está todavía vivo
		if(target != null)
		{
		
			//Instancio misil en mi posición + offsetPlayer. 
			GameObject misil = (GameObject) Instantiate(Resources.Load("Prefabs/Misil"), tr.position + (transform.forward * offsetPlayerMisiles.z) + (transform.right * offsetPlayerMisiles.x) + (transform.up * offsetPlayerMisiles.y), Quaternion.identity);
			
			//Nos ahorramos la siguiente línea modificando la matriz de colisiones en el editor
			//Physics.IgnoreCollision(misil.transform.GetChild(0).collider, collider);
			
			//El misil apunta a la posición del player + offsetTarget
			misil.transform.LookAt(target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y));
			
			if(trace)
			{
				Debug.DrawLine(tr.position + (transform.forward * offsetPlayerMisiles.z) + (transform.right * offsetPlayerMisiles.x) + (transform.up * offsetPlayerMisiles.y), 
					(target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)), Color.blue);
				Debug.Break();
			}
			
			//Habilitamos la estela
			misil.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().enabled = true;
		}
		
		
    } // end dispararMisil()
	
	
    void OnGUI() 
	{
        Event e = Event.current;
        if (e.type == EventType.scrollWheel && modoDisparo != ModoDisparo.auto)
		{
            if(tipoProyectil == TipoProyectil.bala)
			{
				tipoProyectil = TipoProyectil.misil;
				TextoMunicion.text = "Munici\u00F3n: Misil";
			}
			else
			{
				tipoProyectil = TipoProyectil.bala;
				TextoMunicion.text = "Munici\u00F3n: Bala";
			}
		}
		
	}	// end OnGui()
	
	
}
