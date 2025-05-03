using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory.Model;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquippableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Equip";

        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }

        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            Debug.Log($"Attempting to equip: {Name}");

            // Locate the Player GameObject explicitly
            GameObject player = GameObject.Find("Player"); // Ensure the Player GameObject is named "Player" in the scene
            if (player == null)
            {
                Debug.LogError("Player GameObject not found.");
                return false;
            }

            // Get the AgentWeapon component from the Player
            AgentWeapon weaponSystem = player.GetComponent<AgentWeapon>();
            if (weaponSystem != null)
            {
                // Equip the weapon
                weaponSystem.SetWeapon(this, itemState == null ? DefaultParametersList : itemState);
                Debug.Log($"Weapon equipped: {Name}");
                return true;
            }
            else
            {
                Debug.LogError("No AgentWeapon found on the Player GameObject.");
                return false;
            }
        }
    }
}
