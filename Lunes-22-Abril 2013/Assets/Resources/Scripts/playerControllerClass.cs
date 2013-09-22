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
public class playerControllerClass : MonoBehaviour 
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
	public float impulsoBala = 10f;
	
	/// <summary>
	/// Enumeración Tipo proyectil.
	/// </summary>
	public enum TipoProyectil {pistola, misilNormal, misilInteligente, ametralladora, granada};
	
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
	/// Desplazamiento en la instaciación de la pistola con respecto al origen del player.
	/// </summary>
	public Vector3 offsetPlayerBalas = new Vector3(0, 0.75f, 0.5f);
	
	/// <summary>
	/// Desplazamiento en la instaciación de la granada con respecto al origen del player.
	/// </summary>
	public Vector3 offsetPlayerGranadas = new Vector3(0f, 0.75f, 0.5f);
	
	/// <summary>
	/// Desplazamiento en la instaciación del misil con respecto al origen del player.
	/// </summary>
	public Vector3 offsetPlayerMisiles = new Vector3(0, 0.75f, 1);
	
	/// <summary>
	/// The offset player rayo deteccion enemigos.
	/// Desplazamiento del rayo que lanzamos para detectar si hay más o menos enemigos 
	/// y usar de ese modo un tipo de disparo u otro a conveniencia.
	/// </summary>
	public Vector3 offsetPlayerRayoDeteccionEnemigos = new Vector3(0, 0.75f, 0 );
	
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
	protected float rotationSpeed = 400f;
	
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
	
	public float distanciaDisparo;
	
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
	
	/// <summary>
	/// The number balas.
	/// </summary>
	protected int numBalas = 100;
	/// <summary>
	/// The max number balas.
	/// </summary>
	protected int maxNumBalas = 100;
	
	/// <summary>
	/// The cargadores ametralladora.
	/// </summary>
	protected int numCargadoresAmetralladora = 10;
	/// <summary>
	/// The max cargadores ametralladora.
	/// </summary>
	protected int maxCargadoresAmetralladora = 10;
	/// <summary>
	/// The number balas cargador Ametralladora.
	/// </summary>
	protected int numBalasCargadorAmetralladora = 10;
	
	/// <summary>
	/// The number granadas.
	/// </summary>
	protected int numGranadas = 1;
	/// <summary>
	/// The max number granadas.
	/// </summary>
	protected int maxNumGranadas = 50;
	
	/// <summary>
	/// The number misiles inteligentes.
	/// </summary>
	protected int numMisilesInteligentes = 2;
	/// <summary>
	/// The max number misiles inteligentes.
	/// </summary>
	protected int maxNumMisilesInteligentes = 2;
	
	/// <summary>
	/// The number misiles normales.
	/// </summary>
	protected int numMisilesNormales = 5;
	/// <summary>
	/// The max number misiles normales.
	/// </summary>
	protected int maxNumMisilesNormales = 5;
	
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		//Para hacer pruebas asigno el modo de disparo inicial
		modoDisparo = ModoDisparo.manual;
		tipoProyectil = TipoProyectil.granada;
		
		//Asigno la distancia de disparo
		if( Screen.width > Screen.height )
			distanciaDisparo = Screen.width/2;
		else distanciaDisparo = Screen.height/2;
		
		//Asigno mi transform
		tr = transform;
		
		//Asigno mi character controller
		controller = GetComponent<CharacterController>();
		//Desabilito colisiones
		//controller.detectCollisions = false;
		
		//Visualiza la munición al principio
		if(tipoProyectil == TipoProyectil.pistola)
			TextoMunicion.text = "Tipo Arma: Pistola";
		else if(tipoProyectil == TipoProyectil.misilNormal)
			TextoMunicion.text = "Tipo Arma: MisilNormal";
		else if(tipoProyectil == TipoProyectil.misilInteligente)
			TextoMunicion.text = "Tipo Arma: MisilInteligente";
		else if(tipoProyectil == TipoProyectil.ametralladora)
			TextoMunicion.text = "Tipo Arma: Ametralladora";
		else if(tipoProyectil == TipoProyectil.granada)
			TextoMunicion.text = "Tipo Arma: Granada";
		
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
			Vector3 raton;
			Vector3 view;
			raton = Input.mousePosition;
			//Cursor del ratón en coordenadas de viewPort
			view = Camera.main.ScreenToViewportPoint (raton);
			//Rayo desde la vista
			Ray ray = Camera.main.ViewportPointToRay(view);
			RaycastHit hit;
			
			//Lanzamos el rayo para calcular la rotación en función de la posición del mouse 
			if (Physics.Raycast(ray, out hit))
			{
				if(trace)
				{
//					print("Apuntando a: " + hit.transform.name);
//					print ("PosicionMouse = " + hit.point);
				}
				
				//Anulamos la componente y de la posición del mouse.
				hit.point = new Vector3( hit.point.x, transform.position.y, hit.point.z );
				
				// Determine the target rotation.  This is the rotation if the transform looks at the target point.
        		Quaternion targetRotation = Quaternion.LookRotation(hit.point - transform.position);
 
	        	// Smoothly rotate towards the target point.
				if (Mathf.Abs(tr.eulerAngles.y - targetRotation.eulerAngles.y) > umbralPrecisionRotacion)
				{
	        		//transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.time);
					//Rotamos descartando cualquier rotación que no sea del eje Y
					tr.rotation = Quaternion.RotateTowards(tr.rotation, Quaternion.Euler(0, targetRotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
				}
			}
				
				
			
			moveDirection = transform.TransformDirection(Vector3.forward) * Input.GetAxis("Vertical");
			//transform.Rotate(0, Input.GetAxis("Horizontal")*rotationSpeed,0);
			
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
		
		#region Actualización Variables Player
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
		//
		// Fin Sección actualización variables del player
		//*********************************************************************************************
		#endregion
		
		#region Disparo Misiles, Granadas, Ametralladora, Pistola
		//*********************************************************************************************
		//
		// Sección que facilita el disparo de misiles, granadas, ametralladora, pistola
		//
		//*********************************************************************************************
		if(Input.GetButtonDown("Fire2"))
		{
			#region Calculo Armamento en modo Automático
			//**************************************************************************
			//Cálculos tan sólo si estamos en modo automático
			//Para saber qué armamento usar 
			//**************************************************************************
			if( modoDisparo == ModoDisparo.auto )
			{
				//Calculo la posición de origen del rayo sumándole el offset correspondiente.
				Vector3 origenRayo = tr.position + (transform.up * offsetPlayerRayoDeteccionEnemigos.y);
				
				//Cojo la dirección forward del player con una determinada magnitud que viene dada por distanciaDisparo
				//En función de esa distancia calcularemos colisiones posibles del rayo a mayor o menor distancia.
				Vector3 target = transform.TransformDirection(Vector3.forward) * distanciaDisparo;
				
				RaycastHit hit;
				
				//Rayo desde el player hacia adelante una distancia determinada 
				//Si hay colisión vemos los enemigos que hay en ese punto para actuar en consecuencia
				//si no hay colisión efectuamos sin más un disparo de bala en esa dirección siempre que estemos
				//en modo automático.
				if ( Physics.Raycast(origenRayo, target, out hit ) )
				{
					//En este caso el target es el lugar de impacto del rayo
					//target = hit.point;
					
					if(trace)
					{
						print("Apuntando a: " + hit.transform.name);
						Debug.DrawRay( origenRayo, target, Color.red, 1 );
					}
					
					//Casteamos en cualquier caso una esfera ignorando el suelo y los elementos estáticos del escenario
					Collider[] colliders = Physics.OverlapSphere(hit.point, 10, layerMask_IgnoreSphereCast);
					
					//Dependiendo del número de enemigos que tengamos cerca usaremos un arma un otro.
					int contadorEnemigos = 0;
					
					//¿Hay algo dentro de la esfera?
					if(colliders.GetLength(0) > 0)
					{
						//Encontramos elementos dentro de la esfera
						int index = 0;
						
						for(index = 0; index < colliders.GetLength(0); index++)
						{
							//Encontramos al menos un enemigo
							if(colliders[index].transform.tag.Contains("enemigo"))
								contadorEnemigos ++;
								//break;
						}
					}
					
					if( trace ) print( "Numero Enemigos = " + contadorEnemigos );
						
					//¿Hay enemigos?
					//Elegimos munición
					if(modoDisparo == ModoDisparo.auto)
					{
						if(contadorEnemigos > 8 && numGranadas > 0)
						{
							tipoProyectil = TipoProyectil.misilNormal;
							TextoMunicion.text = "Tipo Arma: MisilNormal";
						}
						else if(contadorEnemigos > 8 && numMisilesInteligentes > 0)
						{
							tipoProyectil = TipoProyectil.misilInteligente;
							TextoMunicion.text = "Tipo Arma: MisilIngeligente";
						}
						else if(contadorEnemigos > 5 && numGranadas > 0)
						{
							tipoProyectil = TipoProyectil.granada;
							TextoMunicion.text = "Tipo Arma: Granada";
						}
						else if(contadorEnemigos > 3 && numCargadoresAmetralladora > 0)
						{
							tipoProyectil = TipoProyectil.ametralladora;
							TextoMunicion.text = "Tipo Arma: Ametralladora";
						}
						else 
						{
							tipoProyectil = TipoProyectil.pistola;
							TextoMunicion.text = "Tipo Arma: Pistola";
						}
					}
									
				}
				//Si no existe colisión al lanzar el rayo efectuamos sin más un disparo de bala en esa dirección
				//siempre que estemos en modo de disparo automático
				else
				{
					//Elegimos como munición la bala
					if(modoDisparo == ModoDisparo.auto)
					{
						tipoProyectil = TipoProyectil.pistola;
						TextoMunicion.text = "Munici\u00F3n: Bala";
					}	
				}
				
				//Disparo en función del tipo de munición en MODO AUTOMÁTICO
				if(tipoProyectil == TipoProyectil.pistola && numBalas > 0)
				{
					//Pistola
					dispararPistola();
				}
				else if(tipoProyectil == TipoProyectil.ametralladora)
				{
					//Ametralladora
					StartCoroutine(dispararAmetralladora());
				}
				else if(tipoProyectil == TipoProyectil.misilNormal)
				{
					//Misil Normal
					dispararMisilNormal();
				}
			}
			//
			// Fin Sección para saber qué armamento usar en modo automático
			//*********************************************************************************************
			#endregion
			
			#region Disparo Manual
			//**************************************************************************
			//
			// Sección de Disparo Manual
			//
			//**************************************************************************
			if( modoDisparo == ModoDisparo.manual )
			{
				if(tipoProyectil == TipoProyectil.pistola && numBalas > 0)
				{
					//Pistola
					dispararPistola();
				}
				if(tipoProyectil == TipoProyectil.ametralladora && numCargadoresAmetralladora > 0)
				{
					//Ametralladora
					StartCoroutine(dispararAmetralladora());
				}
				if(tipoProyectil == TipoProyectil.granada && numGranadas > 0)
				{
					//Ametralladora
					dispararGranada();
				}
				if(tipoProyectil == TipoProyectil.misilNormal && numMisilesNormales > 0)
				{
					//Misil Normal
					dispararMisilNormal();
				}
				if(tipoProyectil == TipoProyectil.misilInteligente && numMisilesInteligentes > 0)
				{
					//Misil Normal
					dispararMisilNormal();
				}
				
			}
			//
			// Fin Sección de Disparo Manual
			//*********************************************************************************************
			#endregion
			
	
		}
		//Si no está pulsando el botón de disparo recuperamos energía
		else
		{
			playerEnergy ++;
			if( playerEnergy > playerEnergyMax ) playerEnergy = playerEnergyMax;
		}
		
		//
		// Fin Sección que facilita el disparo del player
		//*********************************************************************************************
		#endregion
		
		#region Recargar Munición
		//*********************************************************************************************
		//Sección que facilita el recargar armamento
		//*********************************************************************************************
		if(Input.GetKey(KeyCode.R))
		{
            numBalas = maxNumBalas;
        }

            
		#endregion
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
	protected void dispararPistola()
    {
		//Cojo la dirección forward del player
		Vector3 playerfwd = transform.TransformDirection(Vector3.forward);
		//Instancio bala en mi posición + offsetPlayer. Quaternion.Identity significa "sin rotación"
		GameObject bala = (GameObject) Instantiate(Resources.Load("Prefabs/Bala"), tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), Quaternion.identity);
		//Por cada disparo de bala resto uno al número de balas disponibles.
		numBalas --;
		//Aplicamos impulso a la bala
		bala.rigidbody.AddForce (playerfwd * impulsoBala, ForceMode.Impulse);
		//Habilitamos la estela
		bala.GetComponent<TrailRenderer>().enabled = true;
	
    } //end dispararPistola()
	
	IEnumerator dispararAmetralladora()
	{
		//Cojo la dirección forward del player
		Vector3 playerfwd = transform.TransformDirection(Vector3.forward);
		
		int numBalasCargadorAmetralladoraTemp = numBalasCargadorAmetralladora;
		
		while( numBalasCargadorAmetralladora > 0 )
		{
			//Instancio bala en mi posición + offsetPlayer. Quaternion.Identity significa "sin rotación"
			GameObject bala = (GameObject) Instantiate(Resources.Load("Prefabs/Bala"), tr.position + (transform.forward * offsetPlayerBalas.z) + (transform.right * offsetPlayerBalas.x) + (transform.up * offsetPlayerBalas.y), Quaternion.identity);
			//Por cada disparo de bala resto uno al número de balas disponibles.
			numBalasCargadorAmetralladoraTemp --;
			//Aplicamos impulso a la bala
			bala.rigidbody.AddForce (playerfwd * impulsoBala, ForceMode.Impulse);
			//Habilitamos la estela
			bala.GetComponent<TrailRenderer>().enabled = true;
			//Cuando agotamos el cargador dejo de disparar la ametralladora
			if( numBalasCargadorAmetralladoraTemp <= 0 )
			{
				numCargadoresAmetralladora --;
				break;
			}
			//Actualizo la dirección del target
			//Cojo la dirección forward del player
			playerfwd = transform.TransformDirection(Vector3.forward);
			
			yield return 0;
		}	
	}
	
	/// <summary>
	/// Disparars the granada.
	/// </summary>
	protected void dispararGranada()
	{
		//Cojo la dirección forward del player 
		Vector3 playerfwd = transform.TransformDirection(Vector3.forward) * 1000;
		//Gasto una granada por cada disparo
		//numGranadas --;
		//Instancio una granada en mi posición + offsetPlayer. Quaternion.Identity significa "sin rotación"
		GameObject granada = (GameObject) Instantiate(Resources.Load("Prefabs/Granada"), tr.position + (transform.forward * offsetPlayerGranadas.z) + (transform.right * offsetPlayerGranadas.x) + (transform.up * offsetPlayerGranadas.y), Quaternion.identity);
		
		granada.transform.LookAt(playerfwd);
	}
	
	/// <summary>
	/// Disparars the misil.
	/// </summary>
	/// <returns>
	/// Nothing
	/// </returns>
	/// <param name='target'>
	/// Target.
	/// </param>
	protected void dispararMisilNormal()
    {
		//Cojo la dirección forward del player
		Vector3 playerfwd = transform.TransformDirection(Vector3.forward) * 1000;
		
		//Instancio misil en mi posición + offsetPlayer. 
		GameObject misil = (GameObject) Instantiate(Resources.Load("Prefabs/Misil"), tr.position + (transform.forward * offsetPlayerMisiles.z) + (transform.right * offsetPlayerMisiles.x) + (transform.up * offsetPlayerMisiles.y), Quaternion.identity);
		
		//El misil apunta a la posición forward del player
		misil.transform.LookAt(playerfwd);
			
		//Habilitamos la estela
		misil.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().enabled = true;
		
    } // end dispararMisilNormal()
	
	
	
	/// <summary>
	/// Se llama cada vez que hay que dibujar la interfaz de usuario ( Similar a Update() )
	/// </summary>
    void OnGUI() 
	{
        Event e = Event.current;
		//Controlamos la rueda del ratón
        if (e.type == EventType.scrollWheel && modoDisparo != ModoDisparo.auto)
		{
            if(tipoProyectil == TipoProyectil.pistola)
			{
				tipoProyectil = TipoProyectil.ametralladora;
				TextoMunicion.text = "Tipo Arma: Ametralladora";		
			}
			else if(tipoProyectil == TipoProyectil.ametralladora)
			{
				tipoProyectil = TipoProyectil.granada;
				TextoMunicion.text = "Tipo Arma: Granada";
			}
			else if(tipoProyectil == TipoProyectil.granada)
			{
				tipoProyectil = TipoProyectil.misilNormal;
				TextoMunicion.text = "Tipo Arma: MisilNormal";
			}
			else if(tipoProyectil == TipoProyectil.misilNormal)
			{
				tipoProyectil = TipoProyectil.misilInteligente;
				TextoMunicion.text = "Tipo Arma: MisilInteligente";	
			}
			else 
			{
				tipoProyectil = TipoProyectil.pistola;
				TextoMunicion.text = "Tipo Arma: Pistola";
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
		//if(collider.transform.tag.Contains("enemigo"))
			//playerState = PlayerState.death;
	}
		
	
}
