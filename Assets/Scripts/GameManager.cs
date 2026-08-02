using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Drag scene objects, not prefabs")]
    public MemorySocket[] sockets;
    public MemoryItem[] cubes;
    public Transform basketSpot;

    [Header("Settings")]
    public float memoriseSeconds = 10f;
    public float rowSpacing = 0.25f;
    public float placingSeconds = 15f;

    private bool scored = false;
    private bool canScore = false;
    private float timeLeft = 0f;

    void Start()
    {
        StartCoroutine(RunRound());
    }

    IEnumerator RunRound()
    {
        SetUpRound();
        yield return new WaitForSeconds(memoriseSeconds);
        MoveCubesToRow();
    }

    // shuffle cubes, assign answers, park each cube on its socket
    void SetUpRound()
    {
        Shuffle(cubes);

        // sockets off so they don't grab the cubes we're about to place
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].SetGrabbing(false);
        }

        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].correctItemID = cubes[i].itemID;

            MemoryItem cube = cubes[i];
            cube.GetComponent<Rigidbody>().isKinematic = true;
            cube.transform.position = sockets[i].transform.position;
            cube.transform.rotation = sockets[i].transform.rotation;
        }
    }

    // lay all cubes out in a row the player can reach
    void MoveCubesToRow()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].SetGrabbing(false);
        }

        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].GetComponent<Rigidbody>().isKinematic = true;
            Vector3 offset = new Vector3(i * rowSpacing, 0f, 0f);
            cubes[i].transform.position = basketSpot.position + offset;
        }

        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].SetGrabbing(true);
        }

        canScore = true;   // retrieval phase begins — scoring allowed now
    }

    void Update()
    {

        if (canScore == false)
        {
            return;
        }

        if (scored == true)
        {
            return;
        }

        if (AllSocketsFull() == true)
        {
            scored = true;
            Score();
        }

    }

    bool AllSocketsFull()
    {
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i].HeldItem() == null)
            {
                return false;
            }
        }
        return true;
    }

    void Score()
    {
        int correct = 0;

        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i].IsCorrect() == true)
            {
                correct++;
            }
        }

        Debug.Log("Score: " + correct + " / " + sockets.Length);
    }

    // simple shuffle — swap each item with a random one
    void Shuffle(MemoryItem[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            MemoryItem temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}