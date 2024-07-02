using UnityEngine;
using Signals;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject holder;
    [SerializeField]
    private Image entityImage;
    [SerializeField]
    private TextMeshProUGUI entityName;

    private void OnSelectNewEntity(EntityOS entity)
    {
        holder.SetActive(true);
        entityImage.sprite = entity.entityImage;
        entityName.text = entity.entityName;
    }

    private void OnEntityRemove()
    {
        holder.SetActive(false);
    }

    private void OnEnable()
    {
        BuildSignals.Instance.onSelectEntity += OnSelectNewEntity;
        //UISignals.Instance.onEntityRemove += OnEntityRemove;
    }

    private void OnDisable()
    {
        if (BuildSignals.Instance == null) return;
        BuildSignals.Instance.onSelectEntity -= OnSelectNewEntity;
        //UISignals.Instance.onEntityRemove -= OnEntityRemove;
    }

    public void AssignGameObject(EntityOS entity)
    {
        BuildSignals.Instance.onSelectEntity?.Invoke(entity);
    }

    public void ShowObject(GameObject toShow)
    {
        toShow.SetActive(true);
    }
}