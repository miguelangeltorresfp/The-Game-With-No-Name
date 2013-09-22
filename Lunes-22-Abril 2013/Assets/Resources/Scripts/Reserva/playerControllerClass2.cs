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
public class playerControllerClass2 : MonoBehaviour 
{
	#region Variables miembro
	
	/// <summary>
	/// My GUI skin.
	/// </summary>
	public GUISkin gameGuiSkin;
	
	//Indica si estamos depurando
	public bool trace;
	
	/// <summary>
	/// The impulso bala.
	/// </summary>
	public float impulsoBala = 10;
	
	/// <summary>
	/// Enumeración Tipo proyectil.
	/// </summary>
	public enum TipoProyectil {misil, bala};
	
	[HideInInspector]
	/// <summary>
	/// The tipo proyectil.
	/// </summary>
	public TipoProyectil tipoProyectil;
	
	/// <summary>
	/// Enumeración Modo disparo.
	/// </summary>
	public enum ModoDisparo {auto, manual};
	
	/// <summary>
	/// The modo disparo.
	/// </summary>
	public ModoDisparo modoDisparo;
	
	/// <summary>
	/// Player state.
	/// </summary>
	public enum PlayerState
	{
		/// <summary>
		/// El jugador está vivo
		/// </summary>
		live, 
		
		/// <summary>
		/// El jugador está muerto
		/// </summary>
		death
	};
	protected PlayerState playerState = PlayerState.live;
	
	/// <summary>
	/// Desplazamiento en la instaciación de la bala con respecto al origen del player.
	/// </summary>
	public Vector3 offsetPlayerBalas = new Vector3(0, 0.75f, 0.5f);
	
	/// <summary>
	/// Desplazamiento en la instaciación del misil con respecto al origen del player.
	/// </summary>
	public Vector3 offsetPlayerMisiles = new Vector3(0, 0.75f, 1);
	
	/// <summary>
	/// Desplazamiento en el punto de impacto con respecto al origen del enemigo.
	/// </summary>
	public Vector3 offsetTarget = new Vector3(0, 0.75f, 0);
	
	/// <summary>
	/// Transform del player
	/// </summary>
	protected Transform tr;
	
	/// <summary>
	/// Precisión de la rotación del player
	/// </summary>
	protected float umbralPrecisionRotacion = 0.05f;
	
	/// <summary>
	/// Precisión del desplazamiento del player
	/// </summary>
	protected float umbralPrecisionDesplazamiento = 0.25f;
	
	/// <summary>
	/// The controller.
	/// </summary>
	protected CharacterController controller;
	
	/// <summary>
	/// The depth. Nos determina si el player se moverá en 2 o 3 dimensiones.
	/// </summary>
	public bool depth=true;
	
	/// <summary>
	/// The move direction.
	/// </summary>
	Vector3 moveDirection = Vector3.zero;
	
	/// <summary>
	/// The character lineal speed.
	/// </summary>
	public float linealSpeed = 6f;
	
	/// <summary>
	/// The jump speed.
	/// </summary>
	public float jumpSpeed = 8f;
		
	/// <summary>
	/// The rotation speed.
	/// </summary>
	protected float rotationSpeed = 4f;
	
	/// <summary>
	/// The gravity.
	/// </summary>
	public float gravity = 20f;
	
	/// <summary>
	/// The texto municion.
	/// </summary>
	public GUIText TextoMunicion;
	
	/// <summary>
	/// The texto modo disparo.
	/// </summary>
	public GUIText TextoModoDisparo;
	
	/// <summary>
	/// The rango bala, en caso de que la selección automática de la selección sea por distancia.
	/// </summary>
	public float rangoBala = 20;
	
	/// <summary>
	/// The nombre layer suelo.
	/// </summary>
	protected string nombreLayerSuelo = "suelo";
	
	/// <summary>
	/// The nombre layer scenary static.
	/// </summary>
	protected string nombreLayerScenaryStatic = "scenary_static";
	
	/// <summary>
	/// The nombre layer player.
	/// </summary>
	protected string nombreLayerPlayer = "Player";
	
