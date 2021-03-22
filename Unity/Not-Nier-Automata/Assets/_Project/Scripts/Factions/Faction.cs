using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FactionReputation
{
    public Faction faction;
    public float reputation;
}

public enum Reputation
{
    Neutral,
    Like,
    Dislike
}

[CreateAssetMenu(fileName = "Faction", menuName = "Game/Create Faction", order = 1)]
public class Faction : ScriptableObject
{
    public string factionName = "Faction";

    public FactionReputation[] factionReputations;

    public Reputation GetGeneralReputationWith(Faction faction)
    {
        for (int i = 0; i < factionReputations.Length; i++)
        {
            FactionReputation factionReputation = factionReputations[i];
            if (factionReputation.faction == faction)
            {
                if (factionReputation.reputation == 0)
                {
                    return Reputation.Neutral;
                }
                if (factionReputation.reputation > 0)
                {
                    return Reputation.Like;
                }
                if (factionReputation.reputation < 0)
                {
                    return Reputation.Dislike;
                }
            }
        }
        return Reputation.Neutral;
    }

    public bool AreEnemies(Faction faction)
    {
        return GetGeneralReputationWith(faction) == Reputation.Dislike;
    }
}
