using UnityEngine.InputSystem;
using UnityEngine;

public class MyInputHandler : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;

    public InputActionReference interact;
    public InputActionReference direction;
    public InputActionReference mouse;
    public InputActionReference sprint;
    public InputActionReference jump;
    public InputActionReference inventory;

    public bool isSprint = false;
    public bool OpenInventory = false;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Awake()
    {
        interact.action.performed += InteractButton;
        sprint.action.performed += SprintButton;
        jump.action.performed += JumpButton;
        inventory.action.performed += InventoryButton;
    }

    private void InteractButton(InputAction.CallbackContext obj)
    {}
    private void InventoryButton(InputAction.CallbackContext obj)
    { OpenInventory = !OpenInventory; }

    public void SprintButton(InputAction.CallbackContext obj)
    { isSprint = !isSprint; }

    public void JumpButton(InputAction.CallbackContext obj)
    {}

    private void Update()
    {
        move = direction.action.ReadValue<Vector2>();
        look = mouse.action.ReadValue<Vector2>();
    }
}
