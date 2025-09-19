using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEditor.Experimental.GraphView;
using StarterAssets;
using System.Collections.Generic;

public class MyController : MonoBehaviour
{
    public MyInputHandler input;
    public GameObject PlayerInventory;
    public GameObject BoxInventory;
    public GameObject LastBox = null;
    public Transform Camera;

    public FirstPersonController otherController;
    public StarterAssetsInputs otherInput;

    public void turnOffInventory()
    { 
        BoxInventory.SetActive(false);
        PlayerInventory.SetActive(false);
    }

    private void Update()
    {
        if(input.inventory.action.WasPressedThisFrame())
        {
            if(PlayerInventory.activeSelf)
            { 
                PlayerInventory.SetActive(false);
                otherController.enabled = true;
                otherInput.cursorInputForLook = true;
                otherInput.cursorLocked = true;
            }
            else
            { 
                PlayerInventory.SetActive(true);
                BoxInventory.SetActive(false);
                otherController.enabled = false;
                otherInput.cursorInputForLook = false;
                otherInput.cursorLocked = false;
            }
        }

        if(input.interact.action.WasPressedThisFrame()) 
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.position, Camera.TransformDirection(Vector3.forward), out hit, 3))
            {
                GameObject temp = hit.collider.gameObject;
                if(temp.CompareTag("BottomlessBox") && PlayerInventory.activeSelf)
                {
                    BoxInventory.SetActive(false);
                    PlayerInventory.SetActive(false);
                    otherController.enabled = true;
                    otherInput.cursorInputForLook = true;
                    otherInput.cursorLocked = true;
                }
                else
                {
                    if(LastBox == null)
                    { LastBox = temp; }
                    else if(LastBox != temp)
                    {
                        LastBox.GetComponent<ContainerInventory>().hasOpen = false;
                        List<InventoryItem> items = LastBox.GetComponent<ContainerInventory>().GetItems();
                        foreach(InventoryItem item in items)
                        { item.gameObject.SetActive(false); }
                        LastBox = temp; 
                    }
                    var container = temp.GetComponent<ContainerInventory>();
                    var boxGrid = BoxInventory.GetComponent<ItemGrid>();

                    BoxInventory.SetActive(true);
                    PlayerInventory.SetActive(true);
                    otherController.enabled = false;
                    otherInput.cursorInputForLook = false;
                    otherInput.cursorLocked = false;
                    boxGrid.EmptyGrid();
                    if (container.hasOpen == false)
                    {
                        //Debug.Log("Placing items");
                        container.hasOpen = true;
                        List<InventoryItem> items = container.GetItems();
                        foreach (InventoryItem item in items)
                        { item.gameObject.SetActive(true); boxGrid.InsertItem(item); }
                    }

                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(Camera.position, Camera.TransformDirection(Vector3.forward) * 3, Color.red);
    }
}