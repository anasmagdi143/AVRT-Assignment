using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;   // add near the other usings

public class GameManager : MonoBehaviour
{
    public MemorySocket[] sockets;    // drag all 6 in
    public MemoryItem[] items;        // drag all 6 in
    public Transform basketPoint;     // empty where cubes drop after memorise
    public float memorizeSeconds = 10f;

    void Start() => StartCoroutine(RunTrial());

    IEnumerator RunTrial()
    {
        Debug.Log("1. Trial started");
        var shuffled = items.OrderBy(_ => Random.value).ToList();
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].correctItemID = shuffled[i].itemID;
            PlaceInSocket(shuffled[i], sockets[i]);
        }
        Debug.Log("2. Setup done, waiting 10s");

        yield return new WaitForSeconds(memorizeSeconds);

        Debug.Log("3. Ejecting now");
        foreach (var s in sockets) EjectFrom(s);
        Debug.Log("4. Eject done");
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            Score();
    }

    void Score()
    {
        int correct = sockets.Count(s => s.IsCorrect());
        Debug.Log($"Score: {correct} / {sockets.Length}");
    }

    void PlaceInSocket(MemoryItem item, MemorySocket socket)
    {
        var rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;                        // freeze during memorise
        item.transform.position = socket.transform.position;
        item.transform.rotation = socket.transform.rotation;
    }

    void EjectFrom(MemorySocket socket)
    {
        var item = items.First(i => i.itemID == socket.correctItemID);
        item.GetComponent<Rigidbody>().isKinematic = false;   // unfreeze
        item.transform.position = basketPoint.position + Random.insideUnitSphere * 0.1f;
    }
}