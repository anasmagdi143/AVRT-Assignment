using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MemorySocket : MonoBehaviour
{
    public string labelID;          // "A3" — fixed, this slot's name
    public string correctItemID;    // set at runtime by the manager

    private XRSocketInteractor socket;

    void Awake() => socket = GetComponent<XRSocketInteractor>();

    public MemoryItem CurrentItem()
    {
        var held = socket.hasSelection ? socket.GetOldestInteractableSelected() : null;
        return (held as MonoBehaviour)?.GetComponent<MemoryItem>();
    }

    public bool IsCorrect()
    {
        var item = CurrentItem();
        return item != null && item.itemID == correctItemID;
    }
}