using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bandit : MonoBehaviour
{
    private bool init;
    public int totalActions;
    private int[] count;
    private float[] score;

    public int numActions;
    public RPSAction lastAction;
    public int lastStrategy;

    public float initialRegret = 15f;
    private float[] regret;
    private float[] chance;
    public RPSAction lastOpponentAction;
    private RPSAction[] lastActionRM;

    public Sprite sprites;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        InitRegretMatching();
    }

    // Se va a ejecutar el algoritmo (por parte de la IA) para jugar, se cambia además el sprite por la acción escogida.
    public RPSAction Jugar()
    {
        RPSAction resultado;
        resultado = GetNextActionRM();

        return resultado;
    }

    // Se inicializa el algoritmo, comprobando si ya se habia llamado previamente
    private void InitRegretMatching()
    {
        if (init)
            return;
        numActions = System.Enum.GetNames(typeof(RPSAction)).Length;
        regret = new float[numActions];
        chance = new float[numActions];
        lastActionRM = new RPSAction[numActions];
        int i;
        for (i = 0; i < numActions; i++)
        {
            regret[i] = initialRegret;
            chance[i] = 0f;
        }
        init = true;
    }

    // Se le dice al oponente la acción escogida
    public void TellOpponentActionRM(RPSAction action)
    {
        int i;
        for (i = 0; i < numActions; i++)
        {
            regret[i] += GetUtility((RPSAction)i, action);
            regret[i] -= GetUtility(lastAction, action);
        }
    }

    // Se usa el algoritmo del Regret Matching para decidir la siguiente acción a escoger
    private RPSAction GetNextActionRM()
    {
        float sum = 0f;
        float prob = 0f;
        int i;

        InitRegretMatching();

        for (i = 0; i < numActions; i++)
        {
            if (regret[i] > 0f)
                sum += regret[i];
        }
        if (sum <= 0f)
        {
            lastAction = (RPSAction)Random.Range(0, numActions);
            return lastAction;
        }
        for (i = 0; i < numActions; i++)
        {
            chance[i] = 0f;
            if (regret[i] > 0f)
                chance[i] = regret[i];
            if (i > 0)
                chance[i] += chance[i - 1];
        }
        if (chance[numActions - 1] > 0)
        {
            prob = Random.Range(0, chance[numActions - 1]);
            for (i = 0; i < numActions; i++)
            {
                if (prob < chance[i])
                {
                    lastAction = (RPSAction)i;
                    return lastAction;
                }
            }
        }
        return (RPSAction)(numActions - 1);
    }

    // Plan de estrategia de la IA a futuro 
    private RPSAction GetActionForStrategy(RPSAction strategy)
    {
        RPSAction action;
        switch (strategy)
        {
            default:
            case RPSAction.Red:
                action = RPSAction.Red;
                break;
            case RPSAction.Black:
                action = RPSAction.Black;
                break;               
        }
        return action;
    }

    // Actualizo la utilidad (el remordimiento) al final de la ronda en función de los resultados
    private float GetUtility(RPSAction myAction, RPSAction opponents)
    {
        float utility = 0f;
        if (opponents == RPSAction.Red)
        {
            if (myAction == RPSAction.Black)
                utility = -2f;
            else if (myAction == RPSAction.Red)
                utility = 2f;
            else if (myAction == RPSAction.None)
                utility = -1f;
        }
        else if (opponents == RPSAction.Black)
        {
            if (myAction == RPSAction.Black)
                utility = 2f;
            else if (myAction == RPSAction.Red)
                utility = -2f;
            else if (myAction == RPSAction.None)
                utility = -1f;
        }
        return utility;
    }
}
