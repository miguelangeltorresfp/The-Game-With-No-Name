using UnityEngine;
using System;
using System.Collections;

///////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// Implementa un sistema rudimentario de pathFinding para un character enemigo 
/// 
///////////////////////////////////////////////////////////////////////////////////////////////////


/// <summary>
/// Enemigo class.
/// </summary>
public class enemigoClass : MonoBehaviour 
{
	#region Declaración de variables miembro
	/// <summary>
	/// Indica si estoy en modo depuración.
	/// </summary>
	public bool trace;
	
	/// <summary>
	/// Transform del player a acompañar
	/// </summary>
	public Transform trPlayer;
	
	/// <summary>
	/// Mi Transform
	/// </summary>
	protected Transform tr;
	
	/// <summary>
	/// Distancia de seguimiento
	/// </summary>
	public float distanceToPlayer = 3;
	
	/// <summary>
	/// Velocidad de desplazamiento
	/// </summary>
	public float maxSpeed = 6.0F;
	
	/// <summary>
	/// Velocidad de desplazamiento utilizada internamente
	/// </summary>
	protected float speedMove;
	
	/// <summary>
	/// Gravedad
	/// </summary>
	public float gravity = 20.0F;
	
	/// <summary>
	/// Dirección del movimiento
	/// </summary>
	protected Vector3 moveDirection = Vector3.zero;
	
	/// <summary>
	/// Character controller
	/// </summary>
	protected CharacterController controller;
	
	/// <summary>
	/// Velocidad de rotación
	/// </summary>
	public float rotationSpeed = 400.0f;
	
	/// <summary>
	/// Precisión en las rotaciones.
	/// </summary>
	public float umbralPrecision = 0.05f;
	
	/// <summary>
	/// Distancia de visión del companion.
	/// </summary>
	public float distanciaVision = 5.0f;
	
	/// <summary>
	/// Dirección actual de barrido del radar.
	/// </summary>
	protected Vector3 direction_radar;
	
	/// <summary>
	/// Intervalo entre rotaciones (en grados).
	/// </summary>
	public float angleStep = 5;
	
	/// <summary>
	/// Número de direcciones a escanear en modo prefixed
	/// </summary>
	protected const int numDirections = 8;
	
	/// <summary>
	/// Direcciones a escanear en modo prefixed
	/// </summary>
	protected Vector3[] directions;
	
	
	/// <summary>
	/// Abanico de colores a utilizar
	/// </summary>
	Color[] color = {Color.red, Color.cyan, Color.magenta, Color.green, Color.blue, Color.white, Color.grey, Color.yellow, Color.red, Color.cyan, Color.magenta, Color.green};
	
	//Offsets desde la posición que lanzo el rayo
	public Vector3 offset = new Vector3(0, 0.5f, 0);
	
	//Estado del companion
	protected enum EvasionState { free, histeresis, orienting, blocked };
	protected EvasionState evasionState = new EvasionState();
	
	//Remanencia para el ciclo de histéresis
	public float remanenciaInicial = 3.0f;
	protected float remanenciaActual;
	
	//Mácara para el raycast
	protected int layerMask;
	
	public Material[] material_toon_normal;
	public Material[] material_toon_outlined;
	
	protected healthClass healthController;
	
	#endregion
	
