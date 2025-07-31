using UnityEngine;
using Deforestation.Machine;
using Deforestation.UI;
using Deforestation.Recolectables;
using Deforestation.Interaction;
using Cinemachine;
using System;
using Deforestation.Dialogue;
using Deforestation.Checkpoints;
using StarterAssets;
using Deforestation.Player;
using TMPro;
using Deforestation.Dinosaurus;

namespace Deforestation
{
	public class GameController : Singleton<GameController>
	{
		#region Properties
		public RespawnPanel RespawnPanel => _respawnPanel;
		public MidCheckpoint MidCheckpoint => _midCheckpoint;
		public FirstDialogue FirstDialogue => _firstDialogue;
		public InitialStory InitialStory => _initialStory;
		public MachineController MachineController => _machine;
		public HealthSystem HealthSystem => _playerHealth;
		public Inventory Inventory => _inventory;
		public InteractionSystem InteractionSystem => _interactionSystem;
		public TreeTerrainController TerrainController => _terrainController;
		public Camera MainCamera;
		public DaggerHurts DaggerHurts => _dagger;
		public Action<bool> OnMachineModeChange;

		public bool MachineModeOn
		{
			get
			{
				return _machineModeOn;
			}
			private set
			{
				_machineModeOn = value;
				OnMachineModeChange?.Invoke(_machineModeOn);
			}
		}
		#endregion

		#region Fields
		[Header("Dino")]
		[SerializeField] protected Velociraptor[] _velociraptor;
		[Header("Respawn")]
		[SerializeField] protected RespawnPanel _respawnPanel;
		[Header("Checkpoint")]
		[SerializeField] protected MidCheckpoint _midCheckpoint;
		[SerializeField] private Vector3 _savedPlayerPos;
        [SerializeField] private Vector3 _savedMachinePos;
		[Header("Player")]
		private GameObject _thePlayer;
		[SerializeField] protected CharacterController _player;
		[SerializeField] protected HealthSystem _playerHealth;
		[SerializeField] protected Inventory _inventory;
		[SerializeField] protected InteractionSystem _interactionSystem;

		[Header("Camera")]
		[SerializeField] protected CinemachineVirtualCamera _virtualCamera;
		[SerializeField] protected Transform _playerFollow;
		[SerializeField] protected Transform _machineFollow;

		[Header("Machine")]
		private GameObject _theMachine;
		[SerializeField] protected MachineController _machine;
		[Header("UI")]
		[SerializeField] protected FirstDialogue _firstDialogue;
		[SerializeField] protected UIGameController _uiController;
		[SerializeField] protected InitialStory _initialStory;
		[Header("Trees Terrain")]
		[SerializeField] protected TreeTerrainController _terrainController;
		[Header("Dinosaurs")]
		[SerializeField] protected DaggerHurts _dagger;

		private bool _machineModeOn;
		private Quaternion _originalPlayerRotation;
        #endregion

        #region Unity Callbacks

        void Start()
		{
			SaveCheckpoint();
			_midCheckpoint.OnCheckpoint += SaveCheckpoint;
				
			_machine.OnTextSalirMaquina += _uiController.TextSalirMaquina;
			_machine.WeaponController.OnNoCrystals += _uiController.NotEnoughCrystals;
            _machine.MachineMovement.OnNoCrystals += _uiController.NotEnoughCrystals;
			_machine.WeaponController.OnNoCrystals += _uiController.NotEnoughCrystals;
			
			_playerHealth.OnHealthChanged += _uiController.UpdatePlayerHealth;
			_machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
			MachineModeOn = false;
			_originalPlayerRotation =_player.transform.rotation;
            _firstDialogue.OnNextImage += _initialStory.ShowNextImage;
			_firstDialogue.OnFinishImages += _initialStory.NoImages;

			_playerHealth.OnDeath += () => Died_VariablesforRevive();
			_respawnPanel.OnRevive += () => Revive();

           _velociraptor = FindObjectsOfType<Velociraptor>();

            foreach (var v in _velociraptor)
            {
                v.OnHurt += _uiController.HurtText;
            }



        }
		#endregion

		#region Public Methods
		public void SaveCheckpoint()
		{
			_savedPlayerPos = _inventory.transform.position;
			_savedMachinePos = _machine.transform.position;

		}
		public void Died_VariablesforRevive()
		{
			Debug.Log("Died eventSys"); 
			_playerHealth.OnDeath += _respawnPanel.Died;
			_playerHealth.OnDeath += _inventory.RestartCrystals;
			_playerHealth.OnDeath += _playerHealth.RevivedHealth;
			_playerHealth.OnDeath += _machine.HealthSystem.RevivedHealth;
		
			_player.GetComponent<FirstPersonController>().enabled = false; 
			_machine.GetComponent<MachineMovement>().enabled = false; 
			
            _playerHealth.OnDeath += () => TeleportPlayer(_savedPlayerPos); 
            _machine.transform.position = _savedMachinePos;

        }

		public void Revive()
		{
			_playerHealth.OnHealthChanged += _uiController.UpdatePlayerHealth;
			_machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;

            _player.GetComponent<FirstPersonController>().enabled = true; 
            _machine.GetComponent<MachineMovement>().enabled = true;

			_player.GetComponent<DetectsWater>().NotWater();

        }
		public void TeleportPlayer(Vector3 target)
		{
			_player.enabled = false;
			_player.transform.position = target;
			_player.enabled = true;
		}

		internal void MachineMode(bool machineMode)
		{
            MachineModeOn = machineMode;

			_player.gameObject.SetActive(!machineMode);
			_player.enabled = !machineMode;

			if (machineMode)
			{
                _uiController.TextSalirMaquina();
                if (Inventory.HasResource(RecolectableType.HyperCrystal))
					_machine.StartDriving(machineMode);

				_player.transform.parent = _machineFollow;
				_uiController.HideInteraction();
				Cursor.lockState = CursorLockMode.None;
				_virtualCamera.Follow = _machineFollow;

				_machine.enabled = true;
				_machine.WeaponController.enabled = true;
				_machine.GetComponent<MachineMovement>().enabled = true;

            }
			else
			{
				_machine.enabled = false;
				_machine.WeaponController.enabled = false;
				_machine.GetComponent<MachineMovement>().enabled = false;
				_player.transform.rotation = _originalPlayerRotation;
				_player.transform.parent = null;

                _virtualCamera.Follow = _playerFollow;
				Cursor.lockState = CursorLockMode.Locked;
			}
			Cursor.visible = machineMode;
		}
		#endregion

	}

}