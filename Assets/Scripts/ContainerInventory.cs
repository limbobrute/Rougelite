using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerInventory : MonoBehaviour
{
    public bool hasOpen = false;
    public List<ItemData> PossibleItems = new List<ItemData>();
    public GameObject itemPrefab;
    public ItemGrid BoxInventory;
    public MyController controller;

    [SerializeField]
    List<InventoryItem> StoredItems = new List<InventoryItem>();

    private void Start()
    {
        for(int i = 0; i < 10; i++)
        {
                int rand2 = Random.Range(0, PossibleItems.Count);
                InventoryItem toStore = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                toStore.Set(PossibleItems[rand2]);
                RectTransform rectTransform = toStore.GetComponent<RectTransform>();
                rectTransform.SetParent(BoxInventory.gameObject.GetComponent<RectTransform>());
                StoredItems.Add(toStore);
                toStore.gameObject.SetActive(false);
                //BoxInventory.InsertItem(toStore);

        }
        controller.turnOffInventory();
    }

    public List<InventoryItem> GetItems()
    { return StoredItems; }
}
