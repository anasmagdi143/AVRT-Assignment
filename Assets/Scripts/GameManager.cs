using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Drag scene objects, not prefabs")]
    public MemorySocket[] sockets;
    public MemoryItem[] cubes;
    public Transform basketSpot;

    [Header("HUD")]
    public TMPro.TextMeshPro hudText;   

    [Header("Restart")]
    public InputActionReference restartAction;   

    [Header("Layout")]
    public float rowSpacing = 0.25f;

    [Header("Placing timer")]
    public float placingSeconds = 15f;

    [Header("Rounds")]
    public float startMemoriseSeconds = 10f;   
    public float memoriseDrop = 1.5f;          
    public float minMemoriseSeconds = 3f;      
    public int passThreshold = 6;              
    public float roundGap = 2.5f;              

    private bool scored = false;
    private bool canScore = false;
    private float timeLeft = 0f;

    private int roundNumber = 0;
    private int lastRoundCorrect = 0;
    private bool gameOver = false;

    void Start()
    {
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (gameOver == false)
        {
            roundNumber = roundNumber + 1;
            yield return StartCoroutine(RunRound());

            if (lastRoundCorrect < passThreshold)
            {
                gameOver = true;
                ShowGameOver();
            }
            else
            {
                yield return new WaitForSeconds(roundGap);
            }
        }
    }

    IEnumerator RunRound()
    {
        scored = false;
        canScore = false;

        SetUpRound();

        
        float memoriseTime = startMemoriseSeconds - (memoriseDrop * (roundNumber - 1));
        if (memoriseTime < minMemoriseSeconds)
        {
            memoriseTime = minMemoriseSeconds;
        }

        if (hudText != null)
        {
            hudText.text = "Round " + roundNumber + " - Memorise!";
        }

        yield return new WaitForSeconds(memoriseTime);

        MoveCubesToRow();

        
        while (scored == false)
        {
            yield return null;
        }

        
        yield return new WaitForSeconds(1.5f);
    }

    
    void SetUpRound()
    {
        
        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].SetGrabbing(false);
        }

        Shuffle(cubes);

        for (int i = 0; i < sockets.Length; i++)
        {
            sockets[i].correctItemID = cubes[i].itemID;

            MemoryItem cube = cubes[i];
            cube.GetComponent<Rigidbody>().isKinematic = true;
            cube.transform.position = sockets[i].transform.position;
            cube.transform.rotation = sockets[i].transform.rotation;
        }
    }

    
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

        canScore = true;
        timeLeft = placingSeconds;
    }

    void Update()
    {
        if (restartAction != null && restartAction.action.WasPressedThisFrame())
        {
            RestartGame();
        }
        if (canScore == false)
        {
            return;
        }

        if (scored == true)
        {
            return;
        }

        // count the placing timer down
        timeLeft = timeLeft - Time.deltaTime;

        if (hudText != null)
        {
            hudText.text = "Time: " + Mathf.Ceil(timeLeft).ToString();
        }

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            scored = true;
            Debug.Log("Time's up!");
            Score();
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

        lastRoundCorrect = correct;

        Debug.Log("Round " + roundNumber + " score: " + correct + " / " + sockets.Length);

        if (hudText != null)
        {
            hudText.text = "Round " + roundNumber + ": " + correct + " / " + sockets.Length;
        }
    }

    void ShowGameOver()
    {
        if (hudText != null)
        {
            hudText.text = "Game Over!\nYou reached round " + roundNumber + "\nPress X to restart";
        }
        Debug.Log("Game over at round " + roundNumber);
    }

    void RestartGame()
    {
        StopAllCoroutines();

        roundNumber = 0;
        lastRoundCorrect = 0;
        gameOver = false;
        scored = false;
        canScore = false;

        StartCoroutine(GameLoop());
    }

    // simple shuffle 
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