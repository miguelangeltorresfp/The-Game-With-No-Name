using UnityEngine;
using System.Collections;

public class SingletonManagers
{

	static SingletonManagers instance = null;

	
	//Propiedad que nos devuelve una instancia de SingletonManagers
	public static SingletonManagers Instance
	{
		get
		{
			if( instance == null )
			instance = new SingletonManagers();
			return instance;
		}
	}
	
	//Método que nos devuelve la instancia de SingletonManagers
	/*public static SingletonManagers GetInstance()
	{
		if( instance == null )
			instance=new SingletonManagers();
		return instance;
	}*/
	
	//Hacemos el constructor privado porque si no es público por defecto
	private SingletonManagers()
	{
		//Configuramos la capas
		layerSuelo = 1 << LayerMask.NameToLayer(nombreLayerSuelo);
		layerScenaryStatic = 1 << LayerMask.NameToLayer(nombreLayerScenaryStatic);
		layerPlayer = 1 << LayerMask.NameToLayer(nombreLayerPlayer);
		
		layerMask_IgnoreSphereCast = ~(layerSuelo | layerScenaryStatic);
		layerMask_IgnorePlayer = ~layerPlayer;	
	}
	
//	public CMsgManager MsgManager{get;set;}
//	public CDataManager DataManager{get;set;}
//	public CCamerasManager CamerasManager{get;set;}
	
	
	#region Variables para configurar las capas
	
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
	
	#endregion Variables para configurar las capas
	

					
}