	/// <summary>
	/// The layer suelo.
	/// </summary>
	protected LayerMask layerSuelo;
	
	/// <summary>
	/// The layer Player.
	/// </summary>
	protected LayerMask layerPlayer;
	
	/// <summary>
	/// The layer scenary static.
	/// </summary>
	protected LayerMask layerScenaryStatic;
	
	/// <summary>
	/// The layer mask_ ignore player.
	/// </summary>
	protected LayerMask layerMask_IgnorePlayer;
	
	/// <summary>
	/// The layer mask_ ignore sphere cast.
	/// </summary>
	protected LayerMask layerMask_IgnoreSphereCast;
	
	/// <summary>
	/// The player energy.
	/// </summary>
	protected int playerEnergy = 100;
	/// <summary>
	/// The player energy max.
	/// </summary>
	protected int playerEnergyMax = 100;
	
	/// <summary>
	/// The playe energy text.
	/// </summary>
	public GUIText playerEnergyText;
	
	
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		//Asigno mi transform
		tr = transform;
		
		//Asigno mi character controller
		controller = GetComponent<CharacterController>();
		//Desabilito colisiones
		//controller.detectCollisions = false;
		
		//Visualiza la munición al principio
		if(tipoProyectil == TipoProyectil.bala)
			TextoMunicion.text = "Munici\u00F3n: Bala";
		else if(tipoProyectil == TipoProyectil.misil)
			TextoMunicion.text = "Munici\u00F3n: Misil";
		
		//Visualiza el modo de disparo
		if(modoDisparo ==  ModoDisparo.auto)
			TextoModoDisparo.text = "Modo disparo: autom\u00E1tico";
		if(modoDisparo ==  ModoDisparo.manual)
			TextoModoDisparo.text = "Modo disparo: manual";
		
		//Configuramos la capas
		layerSuelo = 1 << LayerMask.NameToLayer(nombreLayerSuelo);
		layerScenaryStatic = 1 << LayerMask.NameToLayer(nombreLayerScenaryStatic);
		layerPlayer = 1 << LayerMask.NameToLayer(nombreLayerPlayer);
		
		layerMask_IgnoreSphereCast = ~(layerSuelo | layerScenaryStatic);
		layerMask_IgnorePlayer = ~layerPlayer;
		
