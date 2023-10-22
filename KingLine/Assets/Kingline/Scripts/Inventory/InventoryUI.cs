using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InventoryUI : MonoBehaviour
{
    public static UnityEvent<int> OnItemClick = new();

    [SerializeField]
    private ItemStackView m_itemViewTemplate;

    [SerializeField]
    private ItemStackView[] m_gearSets;

    [SerializeField]
    private ItemStackContentView m_itemViewContentTemplate;

    [SerializeField]
    private Transform m_itemViewContent;

  

    public TMP_Text TotalStrengthText;
    public TMP_Text TotalArmorText;
    public TMP_Text CoinText;

    [SerializeField]
    private ItemInfoView m_itemInfoView;

    private void Start()
    {
        var controller = NetworkManager.Instance.GetController<InventoryNetworkController>();
        controller.OnGearChange.AddListener(OnGearsetChanged);
        OnItemClick.AddListener(OnItemClicked);
    }

    private void OnEnable()
    {
        ShowInventory();
    }

    private void OnDisable()
    {
        ClearInventoryUI();
    }
    
    
    private void OnItemClicked(int index)
    {
        var item = InventoryNetworkController.LocalInventory.Items[index];
        if (item.Id == -1)
        {
            m_itemInfoView.gameObject.SetActive(false);
        }
        else
        {
            m_itemInfoView.gameObject.SetActive(true);
            m_itemInfoView.ShowItemInfo(ItemRegistry.GetItem(item.Id));
        }
    }

    private void OnGearsetChanged(int id)
    {
        if (id == NetworkManager.LocalPlayerPeerId)
            DisplayGear();
    }

 

    public void ShowInventory()
    {
        var items = InventoryNetworkController.LocalInventory.Items;
        for (var i = 0; i < items.Length; i++)
        {
            var m = items[i];
            if (i >= 25)
            {
                var gearView = m_gearSets[Mathf.Abs(25 - i)];
                gearView.Id = i;
                if (m.Id != -1)
                {
                    var gearItem = ItemRegistry.GetItem(m.Id);
                    var contentView = Instantiate(m_itemViewContentTemplate, gearView.Content);
                    contentView.SetContext(SpriteLoader.LoadSprite(gearItem.Name), m.Count, gearItem.Stackable);
                }

                continue;
            }

            var view = Instantiate(m_itemViewTemplate, m_itemViewContent);
            view.Id = i;
            if (m.Id != -1)
            {
                var item = ItemRegistry.GetItem(m.Id);
                var contentView = Instantiate(m_itemViewContentTemplate, view.Content);
                contentView.SetContext(SpriteLoader.LoadSprite(item.Name), m.Count, item.Stackable);
            }
        }
        DisplayGear();
    }

  
    private void ClearInventoryUI()
    {
        for (var i = 0; i < m_itemViewContent.transform.childCount; i++)
            Destroy(m_itemViewContent.transform.GetChild(i).gameObject);

        for (var i = 0; i < m_gearSets.Length; i++)
            if (m_gearSets[i].transform.childCount > 0)
                Destroy(m_gearSets[i].transform.GetChild(0).gameObject);
    }

    private async void DisplayGear()
    {
        var inventory = await InventoryNetworkController.GetInventoryAsync();
        var helmet = inventory.GetHelmet();
        var armor = inventory.GetArmor();
        var hand = inventory.GetHand();

        var m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();

        var baseStrength = m_progressionNetworkController.GetSkill("Strength");
        var baseDefence = m_progressionNetworkController.GetSkill("Defence");

        if (helmet.Id != -1)
        {
            var item = ItemRegistry.GetItem(helmet.Id);
            var armorMaterial = (ArmorItemMaterial)item;
            baseDefence += (byte)armorMaterial.Armor;
        }

        if (armor.Id != -1)
        {
            var item = ItemRegistry.GetItem(armor.Id);
            var armorMaterial = (ArmorItemMaterial)item;
            baseDefence += (byte)armorMaterial.Armor;
        }

        if (hand.Id != -1)
        {
            var item = ItemRegistry.GetItem(hand.Id);
            var armorMaterial = (WeaponItemMaterial)item;
            baseStrength += (byte)armorMaterial.Attack;
        }

        TotalArmorText.text = baseDefence + "";
        TotalStrengthText.text = baseStrength + "";
        CoinText.text = NetworkManager.Instance.GetController<PlayerNetworkController>().LocalPlayer.Currency + "";
    }
}