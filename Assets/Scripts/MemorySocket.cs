using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class MemorySocket : MonoBehaviour
{
    public string labelID;   
    public string correctItemID;
    public AudioClip placeSound;
    private AudioSource audioSource;


    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        audioSource = GetComponent<AudioSource>();

        socket.selectEntered.AddListener(OnPlaced);
    }

    
    public MemoryItem HeldItem()
    {
        if (socket.hasSelection == false)
        {
            return null;
        }

        var held = socket.GetOldestInteractableSelected();
        var heldObject = held as MonoBehaviour;

        if (heldObject == null)
        {
            return null;
        }

        return heldObject.GetComponent<MemoryItem>();
    }

    public bool IsCorrect()
    {
        MemoryItem item = HeldItem();

        if (item == null)
        {
            return false;
        }

        return item.itemID == correctItemID;
    }

    
    public void SetGrabbing(bool on)
    {
        socket.enabled = on;
    }

    void OnPlaced(SelectEnterEventArgs args)
    {
        if (placeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(placeSound);
        }
    }
}