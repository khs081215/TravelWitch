using UnityEngine;
using UnityEngine.UI;
/**
 * 코인 전용 1칸 인벤토리를 생성합니다.
 */
public class Inventory : MonoBehaviour
{
    [HideInInspector] public Item[] items = new Item[0];
    
    private const int numSlots = 1;
    private Image[] itemImages = new Image[numSlots];
    private GameObject[] slots = new GameObject[numSlots];
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] Item coinitem;

    public void Start()
    {
        CreateSlots();
        items[0] = Instantiate(coinitem);
        items[0].quantity = 0;
        itemImages[0].sprite = coinitem.sprite;
        itemImages[0].enabled = true;
        Slot slotScript = slots[0].GetComponent<Slot>();
        Text quantityText = slotScript.qtyText;
        quantityText.enabled = true;
        quantityText.text = items[0].quantity.ToString();
    }

    public void CreateSlots()
    {
        if (slotPrefab != null)
        {
            GameObject newSlot = Instantiate(slotPrefab);
            newSlot.name = "ItemSlot_0";
            newSlot.transform.SetParent(gameObject.transform.GetChild(0).transform);
            slots[0] = newSlot;
            itemImages[0] = newSlot.transform.GetChild(1).GetComponent<Image>();
        }
    }

    public bool AddItem(Item itemToAdd)
    {
        if (items[0] != null && items[0].itemType == itemToAdd.itemType && itemToAdd.stackable == true)
        {
            items[0].quantity = items[0].quantity + 1;
            Slot slotScript = slots[0].GetComponent<Slot>();
            Text quantityText = slotScript.qtyText;
            quantityText.text = items[0].quantity.ToString();
            return true;
        }
        return false;
    }
}