using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBoxController : MonoBehaviour
{
    [SerializeField]
    private RectTransform selectionBox;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private bool isDragging;
    private float dragTimer;

    private readonly List<Agent> highlightedAgents = new();
    private List<Agent> allAgents = new();

    private void Update()
    {
        dragTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            dragTimer = 0.5f;
            allAgents = new List<Agent>(FindObjectsOfType<Agent>());
            Debug.Log(allAgents.Count);
        }

        if (dragTimer > 0.2f && dragTimer < 0.35f)
        {
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            dragTimer = 0;
            selectionBox.gameObject.SetActive(false);
            AgentSignals.Instance.onSelectBoxAgents?.Invoke(highlightedAgents);
            highlightedAgents.Clear();
        }

        if (isDragging)
        {
            endPosition = Input.mousePosition;
            UpdateSelectionBox();
            HighlightAgentsInBox();
        }
    }

    private void UpdateSelectionBox()
    {
        if (!selectionBox.gameObject.activeInHierarchy)
        {
            selectionBox.gameObject.SetActive(true);
        }
        float x = Mathf.Min(startPosition.x, endPosition.x);
        float y = Mathf.Min(startPosition.y, endPosition.y);
        float width = Mathf.Abs(startPosition.x - endPosition.x);
        float height = Mathf.Abs(startPosition.y - endPosition.y);

        selectionBox.anchoredPosition = new Vector2(x, y);
        selectionBox.sizeDelta = new Vector2(width, height);
    }

    private void HighlightAgentsInBox()
    {
        Rect selectionRect = new(
            selectionBox.anchoredPosition,
            selectionBox.sizeDelta
        );

        foreach (Agent agent in allAgents)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(agent.transform.position);
            bool isInBox = selectionRect.Contains(screenPos);

            if (isInBox && !highlightedAgents.Contains(agent))
            {
                highlightedAgents.Add(agent);
                AgentSignals.Instance.onHighlightAgent(agent.gameObject, true);
            }
            else if (!isInBox && highlightedAgents.Contains(agent))
            {
                highlightedAgents.Remove(agent);
                AgentSignals.Instance.onHighlightAgent(agent.gameObject, false);
            }
        }
    }
}
