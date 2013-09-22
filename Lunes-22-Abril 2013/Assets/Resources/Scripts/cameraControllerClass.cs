using UnityEngine;
using System.Collections;

public class cameraControllerClass : MonoBehaviour
{
	#region Variables/Properties Initialization

	/// <summary>
	/// Indica si se va a depurar el código
	/// </summary>
	public bool debug = false;
	
	/// <summary>
	/// Referencia al player que debemos seguir
	/// </summary>
	public GameObject player;
	
	/// <summary>
	/// Referencia al texto
	/// </summary>
	public GUIText Texto;
	
	//Altura y distancia con respecto al player
	public float height = 5, distance = 10;
	
	/// <summary>
	/// Transform de la camara
	/// </summary>
	protected Transform trPlayer;
	
	/// <summary>
	/// Diferencia de posición entre el player y esta cámara al comienzo del juego
	/// </summary>
	protected Vector3 diff;
	
	/// <summary>
	/// mi Transform
	/// </summary>
	protected Transform tr;

	/// <summary>
	/// Tiempo que tiene que tardar la cámara en transitar de un extremo al otro del zoom
	/// </summary>
	public float timeTransition = 1;
	
	/// <summary>
	/// Curva de transition.
	/// </summary>
	public AnimationCurve transitionZoom;
	
	/// <summary>
	/// Field of view en posiciones extremas del zoom
	/// </summary>
	public float fovNear = 25, fovFar = 50;
	
	/// <summary>
	/// Enumeración que indica en que posición de zoom está la cámara.
	/// </summary>
	protected enum ZommState { near, far };
	
	/// <summary>
	/// The state of the zoom.
	/// </summary>
	ZommState zoomState = new ZommState ();
	
	/// <summary>
	/// Enumeración que indica si el zoom está trabajando.
	/// </summary>
	protected enum WorkingState { idle, busy };
	
	/// <summary>
	/// The state of the zoom.
	/// </summary>
	WorkingState workingSate = new WorkingState ();
	
	public enum ViewType {editor, custom};
	public ViewType viewType = ViewType.editor;
	
	#endregion
	
	void Start ()
	{
		//Por defecto, al principio el zoom está alejado
		zoomState = ZommState.far;
		camera.fieldOfView = fovFar;
		
		//Cámara en reposo
		workingSate = WorkingState.idle;
		
		//Asigno los transforms
		tr = this.gameObject.transform;
		trPlayer = player.transform;
		
		if(viewType == ViewType.custom)
			//Me posiciono con respecto al player
			tr.position = new Vector3 (trPlayer.position.x, trPlayer.position.y + height, trPlayer.position.z + distance);
		
		//Calculo la dirección al player
		diff = trPlayer.position - tr.position;
		
		if(viewType == ViewType.custom)
			//Miro al player
			tr.LookAt (trPlayer);
		
	}
	
	void Update()
	{
		if (Input.GetButtonDown ("Zoom") && workingSate == WorkingState.idle)
		{
			//El usuario ha pulsado el botón de zoom
			
			//Cámara ocupada
			workingSate = WorkingState.busy;
			StartCoroutine (makeZoom ());
		}
		// Me desplazo con el player
		if( trPlayer )
		tr.position = trPlayer.position - diff;
	}
	
	
	IEnumerator makeZoom ()
	{
		//Variables de trabajo
		
		//Diferencia entre valores extremos del zoom
		float fovDifference = fovFar - fovNear;
		
		//Cantidad de fov a sumar o restar. Oscila entre cero y fovDifference
		float currentFovQuantity;
		
		//Tiempo actual de evaluación de la curva. Oscila de cero a uno
		float currentTimeEvaluate;
		
		//Tiempo de transición actual. Oscila de cero a timeTransition. Se incrementa en cada bucle con Time.deltaTime
		float currentTransitionTime = 0;
		
		
		while (true)
		{
			//Calculo el lugar donde debo evaluar la curva, determinado por el tiempo (eje x)
			currentTimeEvaluate = currentTransitionTime / timeTransition;
			//La cantidad de fov la devuelve la evaluación de la curva, determinada por la diferencia entre los dos valores extremos del zoom (eje y)
			currentFovQuantity = transitionZoom.Evaluate (currentTimeEvaluate) * fovDifference;
			
			if(debug)
			{
				Texto.text = "Zoom: " + currentFovQuantity.ToString ();
			}
			
			if (zoomState == ZommState.far)
			{
				//Si estoy en far, le resto la cantidad de fov que me haya devuelto la evaluación de la curva
				camera.fieldOfView = fovFar - currentFovQuantity;
				if (currentTimeEvaluate >= 1)
				{
					//Se ha evaluado la curva completamente, cambio de estado y salgo del bucle while
					camera.fieldOfView = fovNear;
					zoomState = ZommState.near;
					break;
				}
			}
			else
			{
				//Si estoy en near, le sumo la cantidad de fov que me haya devuelto la evaluación de la curva
				camera.fieldOfView = fovNear + currentFovQuantity;
				if (currentTimeEvaluate >= 1)
				{
					//Se ha evaluado la curva completamente, cambio de estado y salgo del bucle while
					camera.fieldOfView = fovFar;
					zoomState = ZommState.far;
					break;
				}
			}
			
			//Miro al player
			tr.LookAt (trPlayer);
			//Recalculo el tiempo de transición
			currentTransitionTime += Time.deltaTime;
			//Salgo hasta el próximo update
			yield return 0;
			
		}
		//Miro al player
		tr.LookAt (trPlayer);
		//Cámara en reposo
		workingSate = WorkingState.idle;
	}

}

