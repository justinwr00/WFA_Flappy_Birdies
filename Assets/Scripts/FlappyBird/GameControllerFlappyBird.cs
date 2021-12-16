using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerFlappyBird : MonoBehaviour {

    /// <summary>
    /// Game controller for Flappy Birdies Scene
    /// </summary>

    public AudioClip RupSound;

    // for oscillating buttons
    public float OscillationSpeed;
    public float OscillationHeight;
    private float phi;
    public GameObject RedReadyButton, PinkReadyButton, BlueReadyButton, GreenReadyButton;
    public GameObject RedJoinButton, PinkJoinButton, BlueJoinButton, GreenJoinButton;
    public GameObject ScoreTexts;
    

    public Transform startLine;              // position where pipes are put in play
    GameObject[] pipes = new GameObject[7];  // array of all top pipes
    public float MaxTopPipeOffset, MinTopPipeOffset;              // how far to push up top pipe from starting line transform pos
    public float delayPipeSpawn;             // number of seconds between pipe spawns
    //private int counter = 0; // for testing
    private int pipeCounter = 0;
    private PipeController pipeTemp;
    private bool pipeReady = true;           // is pipe ready to be put in play
    private bool countDownReady = false;     // is ready to countdown to start playing? set to true when any player readys up
    private bool countingDown = false;       // are we currently counting down to start the game?
    private int CountDown = 5;
    private bool player1Ready = false;
    private bool player2Ready = false;
    private bool player3Ready = false;
    private bool player4Ready = false;
    private int playersReady = 0;
    private bool acceptingInput = false;

    public GameObject player1, player2, player3, player4;
    private PlayerControllerFlappyBird p1, p2, p3, p4;

    // for the UI
    // public Text TitleMenu;
    // public GameObject TitleUI, 
    public GameObject ScoreCardUI;
    public float minPipeGap, maxPipeGap;
    public GameObject  Countdown5, Countdown4, Countdown3, Countdown2, Countdown1;
    public Transform CountdownSpawn;

    private GameObject TitleGraphic;    // Flappy Birdies title sign


    // singleton pattern for game controller
    private static GameControllerFlappyBird _instance;
    public static GameControllerFlappyBird Instance
    {
        get
        {
            if (_instance == null)
            {
                //throw exception
                Debug.LogError("Game manager instance error");
            }
            return _instance;
        }
    }

    // oscillating buttons property
    // gives adjustment to button's height
    private static float oscillation;
    public static float Oscillation
    {
        get
        {
            return oscillation;
        }

        set
        {
            oscillation = value;
        }
    }

    // game state property
    private static string gameState;
    public static string GameState
    {
        get
        {
            return gameState;
        }

        set
        {
            gameState = value;
        }
    }

    // pipe gap property - gap between top and bottom pipes
    private static float pipeGap;
    public static float PipeGap
    {
        get
        {
             return pipeGap;
            
        }

        set
        {
            pipeGap = value;
        }
    }


    private void Awake()
    {
        //instantiate singleton
        _instance = this;

        TitleGraphic = GameObject.Find("MainTitle");

        phi = 0f;

       // InitiateButtons();

        // initialize player references
        p1 = player1.GetComponent<PlayerControllerFlappyBird>();
        p2 = player2.GetComponent<PlayerControllerFlappyBird>();
        p3 = player3.GetComponent<PlayerControllerFlappyBird>();
        p4 = player4.GetComponent<PlayerControllerFlappyBird>();

        // initialize list of pipe tops
        for (int i = 0; i < 7; i++)
        {
            pipes[i] = GameObject.Find("p" + i.ToString());
        }

        // initialize game state
        GameState = "WaitingForPlayers";
        PipeGap = maxPipeGap;

    }

    // frame updates //
    /// <summary>
    /// //////////////////////////////////////////////////////
    /// </summary>
    private void Update()
    {
        switch (GameState)
        {
            case "WaitingForPlayers":
                // for oscillating buttons:\
                phi += OscillationSpeed * Time.deltaTime;
                if (phi > 360f)
                    phi = 0f;
                Oscillation = Mathf.Sin(phi) * OscillationHeight;

                // state of waiting for players to join
                // starts counting down from 5 as soon as first player Rups, then play begins

                ListenForInput();
                // if player readys up for the first time, start the countdown
                // when countdown reaches 0, put ready players in play and state machine to "PlayingGame"
                // if any players ready up, start the countdown
                if (playersReady > 0 && countingDown == false)
                {
                    // start counting down to start the game
                    TitleGraphic.SetActive(false);
                    Instantiate(Countdown5, CountdownSpawn.transform.position, Quaternion.identity);
                    countingDown = true;
                    countDownReady = true;
                }

                if (countDownReady && countingDown)
                {
                    // run coroutine to decrement the countdown
                    StartCoroutine(CountDownCoroutine());
                    countDownReady = false;
                }

                if (CountDown < 1 && countingDown)
                {
                    // start the game
                    StartPlayingGame();
                }

                if (countingDown)
                {
                    // display countdown
                    //TitleMenu.text = CountDown.ToString();
                }

                break;

            case "PlayingGame":

                SpawnPipe();

                // if all players are idle, i.e. they all died,
                
                if (IsGameOver())
                {
                    ScoreCardUI.SetActive(true);
                    ScoreTexts.SetActive(true);
                    // TODO: populate scorecard with player scores here

                    GameState = "DisplayingScore";
                    acceptingInput = false;
                    StartCoroutine(DelayInput());
                }

                break;

            case "DisplayingScore":

                // display scorecard and wait for any key to restart game
                if (Input.anyKeyDown && acceptingInput)
                {
                    RestartGame();
                }
                break;

            default:
                break;
        }

    }
    /// <summary>
    /// //////////////////////////////////////////////////////
    /// </summary>
    /// <returns></returns>

    IEnumerator DelayInput()
    {
        // ignore input for 4 seconds
        yield return new WaitForSeconds(3f);
        acceptingInput = true;
    }

    IEnumerator MakePipeReady()
    {
        //yield return null;
        yield return new WaitForSeconds(delayPipeSpawn);
        pipeReady = true;
    }

    IEnumerator CountDownCoroutine()
    {
        yield return new WaitForSeconds(1f);
        CountDown--;
        // spawn countdown number graphic here
        switch (CountDown)
        {
            case 4:
                Instantiate(Countdown4, CountdownSpawn.transform.position, Quaternion.identity);
                break;

            case 3:
                Instantiate(Countdown3, CountdownSpawn.transform.position, Quaternion.identity);
                break;

            case 2:
                Instantiate(Countdown2, CountdownSpawn.transform.position, Quaternion.identity);
                break;

            case 1:
                Instantiate(Countdown1, CountdownSpawn.transform.position, Quaternion.identity);
                break;

            default:
                break;
        }
        countDownReady = true;
    }

    private void SpawnPipe()
    {
        if (pipeReady)
        {
            // put a pipe in play
            pipeTemp = pipes[pipeCounter].GetComponent<PipeController>();
            pipes[pipeCounter].transform.position = startLine.position;
            // apply offset to top pipe position
            pipes[pipeCounter].transform.position = new Vector3(pipes[pipeCounter].transform.position.x, pipes[pipeCounter].transform.position.y + Random.Range(MinTopPipeOffset,MaxTopPipeOffset), pipes[pipeCounter].transform.position.z);
            pipeTemp.IsInPlay = true;
            pipeTemp.PipeGap = Random.Range(minPipeGap, maxPipeGap);
            pipeCounter++;
            if (pipeCounter > 6)
            {
                pipeCounter = 0;
            }
            pipeReady = false;
            StartCoroutine(MakePipeReady());
        }
    }


    private void ListenForInput()
    {
        // listen for players to ready up during the WaitingForPlayers game state

        //listen for player 1
        if (Input.GetButtonDown("Fire1"))
        {
            if (!player1Ready)
            {
                player1Ready = true;
                playersReady++;
                RedReadyButton.SetActive(true);
                RedJoinButton.SetActive(false);
              //  Debug.Log("Player 1 Rup");
              //   Debug.Log(playersReady.ToString() + " players ready");
            }
            
        }

        //listen for player 2
        if (Input.GetButtonDown("Fire2"))
        {
            if (!player2Ready)
            {
                player2Ready = true;
                playersReady++;
                GreenReadyButton.SetActive(true);
                GreenJoinButton.SetActive(false);
                Debug.Log("Player 2 Rup");
                Debug.Log(playersReady.ToString() + " players ready");
            }
        }

        //listen for player 3
        if (Input.GetButtonDown("Fire3"))
        {
            if (!player3Ready)
            {
                player3Ready = true;
                playersReady++;
                BlueReadyButton.SetActive(true);
                BlueJoinButton.SetActive(false);
                Debug.Log("Player 3 Rup");
                Debug.Log(playersReady.ToString() + " players ready");
            }
        }

        //listen for player 4
        if (Input.GetButtonDown("Jump"))
        {
            if (!player4Ready)
            {
                player4Ready = true;
                playersReady++;
                PinkReadyButton.SetActive(true);
                PinkJoinButton.SetActive(false);
                Debug.Log("Player 4 Rup");
                Debug.Log(playersReady.ToString() + " players ready");
            }
        }
    }

    private void StartPlayingGame()
    {
        // takes the game from waiting state to playing the game
        p1.Score = 0;
        p2.Score = 0;
        p3.Score = 0;
        p4.Score = 0;

        // disable the title menu
        TitleGraphic.SetActive(false);
        //TitleUI.SetActive(false); //old text Title
        RedReadyButton.SetActive(false);
        BlueReadyButton.SetActive(false);
        PinkReadyButton.SetActive(false);
        GreenReadyButton.SetActive(false);
        RedJoinButton.SetActive(false);
        BlueJoinButton.SetActive(false);
        PinkJoinButton.SetActive(false);
        GreenJoinButton.SetActive(false);

        GameState = "PlayingGame";
        // change players state if they readied up
        if (player1Ready)
            p1.PlayerState = "InPlay";

        if (player2Ready)
            p2.PlayerState = "InPlay";

        if (player3Ready)
            p3.PlayerState = "InPlay";

        if (player4Ready)
            p4.PlayerState = "InPlay";
    }

    private void RestartGame()
    {
        // resets game to waiting state
        ScoreCardUI.SetActive(false);
        ScoreTexts.SetActive(false);
        // disable the title menu
        //  TitleUI.SetActive(true); //old title sign
        p1.Score = 0;
        p2.Score = 0;
        p3.Score = 0;
        p4.Score = 0;
        TitleGraphic.SetActive(true);
        GameState = "WaitingForPlayers";
        CountDown = 5;
        countingDown = false;
        playersReady = 0;
        player1Ready = false;
        player2Ready = false;
        player3Ready = false;
        player4Ready = false;
       // TitleMenu.text = "Flappy Birdies";

        RedJoinButton.SetActive(true);
        BlueJoinButton.SetActive(true);
        PinkJoinButton.SetActive(true);
        GreenJoinButton.SetActive(true);
    }

    private bool IsGameOver()
    {
        // check if all players are dead
        bool gameover = false;
        if (p1.PlayerState == "Idle" && p2.PlayerState == "Idle" && p3.PlayerState == "Idle" && p4.PlayerState == "Idle")
        {
            gameover = true;
        }
        return gameover;
    }

    
}
