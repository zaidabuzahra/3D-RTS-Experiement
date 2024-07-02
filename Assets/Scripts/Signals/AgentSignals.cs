using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Events;

public class AgentSignals : MonoSingleton<AgentSignals> 
{
    public UnityAction<GameObject> onSelectAgent = delegate { };
}