	void Start()
	{
		//Sólo detecta la capa 9
		layerMask = 1 << 9;
		//Detecta todo excepto la capa 9
		layerMask = ~layerMask;
		
		//Asigno mi transform
		tr = this.gameObject.transform;
		
		GameObject.FindWithTag("Display").SendMessage("liveEnemy");
		
		//Asigno mi character controller
		controller = this.gameObject.GetComponent<CharacterController>();
		
		direction_radar = Vector3.forward;	//Frente (coordenadas de mundo)
		
		//Inicializo las direcciones en un array temporal que luego será copiado a direction[] con un orden especial
		Vector3[] temp_directions = new Vector3[numDirections];
		temp_directions[0] = Vector3.forward;		//Frente (coordenadas de mundo)
		
		//Dinesión del array de direciones
		directions = new Vector3[numDirections];
		directions[0] = Vector3.forward;		//Frente (coordenadas de mundo)
		
		float prefixedAngleStep = 360 / numDirections;
		
		//Se puebla el array con las direcciones
		for(int currDirection = 1; currDirection < numDirections; currDirection++)
			temp_directions[currDirection] = Quaternion.Euler(0, prefixedAngleStep, 0) * temp_directions[currDirection - 1];
		
		
		//Variables auxiliares
		
		//Índice que aumentará a favor de las agujas del reloj
		int firstIndex = 1;
		
		//Índice que disminuirá en contra de las agujas del reloj
		int lastIndex = numDirections - 1;
		
		//Este bucle copia a directions[] el array temporal reordenándolo 
		//de manera que se lancen primero los rayos hacia la parte delantera del companion
		for(int currDirection = 1; currDirection < numDirections; currDirection++)
		{
			if(currDirection % 2 != 0)
			{
				directions[currDirection] = temp_directions[firstIndex];
				firstIndex++;
			}
			else
			{
				directions[currDirection] = temp_directions[lastIndex];
				lastIndex--;
			}
		}
		
		//Inicialización de estado y velocidad
		speedMove = maxSpeed;
		evasionState = EvasionState.free;
		
		healthController = GetComponent<healthClass>();
		
		
	}	//	end	Start()
	
	
	void Update() 
	{
		
		//Si no hay player al que seguir, no hacer nada
		if(trPlayer == null)
			return;
		
		//Si hay obstáculos
		if(checkCollision())
			moveDirection = prefixedScan();	//PrefixedScan me devuelve una posición de escape
		
		//Radar (sólo visual, no tiene ningún efecto)
		//radar();	

		
		//Comprueba el estado del companion
		if(evasionState == EvasionState.orienting)
		{
			if(trace)
				Debug.DrawRay(tr.position, tr.forward * distanciaVision, Color.magenta);
			if(LookThrough(moveDirection))	//Continúo orientándome hasta llegar a la dirección requerida
			{
				evasionState = EvasionState.histeresis;	//Entro en histéresis
				remanenciaActual = remanenciaInicial;	//Asigno la remanencia inicial
			}
		}
		else if(evasionState == EvasionState.blocked)
		{
			moveDirection = prefixedScan();	//Si estoy bloqueado busco una dirección de escape
			speedMove = 0;	//Me detengo
		}
		else if(evasionState == EvasionState.histeresis)
		{
			if(trace)
				Debug.DrawRay(tr.position, tr.forward * distanciaVision, Color.yellow);
			
			//Me muevo hacia adelante
			moveDirection = tr.forward;
			//Disminuyo la velocidad de movimiento
			speedMove = maxSpeed / 1;
			//Disminuyo la remanancia
			remanenciaActual -= Time.deltaTime;
			//Si ya no hay remanencia, dejo libre al companion
			if(remanenciaActual <= 0)
				evasionState = EvasionState.free;
		}
		else if(evasionState == EvasionState.free)
		{
			//Actúo normalmente, obtengo la dirección de movimiento
			moveDirection = GetDirection();
			//restablezco la velocidad
			speedMove = maxSpeed;
		}
		
		// Me desplazo
		if(moveDirection != Vector3.zero)
		{
		    controller.Move(moveDirection.normalized * ( speedMove * ((healthController.health * 100) / healthController.maxHealth) / 100 ) * Time.deltaTime);
		}
		
		
	}	//	end Update()
	
	/// <summary>
	/// Gets the direction.
	/// </summary>wda
	Vector3 GetDirection()
	{
		Vector3 moveDirection = Vector3.zero;
		
		//Lugar al que debo dirigir mi movimiento
        Vector3 toRightPlayer = trPlayer.position + (trPlayer.right * distanceToPlayer);
		
		//Dirección al lugar de movimiento
		moveDirection = toRightPlayer - tr.position;
		//Descarto el componente Y de la dirección
		moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
		
		//Me oriento hacia moveDirection
		if(moveDirection != Vector3.zero)
			LookThrough((moveDirection));
		
		//Actualizo gravedad
		moveDirection.y = -gravity * Time.deltaTime;
		
		
		return moveDirection;

	}	//	end GetDirection()
	
