using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [SerializeField]
    private FakeInventorySO inventoryData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            //Debug.Log($"Picking up item: {item.InventoryItem.Name} | Quantity: {item.Quantity}");

            // Disable the collider immediately to prevent double triggering
            collision.enabled = false;

            int remainder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
            if (remainder == 0)
            {
                item.DestroyItem();
            }
            else
            {
                item.Quantity = remainder;
                collision.enabled = true; // Re-enable collider if not fully picked up
                //Debug.Log($"Item not fully picked up. Remaining quantity: {remainder}");
            }
        }
    }
}
