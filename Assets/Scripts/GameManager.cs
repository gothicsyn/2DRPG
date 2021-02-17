using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public CharStats[] playerStats;

    public bool gameMenuOpen, dialogActive, fadingBetweenArea, shopActive;

    public string[] itemsHeld;
    public int[] numberOfItems;
    public Item[] itemReference;

    public int currentGold;

    // Start is called before the first frame update
    void Start()
    {
        //ensure a single instance is available
        instance = this;

        //ensure data is carried between areas
        DontDestroyOnLoad(gameObject);

        //Set up the inventory
        SortItems();
    }

    // Update is called once per frame
    void Update()
    {
        //if any of listed bools are true, prevent player maovement
        if(gameMenuOpen || dialogActive || fadingBetweenArea || shopActive)
        {
            PlayerController.instance.canMove = false;
        } else
        {
            PlayerController.instance.canMove = true;
        }


        if (Input.GetKeyDown(KeyCode.O))
        {
            SavingData();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            LoadingData();
        }
    }

    public Item GetItemDetails(string itemToGrab)
    {
        for (int i = 0; i < itemReference.Length; i++)
        {
            if(itemReference[i].itemName == itemToGrab)
            {
                return itemReference[i];
            }
        }

            return null;
    }

    public void SortItems()
    {
        bool itemAfterSpace = true;

        while (itemAfterSpace)
        {
            itemAfterSpace = false;
            for (int i = 0; i < itemsHeld.Length -1; i++)
            {
                if (itemsHeld[i] == "")
                {
                    itemsHeld[i] = itemsHeld[i + 1];
                    itemsHeld[i + 1] = "";

                    numberOfItems[i] = numberOfItems[i + 1];
                    numberOfItems[i + 1] = 0;

                    if (itemsHeld[i] != "")
                    {
                        itemAfterSpace = true;
                    }
                }
            }
        }
    }

    public void AddItem(string itemToAdd)
    {
        int newItemPosition = 0;
        bool foundSpace = false;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd)
            {
                newItemPosition = i;
                i = itemsHeld.Length;
                foundSpace = true;
            }
        }

        if (foundSpace)
        {
            bool itemExists = false;
            for (int i = 0; i < itemReference.Length; i++)
            {
                if (itemReference[i].itemName == itemToAdd)
                {
                    itemExists = true;

                    i = itemReference.Length;
                }
            }

            if (itemExists)
            {
                itemsHeld[newItemPosition] = itemToAdd;
                numberOfItems[newItemPosition]++;
            }
            else
            {
                Debug.LogError(itemToAdd + " Does Not Exist!!");
            }
        }

        GameMenu.instance.ShowItems();
    }

    public void RemoveItem(string itemToRemove)
    {
        bool foundItem = false;
        int itemPosition = 0;

        for (int i = 0; i < itemsHeld.Length; i++)
        {
            if(itemsHeld[i] == itemToRemove)
            {
                foundItem = true;
                itemPosition = i;

                i = itemsHeld.Length;
            }
        }

		if(foundItem)
		{
            numberOfItems[itemPosition]--;

            if(numberOfItems[itemPosition] <= 0)
            {
                itemsHeld[itemPosition] = "";
            }

            GameMenu.instance.ShowItems();
        } else
        {
            Debug.LogError("Could not find" + itemToRemove);
        }
    }

    public void SavingData()
    {
        //Save Player Position
        PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
        PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
        PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);

        //Save Character Info
        for(int i = 0; i < playerStats.Length; i++)
        {
            if (playerStats[i].gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_active", 1);
            } 
            else
            {
                PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_active", 0);
            }

            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_Level", playerStats[i].playerLevel);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_CurrentExp", playerStats[i].currentEXP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_CurrentHP", playerStats[i].currentHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_CurrentMP", playerStats[i].currentMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_MaxHP", playerStats[i].maxHP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_MaxMP", playerStats[i].maxMP);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_Strength", playerStats[i].strength);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_Defence", playerStats[i].defence);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_WeaponPower", playerStats[i].wpnPwr);
            PlayerPrefs.SetInt("Player_" + playerStats[i].characterName + "_ArmorPower", playerStats[i].armrPwr);
            PlayerPrefs.SetString("Player_" + playerStats[i].characterName + "_EquippedWeapon", playerStats[i].equippedWpn);
            PlayerPrefs.SetString("Player_" + playerStats[i].characterName + "_EqippedArmor", playerStats[i].equippedArmr);
        }

        //Save Inventory Data
        for (int i = 0; i < itemsHeld.Length; i++)
        {
            PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]);
            PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]);
        }
    }

    public void LoadingData()
    {
        PlayerController.instance.transform.position = new Vector3(PlayerPrefs.GetFloat("Player_Position_x"),
            PlayerPrefs.GetFloat("Player_Position_y"), PlayerPrefs.GetFloat("Player_Position_z"));

        for (int i = 0; i < playerStats.Length; i++)
        {
            if (PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_active") == 0)
            {
                playerStats[i].gameObject.SetActive(false);
            }
            else
            {
                playerStats[i].gameObject.SetActive(true);
            }

            //Load Player Stats
            playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Level");
            playerStats[i].currentEXP = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_CurrentExp");
            playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_CurrentHP");
            playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_CurrentMP");
            playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_MaxHP");
            playerStats[i].maxMP = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_MaxMP");
            playerStats[i].strength = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Strength");
            playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_Defence");
            playerStats[i].wpnPwr = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_WeaponPower");
            playerStats[i].armrPwr = PlayerPrefs.GetInt("Player_" + playerStats[i].characterName + "_ArmorPower");
            playerStats[i].equippedWpn = PlayerPrefs.GetString("Player_" + playerStats[i].characterName + "_EquippedWeapon");
            playerStats[i].equippedArmr = PlayerPrefs.GetString("Player_" + playerStats[i].characterName + "_EqippedArmor");
        }

        //LoadingData Inventory Data
        for (int i = 0; i < itemsHeld.Length; i++)
        {
            itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i);
            numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i);
        }
    }
}