	/// <summary>
	/// Comprueba si hay colisión frontal
	/// </summary>
	/// <returns>
	/// Devuelve true si hay colisión
	/// </returns>
	bool checkCollision()
	{
		RaycastHit hit;
		Vector3 fwd = tr.TransformDirection(directions[0]);
		if(trace)
			Debug.DrawRay(tr.position + offset, fwd * distanciaVision, Color.red);
		return Physics.Raycast (tr.position + offset, fwd, out hit, distanciaVision, layerMask);
	}
	
	/// <summary>
	/// Función que hace que el companion se oriente progresivamente hacia la dirección pasada como parámetro
	/// </summary>
	/// <returns>
	/// Devuelve true si se ha alcanzado la rotación
	/// </returns>
	/// <param name='direction'>
	/// Dirección hacia la que debe orientarse el companion
	/// </param>
	bool LookThrough(Vector3 direction)
    {
		//Rotación que debo adoptar para mirar hacia direction
		Quaternion rotation = Quaternion.LookRotation(direction);
		
        if (Mathf.Abs(tr.eulerAngles.y - rotation.eulerAngles.y) > umbralPrecision)
		{
			tr.rotation = Quaternion.RotateTowards(tr.rotation, rotation, Time.deltaTime * rotationSpeed * direction.magnitude);
			return false;
		}
		return true;
    } // end LookThrough()

	//Barrido por pasos de angleStep grados (sólo visual) alrederdor del objeto (debe llamarse desde Update() )
	void radar() 
	{
		Debug.DrawRay(tr.position, direction_radar.normalized * distanciaVision, Color.green);
		//Al multiplicar una rotación (Quaternion) por un vector3, me devuelve un Vector3 debido a la sobrecarga del operador *
		//Ver documentación de unity en http://docs.unity3d.com/Documentation/ScriptReference/Quaternion-operator_multiply.html
 		direction_radar = Quaternion.Euler(0, angleStep, 0) * direction_radar;
		
	}	//	end	radar()
	
	
		
	/// <summary>
	/// Realiza un scan en numDirections direcciones en torno al objeto.
	/// En caso de obstáculo, rota el transform hacia la primera dirección libre que encuentra.
	/// </summary>
	/// <returns>
	/// Devuelve true si encuentra un obstáculo.
	/// </returns>
	Vector3 prefixedScan()
	{
		if(trace)
			Debug.Log("Escaneando...");
		
		// bucle de scanning
		for(int index = 0; index < numDirections; index++)
		{
			Vector3 fwd = tr.TransformDirection(directions[index]);
			
			//Dibujo un rayo (debug)
			if(trace)
				Debug.DrawRay(tr.position + offset, fwd * distanciaVision, color[index]);			
			
			//Lanzo el rayo
			RaycastHit hit;
			bool impacto = Physics.Raycast (tr.position + offset, fwd, out hit, distanciaVision, layerMask);
			
			//si encuentro una dirección libre (sin impacto dentro de mi distancia de visión)
			if(!impacto)
			{
				//Disminuyo la velocidad
				speedMove = maxSpeed / 1;
				//Actualizo estado
				evasionState = EvasionState.orienting;
				//Devuelvo la dirección de escape
				return fwd;
			}
		}
		
		//Bloqueo. (No hay ruta de escape)
		evasionState = EvasionState.blocked;
		return tr.forward;
		
	} // end prefixedScan
	
	
	
	void OnMouseEnter() 
	{
		
        transform.Find("body_enemy").renderer.materials = material_toon_outlined;
    }	
		
	void OnMouseExit() 
	{
		
        transform.Find("body_enemy").renderer.materials = material_toon_normal;
    }	
		
	void OnDestroy()
	{
		GameObject go = GameObject.FindWithTag("Display");
		if(go != null)
			go.SendMessage("deadEnemy");
	}
	
	
}