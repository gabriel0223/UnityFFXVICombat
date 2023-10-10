using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class InputManager : MonoBehaviour
	{
		public event Action OnAttackPressed;
		public event Action OnPhoenixShiftPressed;
		public event Action OnDodgePressed;
		public event Action OnShowAbilitiesPressed;
		public event Action OnShowAbilitiesReleased;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private PlayerInputAsset _playerInputAsset;

		private void Awake()
		{
			_playerInputAsset = new PlayerInputAsset();
		}

		private void OnEnable()
		{
			_playerInputAsset.Enable();

			_playerInputAsset.Player.ShowAbilities.performed += ctx => OnShowAbilitiesPressed?.Invoke();
			_playerInputAsset.Player.ShowAbilities.canceled += ctx => OnShowAbilitiesReleased?.Invoke();
		}

		private void OnDisable()
		{
			_playerInputAsset.Disable();
		}

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnAttack()
		{
			OnAttackPressed?.Invoke();
		}

		public void OnPhoenixShift()
		{
			OnPhoenixShiftPressed?.Invoke();
		}

		public void OnDodge()
		{
			OnDodgePressed?.Invoke();
		}

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
}