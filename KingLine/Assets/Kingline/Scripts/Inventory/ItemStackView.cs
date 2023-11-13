using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemStackView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,
    IPointerExitHandler, IDropHandler
{
    public static int From;
    public static int SelectedIndex;
    public static int To;

    public static ItemStackView _selectedItemView;
    public Transform Content;

    [SerializeField]
    private Image m_background;

    public int Id;

    public IType Filter;


    public Color SELECTED_BACKGROUND_COLOR = new(0.5f, 0.6f, 0.7f, 0.5f);
    public Color NOT_SELECTED_BACKGROUND_COLOR = new(0.4f, 0.4f, 0.4f, 1f);
    public Color POINTER_OVER_BACKGROUND_COLOR = new(0.5f, 0.5f, 0.5f, 1f);


    public UnityEvent<int, int> OnDropItem = new();

    private void Start()
    {
        m_background.color = NOT_SELECTED_BACKGROUND_COLOR;
        if (Filter != IType.NONE)
        {
            SELECTED_BACKGROUND_COLOR = Color.green;
            NOT_SELECTED_BACKGROUND_COLOR = Color.white;
            POINTER_OVER_BACKGROUND_COLOR = Color.white;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (Id == From)
            return;

        var dropped = eventData.pointerDrag;
        var view = dropped.GetComponent<ItemStackContentView>();
        if (view == null) return;
        if (Content.childCount != 0)
            return;

        var inv = InventoryNetworkController.LocalInventory;
        var item = inv.Items[From];
        var itemInfo = ItemRegistry.GetItem(item.Id);
        if (Filter != IType.NONE)
        {
            if (itemInfo.Type != Filter)
                return;
        }

        view.ParentAfterDrag = transform;
        To = Id;
        OnPointerDown(eventData);
        NetworkManager.Instance.Send(new ReqInventoryMove
        {
            FromIndex = (short)From,
            ToIndex = (short)To
        });

        OnDropItem?.Invoke(From, To);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SelectedIndex = this.Id;
        if (_selectedItemView != null)
            _selectedItemView.m_background.color = _selectedItemView.NOT_SELECTED_BACKGROUND_COLOR;

        var view = Content.GetComponentInChildren<ItemStackContentView>();
        if (view != null)
        {
            InventoryView.OnItemClick?.Invoke(view.ItemId);
        }
        else
        {
            InventoryView.OnItemClick?.Invoke(-1);
        }

   
        _selectedItemView = this;
        _selectedItemView.m_background.color = SELECTED_BACKGROUND_COLOR;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_selectedItemView != this) m_background.color = POINTER_OVER_BACKGROUND_COLOR;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_selectedItemView != this) m_background.color = NOT_SELECTED_BACKGROUND_COLOR;
    }
}