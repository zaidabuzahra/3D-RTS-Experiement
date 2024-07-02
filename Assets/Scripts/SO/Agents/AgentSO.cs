using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Agent", menuName = "Agnet/AgentEntity")]
public class AgentSO : ScriptableObject
{
    public string entityName;
    public sbyte entityID;
    public double health, damage, speed, radius;
    public GameObject entityPrefab;
    public Sprite entityImage;
    public AgentType type;
}

public enum AgentType
{
    Villager, 
    Army
}