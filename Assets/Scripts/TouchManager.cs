using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction jumpAction;
    private PlayerController playerController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = FindObjectOfType<PlayerController>();
        jumpAction = playerInput.actions["Jump"];
    }

    private void OnEnable()
    {
        jumpAction.performed += Jumped;
    }

    private void OnDisable()
    {
        jumpAction.performed -= Jumped;
    }

    private void Jumped(InputAction.CallbackContext context)
    {
        playerController.Jump();
        playerController.StartGame();
    }
}
