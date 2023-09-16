using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public InputManager inputManager;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            inputManager.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            inputManager.LookInput(virtualLookDirection);
        }
    }
}
