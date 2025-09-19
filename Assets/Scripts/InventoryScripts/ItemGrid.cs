using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

public class ItemGrid : MonoBehaviour
{
    [SerializeField]
    int gridSizeWidth;
    [SerializeField]
    int gridSizeHeight;
    [SerializeField]
    GameObject item;

    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    Vector2 posOnGrid = new Vector2();
    Vector2Int tileGridPos = new Vector2Int();

    RectTransform rectTransform;
    InventoryItem[,] inventoryItemSlot;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }
    public Vector2Int GetGridPosition(Vector2 mousePos)
    {
        posOnGrid.x = mousePos.x - rectTransform.position.x;
        posOnGrid.y = rectTransform.position.y - mousePos.y;

        tileGridPos.x = (int)(posOnGrid.x / tileSizeWidth);
        tileGridPos.y = (int)(posOnGrid.y / tileSizeHeight);

        return tileGridPos;
    }

    public void InsertItem(InventoryItem itemToInsert)
    {
        if (itemToInsert.position.x != -1)
        { PlaceItem(itemToInsert, itemToInsert.position.x, itemToInsert.position.y); }
        else
        {
            Vector2Int? posOnGrid = FindSpaceForItem(itemToInsert);

            if (posOnGrid == null)
            { return; }

            PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }
    }

    Vector2Int? FindSpaceForItem(InventoryItem itemToInsert)
    {
        for(int y = 0; y < gridSizeHeight - itemToInsert.itemData.height+1; y++)
        {
            for(int x = 0; x < gridSizeWidth - itemToInsert.itemData.width+1; x++)
            {
                if(CheckSpace(x, y, itemToInsert.itemData.width, itemToInsert.itemData.height))
                { return new Vector2Int(x, y); }
            }
        }
        return null;
    }

    public InventoryItem GrabItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
        { return null; }

        CleanGridReference(toReturn);

        return toReturn;
    }

    private void CleanGridReference(InventoryItem toReturn)
    {
        for (int ix = 0; ix < toReturn.itemData.width; ix++)
        {
            for (int iy = 0; iy < toReturn.itemData.height; iy++)
            {
                inventoryItemSlot[toReturn.onGridPosX + ix, toReturn.onGridPosY + iy] = null;
            }
        }
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (!BoundryCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height))
        { return false; }

        if (OverlapCheck(posX, posY, inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlapItem) == false)
        { overlapItem = null; return false; }

        if (overlapItem != null)
        { CleanGridReference(overlapItem); }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    private void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);
        //Debug.Log("Grid " + gameObject.name + " is adding " + inventoryItem.name + " to it's inventory");
        for (int x = 0; x < inventoryItem.itemData.width; x++)
        {
            for (int y = 0; y < inventoryItem.itemData.height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }
        inventoryItem.onGridPosX = posX;
        inventoryItem.onGridPosY = posY;
        inventoryItem.position = new Vector2Int(posX, posY);
        Vector2 pos = FindPositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = pos;
    }

    public Vector2 FindPositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 pos = new Vector2();
        pos.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.itemData.width / 2;
        pos.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.itemData.height / 2);
        return pos;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    if (overlapItem == null)
                    { overlapItem = inventoryItemSlot[posX + x, posY + y]; }
                    else
                    {
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y])
                        { return false; }
                    }
                }
            }
        }
        return true;
    }

    private bool CheckSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {return false;}
            }
        }
        return true;
    }

    bool PosCheck(int posX, int posY)
    {
        if(posX < 0 || posY < 0) 
        { return false; }

        if(posX >= gridSizeWidth || posY >= gridSizeHeight) 
        { return false; }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(!PosCheck(posX, posY))
        { return false; }

        posX += width-1;
        posY += height-1;

        if(!PosCheck(posX, posY))
        { return false; }

        return true;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    public void EmptyGrid()
    {
        for (int x = 0; x < gridSizeWidth; x++)
        {
            for (int y = 0; y < gridSizeHeight; y++)
            { inventoryItemSlot[x, y] = null; }
        }
    }
}
