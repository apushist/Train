using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] GameObject _inventoryScreen;
    [SerializeField] GameObject _changedItemView;
    [SerializeField] GameObject _neededItemView;
    [SerializeField] public Sprite _itemSpriteEmpty;
    [SerializeField] InventoryItem _inventoryItemDefault;

    [Header("Item Needed Settings")]
    [SerializeField] Image _neededItemSpriteEmpty;
    [SerializeField] Image _neededItemSpriteFull;  
    [SerializeField] private TextMeshProUGUI _itemNeededCounter;

    [Header("Item Scale Settings")]
    [SerializeField] private RectTransform _parentSprite;
    [SerializeField] private float _itemOffset;
    [SerializeField] private float _itemEndOffset;
    [SerializeField] int _itemCount;

    [Header("Item Changed Settings")]
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private Image _itemBigImage;
    
    List<InventoryItem> items = new List<InventoryItem>();
    List<InventoryItem> itemsGrid = new List<InventoryItem>();
    bool isEnabled;
    bool nearInteractionObject;
    ItemsCollector currentInteraction;
    float _itemSize;
    public static Inventory instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        
        CloseInventory();
        _itemSize = ((_parentSprite.offsetMax.x - _parentSprite.offsetMin.x) - (_itemEndOffset * 2) - (_itemOffset * (_itemCount - 1))) / _itemCount;
        GenerateGrid();
        ChangeInventoryView(true);
    }
    public void AddItem(int ident)
    {
        InventoryItem item = ItemsData.instance.SearchItemById(ident);
        items.Add(item);
        for (int i = 0; i < _itemCount * _itemCount; i++)
        {
            if (itemsGrid[i].empty)
            {
                itemsGrid[i].SetNewItem(item, false);
                Debug.Log(i);
                return;
            }
        }
        Debug.Log("Full Inventory!");
    }

    public void ShowMoreInfoItem(InventoryItem item)
    {
        _itemDescriptionText.text = item._description;
        _itemBigImage.sprite = item._itemImage;
    }
    public void HideMoreInfoItem(InventoryItem item)
    {
        _itemDescriptionText.text = "";
        _itemBigImage.sprite = _itemSpriteEmpty;
    }

    public void GenerateGrid()
    {
        for (int y = 0; y < _itemCount; y++)
        {
            for (int x = 0; x < _itemCount; x++)
            {
                var spawnedItem = Instantiate(_inventoryItemDefault, _parentSprite);
                spawnedItem.name = $"Tile {x} {y}";
                var rect = spawnedItem.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(x * (_itemSize + _itemOffset) + _itemEndOffset, (_parentSprite.offsetMax.y * 2) - y * (_itemSize + _itemOffset) - _itemEndOffset);                
                rect.localScale = new Vector2(_itemSize, _itemSize) / (rect.offsetMax.x - rect.offsetMin.x);
                itemsGrid.Add(spawnedItem);
                spawnedItem.Init();
            }

        }
    }
    public bool PressKeyInventory()
    {
        if (isEnabled)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
        return isEnabled;
    }
    public void OpenInventory()
    {
        _inventoryScreen.SetActive(true);
        isEnabled = true;
        if (nearInteractionObject)
        {
            ChangeInventoryView(false);
            UpdateNeededItemSpriteView(false);
        }
        else
        {
            ChangeInventoryView(true);
        }
    }
    public void CloseInventory()
    {
        _inventoryScreen.SetActive(false);
        isEnabled = false;
    }

    void ChangeInventoryView(bool b)
    {
        _changedItemView.SetActive(b);
        _neededItemView.SetActive(!b);
    }
    public void InteractWithObject(ItemsCollector col = null)
    {
        if (col!=null)
        {
            currentInteraction = col;
            nearInteractionObject = true;
        }
        else
        {
            currentInteraction = null;
            nearInteractionObject = false;
        }
    }
    public void TrySetItemInteractable(InventoryItem item)
    {
        if(nearInteractionObject && currentInteraction != null)
        {
            bool successed = currentInteraction.TrySetItem(item);
            UpdateNeededItemSpriteView(successed);
        }
        else
        {
            Debug.Log("error item interact set");
        }
    }
    public void UpdateNeededItemSpriteView(bool succeed)
    {
        _neededItemSpriteEmpty.sprite = currentInteraction._neededItem._itemImage;
        _neededItemSpriteFull.sprite = currentInteraction._neededItem._itemImage;
        _neededItemSpriteEmpty.gameObject.SetActive(!succeed);
        _neededItemSpriteFull.gameObject.SetActive(succeed);
        if (succeed)
        {
            _itemNeededCounter.text = "1 / 1";
        }
        else
        {
            _itemNeededCounter.text = "0 / 1";
        }
        
    }
}
