using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Agent", menuName = "Agnet/AgentEntity")]
public class AgentSO : ScriptableObject
{
    public EntityOS entity;
    public double health, damage, speed, radius;
    [EnumToggleButtons]public AgentType type;
}

[Serializable]
public enum AgentType
{
    Building,
    Villager, 
    Army
}