using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsManager : MonoBehaviour
{
    private readonly List<GameObject> selectedAgents = new List<GameObject>();

    private void OnEnable()
    {
        AgentSignals.Instance.onSelectAgent += ToggleSelectingAgent;
        AgentSignals.Instance.onDeselectAgents += DeselectAllAgents;
        AgentSignals.Instance.onHighlightAgent += ToggleHighlightingAgent;
        AgentSignals.Instance.onSelectBoxAgents += ApplyBoxSelection;
    }

    private void ToggleSelectingAgent(GameObject selected)
    {
        if (selectedAgents.Contains(selected)) 
        {
            ToggleHighlightingAgent(selected, false);
            return;
        }

        selectedAgents.Add(selected);
        ToggleHighlightingAgent(selected, true);
    }

    private void ToggleHighlightingAgent(GameObject selected, bool visible)
    {
        GameObject highlight = selected.GetComponent<Agent>().selectHighlight;
        if (highlight != null)
        {
            Debug.Log($"Name: {this.name} Clicked On: {selected.name} \n Active: {highlight.activeSelf}");
            highlight.SetActive(visible);
        }
    }

    private void DeselectAllAgents()
    {
        for (int i = 0; i < selectedAgents.Count; i++)
        {
            ToggleSelectingAgent(selectedAgents[i]);
        }
        selectedAgents.Clear();
    }

    private void ApplyBoxSelection(List<Agent> highlightedAgents)
    {
        foreach (Agent agent in highlightedAgents)
        {
            selectedAgents.Add(agent.gameObject);
        }
    }

    private void OnDisable()
    {
        
    }
}