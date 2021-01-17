using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Bandit ia;
    public SpriteRenderer player;
    public Sprite sprites;

    public int scoreIA, scorePlayer;
    public Text textScoreIA, textScorePlayer;

    private void Start()
    {
        for(int i = 0; i < 10; ++i)
        {
            IAPlay();
        }
    }

    // Update is called once per frame
    void Update()
    {
        textScoreIA.text = scoreIA.ToString();
        textScorePlayer.text = scorePlayer.ToString();
    }

    public void IAPlay()
    {

        RPSAction iaAction = ia.Jugar();
        ia.TellOpponentAction((RPSAction)iaAction);

        Debug.LogWarning(iaAction);
    }

    public void PlayerPlay(int action)
    {

    }
}
