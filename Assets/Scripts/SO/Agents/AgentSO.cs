using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Agent", menuName = "Agnet/AgentEntity")]
public class AgentSO : ScriptableObject
{
    public EntityOS entity;
    public double health, damage, speed, radius;
    public AgentType type;
}

public enum AgentType
{
    Villager, 
    Army
}