		//Inicializamos el texto de la energía del jugador
		playerEnergyText.text = "Energ\u00EDa : " + playerEnergy.ToString();
		
	}	//end Start()
	
	// Update is called once per frame
	void Update () 
	{
		#region Movimiento Player
		//*********************************************************************************************
		// Sección que facilita el movimiento del player
		//
		if( controller.isGrounded )
		{
			if(depth)
			{
				//moveDirection = new Vector3( Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
				moveDirection = new Vector3( 0,0,Input.GetAxis("Vertical"));
				transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed,0);
			}
			else
			{
				moveDirection = new Vector3(Input.GetAxis("Horizontal"),0,0 );
			}
			
			moveDirection=transform.TransformDirection(moveDirection);
			moveDirection*=linealSpeed;
			
			if( Input.GetButton( "Jump" ))
			{
				moveDirection.y = jumpSpeed;
			}
			
		}
		moveDirection.y -= gravity*Time.deltaTime;
		controller.Move(moveDirection*Time.deltaTime);
		//
		// Fin Sección que facilita el movimiento del player
		//*********************************************************************************************
		#endregion
		
		//Actualizamos el valor de energy en el GUI correspondiente
		playerEnergyText.text = "Energ\u00EDa : " + playerEnergy.ToString();
		
		//Salir del juego
 		if (Input.GetButtonDown("Scape")) 
		{
            Application.Quit();
        }
		else if(playerState == PlayerState.death)
		{
			return;
		}
		
		//Cambiar modo de disparo
		if(Input.GetKeyDown(KeyCode.A))
		{
			modoDisparo = ModoDisparo.auto;
			TextoModoDisparo.text = "Modo disparo: autom\u00E1tico";
		}
		else if(Input.GetKeyDown(KeyCode.M))
		{
			modoDisparo = ModoDisparo.manual;
			TextoModoDisparo.text = "Modo disparo: manual";
		}
		
		if(Input.GetButtonDown("Fire1"))
		{
			Vector3 raton;
			Vector3 view;
			raton = Input.mousePosition;
			//Cursor del ratón en coordenadas de viewPort
			view = Camera.main.ScreenToViewportPoint (raton);
			//Rayo desde la vista
			Ray ray = Camera.main.ViewportPointToRay(view);
			RaycastHit hit;
			
			//Se lanza el rayo 
			if (Physics.Raycast(ray, out hit))
			{
				Transform target;
				if(trace)
					print("Apuntando a: " + hit.transform.name);
				
				#region facilitadores
				//*********************************************************************************************
				//
				// Sección que facilita el disparo
				//
				
				//¿Se hizo clic contra el suelo?
				if(hit.transform.tag.Contains("suelo"))
				{
					//Se hizo clic contra el suelo, casteamos una esfera ignorando el suelo y los elementos estáticos del escenario
					Collider[] colliders = Physics.OverlapSphere(hit.point, 3, layerMask_IgnoreSphereCast);
					
					//¿Hay algo dentro de la esfera?
					if(colliders.GetLength(0) > 0)
					{
						//Encontramos elementos dentro de la esfera
						int index = 0;
						for(index = 0; index < colliders.GetLength(0); index++)
						{
							//Encontramos al menos un enemigo
							if(colliders[index].transform.tag.Contains("enemigo"))
								break;
						}
						
						//¿Hay enemigos?
						if(index < colliders.GetLength(0))
						{
							//Hay enemigos en la esfera
							Vector3 direccion;
							if(colliders[index].transform.tag.Contains("estatico"))
							{
								//Enemigo estático
								direccion = colliders[index].transform.position - (tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y));
							}	
							else
							{
								//Enemigo no estático
								direccion = (colliders[index].transform.position + (colliders[index].transform.forward * offsetTarget.z) + (colliders[index].transform.right * offsetTarget.x) + (colliders[index].transform.up * offsetTarget.y)) - 
												(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y));
							}
							
							//Dirección y origen de rayo hacia el target desde el player
							ray.direction = direccion;
							ray.origin = tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y);
							//Se lanza un rayo en dirección al target, ignorando al player
							if(!Physics.Raycast(ray, out hit, direccion.magnitude, layerMask_IgnorePlayer))
							{
								//No se encontró una dirección despejada hacia el target (por ejemplo, había obstáculos en la trayectoria de tiro)
								if(trace)
								{
									Debug.DrawRay(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), direccion, Color.magenta);
									Debug.Break();
								}
								//En vez de return, se podría continuar para disparar contra cosas del escenario (siempre que se haya seleccionado a un target enemigo)
								return;
							}
							else 
							{
								//Enemigo facilitado
								if(trace)
									print ("Enemigo " + hit.transform.name + " facilitado");
									
							}
						}
						else
						{
							//No hay enemigos en la esfera
							if(trace)
								print ("No hay enemigos en la esfera");	
							return;
						}
					}
					else
					{
						//No hay nada en la esfera (excepto suelo o elementos estáticos del escenario)
						if(trace)
							print ("No hay nada en la esfera");
						
						//**************************************************************
						//
						//Parte experimental para movimiento por clic
						//
						//StopAllCoroutines();
						//Nos movemos hacia el punto donde se hizo clic en el suelo
						//StartCoroutine(goTo(hit.point));
						//
						//**************************************************************
						
						//return;
					}
				}
				//
				// Fin Sección que facilita el disparo
				//*********************************************************************************************
				#endregion
				
				if(hit.transform.tag.Contains("enemigo"))
				{
					//Offset temporal del target, si el enemigo es estático lo pondremos a cero.
					Vector3 tempOffset = offsetTarget;
					//Transform del target
					target = hit.transform;
					
					//Elegimos munición
					if(modoDisparo == ModoDisparo.auto)
					{
						if(target.GetComponent<healthClass>().health > 10 || playerEnergy < 10)	//decido por salud y por energía del player
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

					//Sólo si enemigo estático
					if(target.CompareTag("enemigo_estatico"))
					{
						if(trace)
							print ("Enemigo est\u00E1tico detectado: " + hit.transform.name);
						tempOffset = Vector3.zero;
					}
					
					//Disparo en función del tipo de munición
					if(tipoProyectil == TipoProyectil.bala)
					{
						//Bala
						StopAllCoroutines();
						StartCoroutine(dispararBala(target, tempOffset));
						
						//Por cada disparo de bala resto una cantidad a la energía del player
						playerEnergy -= 10;
					}
					else if(tipoProyectil == TipoProyectil.misil)
					{
						//Misil
						StopAllCoroutines();
						StartCoroutine(dispararMisil(target, tempOffset));
						
						//Por cada disparo de misil resto una cantidad a la energía del player
						playerEnergy -= 50;
					}
				}
			}
		}
		//Si no está pulsando el botón de disparo recuperamos energía
		else
		{
			playerEnergy ++;
			if( playerEnergy > playerEnergyMax ) playerEnergy = playerEnergyMax;
		}
	}	//	end Update()
	
	
	
	/// <summary>
	/// Dispara una bala sobre el target
	/// </summary>
	/// <returns>
	/// Nothing
	/// </returns>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='offsetTarget'>
	/// Offset target.
	/// </param>
	IEnumerator dispararBala(Transform target, Vector3 offsetTarget)
    {
		//Calculo la dirección del proyectil hacia el target
		Vector3 direction = (target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)) - 
			(tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y));
		

		//Si estoy parado o casi, me giro hacia el target
		//if(controller.velocity.magnitude < 5)
		{
			//Rotación que debo adoptar para mirar hacia direction
			Quaternion rotation = Quaternion.LookRotation(direction);
		
	        while (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecisionRotacion)
			{
				//Rotamos descartando cualquier rotación que no sea del eje Y
				tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
				yield return 0;
			}
		}
		
		//Comprueba si el enemigo está todavía vivo
		if(target != null)
		{
			//Instancio bala en mi posición + offsetPlayer. Quaternion.Identity significa "sin rotación"
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
			//El enemigo ha muerto antes de poder disparar
			if(trace)
				print("target nulo");
		}
		
    } // end dispararBala()
	
	
	
	/// <summary>
	/// Disparars the misil.
	/// </summary>
	/// <returns>
	/// Nothing
	/// </returns>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='offsetTarget'>
	/// Offset target.
	/// </param>
	IEnumerator dispararMisil(Transform target, Vector3 offsetTarget)
    {
		//Calculo la dirección del proyectil hacia el target
		Vector3 direction = (target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)) - 
			(tr.position + (transform.forward * offsetPlayerMisiles.z) + (transform.right * offsetPlayerMisiles.x) + (transform.up * offsetPlayerMisiles.y));
		
		//Si estoy parado o casi, me giro hacia el target
		//if(controller.velocity.magnitude < 5)
		{
			//Rotación que debo adoptar para mirar hacia direction
			Quaternion rotation = Quaternion.LookRotation(direction);
			
	        while (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecisionRotacion)
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
		else
		{
			//El enemigo ha muerto antes de poder disparar
			if(trace)
				print("target nulo");
		}
		
		
    } // end dispararMisil()
	
	
	/// <summary>
	/// Disparar el misil teledirigido.
	/// </summary>
	/// <returns>
	/// Nothing
	/// </returns>
	/// <param name='target'>
	/// Target.
	/// </param>
	/// <param name='offsetTarget'>
	/// Offset target.
	/// </param>
	IEnumerator dispararMisilTelediridigo(Transform target, Vector3 offsetTarget)
    {
		//Calculo la dirección del proyectil hacia el target
		Vector3 direction = (target.position + (target.forward * offsetTarget.z) + (target.right * offsetTarget.x) + (target.up * offsetTarget.y)) - 
			(tr.position + (transform.forward * offsetPlayerMisiles.z) + (transform.right * offsetPlayerMisiles.x) + (transform.up * offsetPlayerMisiles.y));
		
		//Si estoy parado o casi, me giro hacia el target
		//if(controller.velocity.magnitude < 5)
		{
			//Rotación que debo adoptar para mirar hacia direction
			Quaternion rotation = Quaternion.LookRotation(direction);
			
	        while (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecisionRotacion)
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
		else
		{
			//El enemigo ha muerto antes de poder disparar
			if(trace)
				print("target nulo");
		}
		
		
    } // end dispararMisilTeledirigido()
	
	/// <summary>
	/// Se llama cada vez que hay que dibujar la interfaz de usuario ( Similar a Update() )
	/// </summary>
    void OnGUI() 
	{
        Event e = Event.current;
		//Controlamos la rueda del ratón
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
		
		if(playerState == PlayerState.death)
		{
			//Skin
			GUI.skin = gameGuiSkin;
			
			float AnchoDialogo = Screen.width / 6;	// 1/6 del ancho de la pantalla
			float AltoDialogo = Screen.height / 10;	// 1/10 del alto de la pantalla
			
			//Area Principal centrada en la pantalla, con título y estilo de caja skinned
			//BeginArea (screenRect : Rect, text : String, style : GUIStyle)
			GUILayout.BeginArea (new Rect (Screen.width / 2 - AnchoDialogo / 2, Screen.height / 2 - AltoDialogo / 2, AnchoDialogo, AltoDialogo), "Has Muerto", gameGuiSkin.box);
			
				//Botón estilo skinned
				if(GUILayout.Button ("Aceptar", gameGuiSkin.button))
				{
					Application.Quit();
				}			
	
	
			GUILayout.EndArea ();			
		}
			
	}	// end OnGui()
	
	/// <summary>
	/// Función que hace que el personaje se oriente progresivamente hacia la dirección pasada como parámetro
	/// </summary>
	/// <returns>
	/// Devuelve true si se ha alcanzado la rotación
	/// </returns>
	/// <param name='direction'>
	/// Dirección hacia la que debe orientarse el personaje
	/// </param>
	bool LookThrough(Vector3 direction)
    {
		//Rotación que debo adoptar para mirar hacia direction
		Quaternion rotation = Quaternion.LookRotation(direction);
		
        if (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecisionRotacion)
		{
			//Rotamos descartando cualquier rotación que no sea del eje Y
			tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
			return false;
		}
		return true;
    } // end LookThrough()
	
	/// <summary>
	/// Desplaza al player hacia la posición del parámetro
	/// </summary>
	/// <returns>
	/// Nothing
	/// </returns>
	/// <param name='position'>
	/// Position a la que desplazarse.
	/// </param>
	IEnumerator goTo(Vector3 position)
	{
		bool orientado = false;
		
		//Se ejecuta hasta que estamos lo suficientemente cerca del punto de destino
		while ((position - tr.position).magnitude > umbralPrecisionDesplazamiento)
		{
			if(!orientado)
				orientado = LookThrough(position - tr.position);
			
			Vector3 direction = position - tr.position;
			direction = new Vector3(direction.x, tr.position.y, direction.z);
			//Me desplazo con limitador de velocidad			
			controller.Move(direction.normalized * linealSpeed * Time.deltaTime * Mathf.Clamp(direction.magnitude, 0.1f, 0.75f));
			yield return 0;
			
			//Comprobamos si el jugador sigue pulsando el botón del mouse y actualizamos position en caso positivo
			if(Input.GetButton("Fire1"))
			{
				Vector3 raton = Input.mousePosition;
				//Cursor del ratón en coordenadas de viewPort
				Vector3 view = Camera.main.ScreenToViewportPoint (raton);
				//Rayo desde la vista
				Ray ray = Camera.main.ViewportPointToRay(view);
				RaycastHit hit;
				
				//Se lanza el rayo 
				if (Physics.Raycast(ray, out hit))
				{
					position = hit.point;
					orientado = false;
				}
				
			}
				
		}
	}
	
	//Llamado cuando algo entra en contacto con el trigger
	void OnTriggerEnter(Collider collider)
	{
		if(collider.transform.tag.Contains("enemigo"))
			playerState = PlayerState.death;
	}
		
	
}
