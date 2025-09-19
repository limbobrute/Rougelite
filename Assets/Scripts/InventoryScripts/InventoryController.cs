using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    //[HideInInspector]
    [SerializeField]
    private ItemGrid selectedItemGrid;

    public ItemGrid SelectedItemGrid 
    { 
        get => selectedItemGrid; 
        set { 
                selectedItemGrid = value;
                highlighter.SetParent(value);
        } 
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    InventoryItem ItemToHighlight;
    InventoryHighlight highlighter;
    RectTransform rectTransform;

    [SerializeField]
    List<ItemData> items;
    [SerializeField]
    GameObject item;
    [SerializeField]
    Transform canvasTransform;

    Vector2Int oldPos;

    private void Awake()
    {
        highlighter = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        ItemIconDrag();


        if(Input.GetKeyDown(KeyCode.Q))
        { CreateRandomItem(); }

        if (selectedItemGrid == null)
        { highlighter.Show(false); return; }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    void HandleHighlight()
    {
        Vector2Int PosOnGrid = GetTileGridPos();

        if(oldPos == PosOnGrid)
        { return; }

        oldPos = PosOnGrid;
        if (selectedItem == null)
        {
            ItemToHighlight = selectedItemGrid.GetItem(PosOnGrid.x, PosOnGrid.y);
            if (ItemToHighlight != null)
            {
                highlighter.Show(true);
                highlighter.SetSize(ItemToHighlight);
                highlighter.SetPos(selectedItemGrid, ItemToHighlight);
            }
            else
            { highlighter.Show(false); }
        }
        else
        {
            highlighter.Show(selectedItemGrid.BoundryCheck(PosOnGrid.x, PosOnGrid.y, selectedItem.itemData.width, selectedItem.itemData.height));
            highlighter.SetSize(selectedItem);
            highlighter.SetPos(selectedItemGrid, selectedItem, PosOnGrid.x, PosOnGrid.y);
        }
    }

    void CreateRandomItem()
    {
        InventoryItem inventoryitem = Instantiate(item).GetComponent<InventoryItem>();
        rectTransform = inventoryitem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        selectedItem = inventoryitem;
        
        int selectedItemID = Random.Range(0, items.Count);
        inventoryitem.Set(items[selectedItemID]);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int posOnGrid = GetTileGridPos();

        if (selectedItem == null)
        {
            PickUpItem(posOnGrid);
        }
        else
        {
            PlaceItem(posOnGrid);
        }
    }

    private Vector2Int GetTileGridPos()
    {
        Vector2 pos = Input.mousePosition;

        if (selectedItem != null)
        { pos.x -= (selectedItem.itemData.width - 1) * ItemGrid.tileSizeWidth / 2; pos.y += (selectedItem.itemData.height - 1) * ItemGrid.tileSizeHeight / 2; }

        return selectedItemGrid.GetGridPosition(pos);
    }

    private void PlaceItem(Vector2Int posOnGrid)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, posOnGrid.x, posOnGrid.y, ref overlapItem);
        if (complete)
        {
            selectedItem = null;
            if(overlapItem !=null)
            { 
                selectedItem = overlapItem; 
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
            }
            //rectTransform = null;
        }
    }

    private void PickUpItem(Vector2Int posOnGrid)
    {
        selectedItem = selectedItemGrid.GrabItem(posOnGrid.x, posOnGrid.y);
        if (selectedItem != null)
        { rectTransform = selectedItem.GetComponent<RectTransform>(); }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        { rectTransform.position = Input.mousePosition; }
    }
}
