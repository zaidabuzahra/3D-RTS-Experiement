using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.LogWarning(selectedAgents.Count);
            for (int i = 0; i < selectedAgents.Count; i++)
            {
                if (selectedAgents[i].GetComponent<Agent>().agentData.type != AgentType.Building)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.LogWarning(selectedAgents[i].name);
                        //selectedAgents[i].GetComponent<Agent>().agent.destination = hit.point;
                        //selectedAgents[i].GetComponent<Agent>().agent.isStopped = false;
                        selectedAgents[i].GetComponent<Agent>().agent.SetDestination(hit.point);
                    }
                }
            }
        }
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