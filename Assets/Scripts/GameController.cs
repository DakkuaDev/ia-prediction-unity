using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Game Clock
    private bool game;

    //Tokens Pos
    Vector3 initialPlayer;
    Vector3 initialIA;

    Vector3 blackPlayer = new Vector3(97, 134, 233);
    Vector3 redPlayer = new Vector3(338, 134, 233);

    Vector3 blackIA = new Vector3(191, 133, 233);
    Vector3 redIA = new Vector3(449, 133, 233);

    // Actions
    RPSAction playerAction;
    RPSAction iaAction;
    RPSAction rouleteAction;

    // Scripts
    public Bandit ia;
    public SpriteRenderer player;
    public Sprite sprites;

    // Scores
    [Range(500,3000)]    
    public int scoreIA, scorePlayer;
    public Text textScoreIA, textScorePlayer;

    // Buttons
    public Button btnBlack;
    public Button btnRed;

    // Timer
    [Range(10,30)]
    public float roundTime = 15;
    private float initialRoundTime;
    public Text textRoundTime;


    private void Start()
    {
        game = true;
        initialPlayer = player.transform.position;
        initialIA = ia.transform.position;
        initialRoundTime = roundTime;

        // Pongo en escucha los botones del UI
        btnBlack.onClick.AddListener(delegate { PlayerPlay(RPSAction.Black); });
        btnRed.onClick.AddListener(delegate { PlayerPlay(RPSAction.Red); });

        // Juega la IA
        // TODO: Que juege en un momento random del tiempo de ronda
        IAPlay();
    }

    // Update is called once per frame
    void Update()
    {
        if(game)
        {
            RoundTimer();
        }

        textScoreIA.text = scoreIA.ToString();
        textScorePlayer.text = scorePlayer.ToString();

    }

    public void IAPlay()
    {

        iaAction = ia.Jugar();

        switch(iaAction)
        {
            case RPSAction.Black:
                ia.transform.position = blackIA;
                break;
            case RPSAction.Red:
                ia.transform.position = redIA;
                break;
        }

        scoreIA -= 100;
        Debug.Log(iaAction + "IA");
    }

    void PlayerPlay(RPSAction _playerAction)
    {
        playerAction = _playerAction;

        // TODO: El jugador deberia apostar solo una vez no cada vez que la cambia de posición
        switch(playerAction)
        {
            case RPSAction.Black: 
                playerAction = RPSAction.Black;
                player.transform.position = blackPlayer;
                break;
            case RPSAction.Red: 
                playerAction = RPSAction.Red;
                player.transform.position = redPlayer;
                break;
        }

        scorePlayer -= 100;
        Debug.Log(playerAction + "Player");
    }

    void RoundResults()
    {

        // Decisión de la ruleta (trucada)
        int rouleteNumber = Random.Range(1, 37);
        Debug.LogWarning("Número Ruleta: " + rouleteNumber);
        if (rouleteNumber > 0 && rouleteNumber <= 20) // 65% de posibilidad
        {
            rouleteAction = RPSAction.Black;
        }
        else if( rouleteNumber > 20 && rouleteNumber <= 37) // 35% de posibilidad
        {
            rouleteAction = RPSAction.Red;
        }
        Debug.LogWarning(rouleteAction + " Ruleta");

        // Comprobación de Tokens
        if(playerAction == rouleteAction)
        {
            scorePlayer += 200;
        }
        if(iaAction == rouleteAction)
        {
            scoreIA += 200;
        }

        ia.TellOpponentActionRM((RPSAction)rouleteAction);
        ResetRound();
    }

    void ResetRound()
    {
        player.transform.position = initialPlayer;
        ia.transform.position = initialIA;

        roundTime = initialRoundTime;

        game = true;
        IAPlay();
    }

    private void RoundTimer()
    {

        if (roundTime > 0)
        {
            roundTime -= Time.deltaTime;
            textRoundTime.text = roundTime.ToString("00");
        }
        else
        {
            game = false;
            RoundResults();
        };
    }
}
