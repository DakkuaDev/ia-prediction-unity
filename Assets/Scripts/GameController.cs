using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    #region variables
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

    [Header("GAME PROPIERTIES")]
    // Timer
    [Range(10, 30)]
    public float roundTime = 15;

    // Scores
    [Range(500,3000)]    
    public int scoreIA, scorePlayer;

    [Range(1, 37)]
    public int rouletteDistribution;

    [Header("GAME ELEMETS")]
    public Text textScoreIA;
    public Text textScorePlayer;

    // Slider
    public Slider sldMultiplicator;
    public Text multiplicator;

    // Buttons
    public Button btnBlack;
    public Button btnRed;

    // Info 
    public Text infoPlayer, infoIA;
    public Text benefPlayer, lossesPlayer;
    public Text benefIA, lossesIA;

    // Roulette
    public Text rouletteNumber;
    private float initialRoundTime;
    public Text textRoundTime;
    #endregion

    private void Start()
    {
        game = true;
        initialPlayer = player.transform.position;
        initialIA = ia.transform.position;
        initialRoundTime = roundTime;

        rouletteNumber.enabled = false;
        infoPlayer.enabled = false;
        infoIA.enabled = false;

        // Pongo en escucha los botones del UI
        btnBlack.onClick.AddListener(delegate { PlayerPlay(RPSAction.Black); });
        btnRed.onClick.AddListener(delegate { PlayerPlay(RPSAction.Red); });

        // Pongo el juego en marcha
        ResetRound();
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

        multiplicator.text = "x" + sldMultiplicator.value.ToString();
    }

    public IEnumerator IAPlay()
    {
        yield return new WaitForSeconds(Random.Range(1, roundTime - 1));
        iaAction = ia.Jugar();

        switch(iaAction)
        {
            case RPSAction.Black:
                ia.transform.position = blackIA;
                break;
            case RPSAction.Red:
                ia.transform.position = redIA;
                break;
            case RPSAction.None: break;
        }
    }

    void PlayerPlay(RPSAction _playerAction)
    {
        switch(_playerAction)
        {
            case RPSAction.Black: 
                playerAction = RPSAction.Black;
                player.transform.position = blackPlayer;
                break;
            case RPSAction.Red: 
                playerAction = RPSAction.Red;
                player.transform.position = redPlayer;
                break;
            case RPSAction.None: break;
        }
    }

    void RoundResults()
    {
        // Se retira el dinero apostado comprobando que se puede apostar en la ronda actual

        bool playingPlayer = false;
        bool playingIA = false;

        if (100 * (int)sldMultiplicator.value <= scorePlayer && playerAction != RPSAction.None)
        {
            scorePlayer -= 100 * (int)sldMultiplicator.value;
            lossesPlayer.text = "-" + (100 * (int)sldMultiplicator.value).ToString();
            playingPlayer = true;
            infoPlayer.enabled = false;
        }
        else
        {
            infoPlayer.enabled = true;
            lossesPlayer.text = "- 000";
        }
       
        var IAMultiplicator = Random.Range(1, 5);

        if (100 * IAMultiplicator <= scoreIA && iaAction != RPSAction.None)
        {
            scoreIA -= 100 * IAMultiplicator;
            lossesIA.text = "-" + (100 * IAMultiplicator).ToString();
            playingIA = true;
            infoIA.enabled = false;
        }
        else
        {
            infoIA.enabled = true;
            lossesIA.text = "- 000";
        }
     
        // Decisión de la ruleta (trucada)

        int _rouleteNumber = Random.Range(1, 37);
        rouletteNumber.enabled = true;
        rouletteNumber.text = _rouleteNumber.ToString();

        //Debug.LogWarning("Número Ruleta: " + rouleteNumber);

        if (_rouleteNumber > 0 && _rouleteNumber <= rouletteDistribution) 
        {
            rouleteAction = RPSAction.Black;
            rouletteNumber.color = new Color(0, 0, 0);
        }
        else if( _rouleteNumber > rouletteDistribution && _rouleteNumber <= 37) 
        {
            rouleteAction = RPSAction.Red;
            rouletteNumber.color = new Color(254, 0, 0);
        }

        //Debug.LogWarning(rouleteAction + " Ruleta");

        // Comprobación de Tokens
        if(playerAction == rouleteAction)
        {
            if (playingPlayer)
            {
                scorePlayer += 200 * (int)sldMultiplicator.value;
                benefPlayer.text = "+ " + (200 * (int)sldMultiplicator.value).ToString();
            }
            else benefPlayer.text = "+ 000";
        }
        else benefPlayer.text = "+ 000";

        if (iaAction == rouleteAction)
        {
            if (playingIA)
            {
                scoreIA += 200 * IAMultiplicator;
                benefIA.text = "+" + (200 * IAMultiplicator).ToString();
            }
            else benefIA.text = "+ 000";
               
        }
        else benefIA.text = "+ 000";

        ia.TellOpponentActionRM((RPSAction)rouleteAction);
        ResetRound();
    }

    void ResetRound()
    {
        // Seteo todo
        playerAction = RPSAction.None;
        player.transform.position = initialPlayer;
        ia.transform.position = initialIA;

        roundTime = initialRoundTime;

        game = true;

        // Juega la IA
        StartCoroutine(IAPlay());
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
