using System;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CollectionManager : Singleton<CollectionManager>
{
    public SpriteResolver a;
    private GameManager _gameManager;
    private UIManager _uiManager;
    private Ore _ore;

    public ItemDatabase itemDatabase;
    private ItemCollection _itemCollection;
    private ItemStats _itemStats;

    private float[] _cumulativeDropChances;

    public int itemCollectionIndex;
    public int itemStatsIndex;

    private bool once;

    private void Awake()
    {
        _uiManager = UIManager.Instance;
        _gameManager = GameManager.Instance;
        _ore = Ore.Instance;
    }

    private void Start()
    {
        UpdateItemSelection();
        CheckEveryCollection();
    }

    public void UpdateItemSelection()
    {
        try
        {
            _itemCollection = itemDatabase.collections[itemCollectionIndex];
            _itemStats = _itemCollection.items[itemStatsIndex];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogWarning("Item Collection for This Ore Does Not Exist");
        }
    }

    //Method to get the cumulative drop rate
    private void CalculateCumDropRate()
    {
        if (_itemCollection.items.Length != 0)
        {
            Array.Resize(ref _cumulativeDropChances, _itemCollection.items.Length);
            var i = 0;
            float dropChance = 0;
            foreach (var item in _itemCollection.items)
            {
                dropChance += item.dropChance;
                _cumulativeDropChances[i] = dropChance;
                Debug.Log(_cumulativeDropChances[i]);
                i++;
            }
        }
        else
        {
            Debug.LogWarning("No items in the item collection");
        }
    }

    //Method to drop an item
    public void DropItem()
    {
        if (_itemCollection.items.Length != 0)
        {
            var randomNumber = Random.Range(0f, 100f);
            var i = 0;
            foreach (var variable in _cumulativeDropChances)
            {
                if (randomNumber <= variable)
                {
                    itemStatsIndex = i;
                    UpdateItemSelection();
                    CheckCollection();
                    break;
                }

                i++;
            }
        }
        else
        {
            Debug.LogWarning("No items in the item collection");
        }
    }

    //Method to add Item to collection otherwise Give Money to player
    private void CheckCollection()
    {
        _itemStats.timesForged++;
        var itemButton = _uiManager.collectionList.transform.GetChild(itemCollectionIndex).GetChild(itemStatsIndex)
            .GetComponent<Button>();
        var displayDescription = $"Selling Price: {_itemStats.itemPrice}\n" +
                                 $"Rarity: {_itemStats.itemRarity}\n" +
                                 $"First Forged: {_itemStats.itemFirstForged}\n" +
                                 $"Times Forged: {_itemStats.timesForged}\n" +
                                 "\n" +
                                 $"{_itemStats.itemDescription}";
        var itemName = _itemStats.itemName;
        var itemSprite = _itemStats.itemSprite;
        
        if (!_itemStats.isUnlocked)
        {
            _itemStats.isUnlocked = true;
            _uiManager.collectionList.transform.GetChild(itemCollectionIndex).GetChild(itemStatsIndex).GetChild(0)
                .GetComponent<Image>().color = Color.white;
            _itemStats.itemFirstForged = DateTime.Today.ToString("d");
            Debug.Log($"{_itemStats.itemName} has been added to your collection");
        }

        itemButton.onClick.RemoveAllListeners();
        Debug.Log(_itemStats.itemName + "Assigning to button" + itemButton);

        itemButton.onClick.AddListener(
            delegate
            {
                _uiManager.AssignOverlayValue(itemName, displayDescription, itemSprite);
                Debug.Log(_itemStats.itemName);
                _uiManager.OpenOverlay();
            });
        _gameManager.ModifyMoney(_itemStats.itemPrice);
    }

    private void CheckEveryCollection()
    {
        var j = 0;
        foreach (var collection in itemDatabase.collections)
        {
            var i = 0;
            foreach (var items in collection.items)
            {
                var itemButton = _uiManager.collectionList.transform.GetChild(j).GetChild(i).GetComponent<Button>();
                var displayDescription = $"Selling Price: {items.itemPrice}\n" +
                                         $"Rarity: {items.itemRarity}\n" +
                                         $"First Forged: {items.itemFirstForged}\n" +
                                         $"Times Forged: {items.timesForged}\n" +
                                         "\n" +
                                         $"{items.itemDescription}";
                if (items.isUnlocked)
                {
                    _uiManager.collectionList.transform.GetChild(j).GetChild(i).GetChild(0).GetComponent<Image>()
                        .color = Color.white;
                    itemButton.onClick.AddListener(delegate
                    {
                        _uiManager.AssignOverlayValue(items.itemName, displayDescription, items.itemSprite);
                        Debug.Log(items.itemName);
                        _uiManager.OpenOverlay();
                    });
                }

                Debug.Log("Checked" + items.itemName + "from" + collection.collectionName);
                i++;
            }

            j++;
        }
    }

    public void UpdateRandomSystem()
    {
        itemCollectionIndex = _ore.selectedOreIndex;
        Debug.Log(_itemCollection.items.Length);
        if (_itemCollection.items.Length != 0)
        {
            CalculateCumDropRate();
            UpdateItemSelection();
        }
        else
        {
            Debug.LogWarning("No items in the item collection");
        }
    }
}