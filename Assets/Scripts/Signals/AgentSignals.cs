using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;

public class AgentSignals : MonoSingleton<AgentSignals> 
{
    public UnityAction<GameObject> onSelectAgent = delegate { };
    public UnityAction onDeselectAgents = delegate { };
    public UnityAction<List<Agent>> onSelectBoxAgents = delegate { };
    public UnityAction<GameObject, bool> onHighlightAgent = delegate { };
}