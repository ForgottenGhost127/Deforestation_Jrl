using UnityEngine;
using DG.Tweening;
using System;
using Deforestation.Recolectables;

namespace Deforestation.Audio
{
	public class AudioController : MonoBehaviour
	{
		const float MAX_VOLUME = 0.1f;

		#region Fields
		[Header("FX")]
		[SerializeField] private AudioSource _brokenNoise;
		[SerializeField] private AudioSource _machineMotor;
		[SerializeField] private AudioSource _steps;
		[SerializeField] private AudioSource _machineOn;
		[SerializeField] private AudioSource _machineOff;
		[SerializeField] private AudioSource _shoot;
		[SerializeField] private AudioSource _grabObject;


		[Space(10)]

		[Header("Music")]
		[SerializeField] private AudioSource _musicMachine;
		[SerializeField] private AudioSource _musicHuman;
        #endregion

        #region Unity Callbacks	
        private void Awake()
		{
			GameController.Instance.OnMachineModeChange += SetMachineMusicState;
			GameController.Instance.MachineController.OnMachineDriveChange += SetMachineDriveEffect;
			GameController.Instance.MachineController.WeaponController.OnMachineShoot += ShootFX;
			GameController.Instance.InteractionSystem.OnCollect += GrabObject;
			GameController.Instance.FirstDialogue.OnBrokenNoise += BrokenNoise;
			

        }		

		private void Start()
		{
			_musicHuman.Play();
		}

		private void Update()
		{
			//TODO: MOVER A inputcontroller
			if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
			{
				if (!_steps.isPlaying)
					_steps.Play();
			}
			else
				if (_steps.isPlaying)
					_steps.Stop();

		}
		#endregion

		#region Private Methods
		private void SetMachineMusicState(bool machineMode)
		{
			if (machineMode)
			{
				_musicHuman.DOFade(0, 3);
				_musicMachine.DOFade(MAX_VOLUME, 3);
				_musicMachine.Play();
			}
			else
			{
				_musicHuman.DOFade(MAX_VOLUME, 3);
				_musicMachine.DOFade(0, 3);
			}
		}

		private void SetMachineDriveEffect(bool startDriving)
		{
			if (startDriving)
			{
				_machineOn.Play();
				_machineMotor.Play();

			}
			else
			{
				_machineMotor.Stop();
				_machineOff.Play();
			}

		}
		private void ShootFX()
		{
			_shoot.Play();
		}

		private void GrabObject()
		{
			_grabObject.Play();
		}

		private void BrokenNoise()
		{
			_brokenNoise.Play();
		}
		#endregion

	}

}