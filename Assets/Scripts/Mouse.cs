using UnityEngine;
using UnityEngine.EventSystems;

public class Mouse : MonoBehaviour
{
    [SerializeField]
    private GameObject mouse;
    [SerializeField]
    private GUIStyle mouseSelectBoxStyle;
    [SerializeField]
    private LayerMask mouseLayerMask;

    [SerializeField]
    private EntityOS entityData; // This has to be taken out to the BuildingSystem class. It will also be assigned according to communication with the UI system

    [SerializeField]
    private float cameraSpeed = 5f, padding = 50;

    [SerializeField]
    private Camera cam;

    private Vector3 clickPosition;
    private Vector3 mouseCurrentPos, mousePos;

    private bool isDragging;
    private float dragTimer;

    private void Update()
    {
        MoveCamera();
        MouseRay();
    }

    private void OnEnable()
    {
        BuildSignals.Instance.onSelectEntity += AdjustMouseScale;
    }

    private void OnDisable()
    {
        if (BuildSignals.Instance == null) return;
        BuildSignals.Instance.onSelectEntity -= AdjustMouseScale;
    }

    private void AdjustMouseScale(EntityOS entity)
    {
        entityData = entity;
        mouse.transform.localScale = new Vector3(0.1f * entity.entitySize.x, mouse.transform.localScale.y, 0.1f * entity.entitySize.y);
    }

    private void MouseRay()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        mousePos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePos);

        dragTimer -= Time.deltaTime;

        if (Physics.Raycast(ray, out RaycastHit rayHit, 999f, mouseLayerMask))
        {
            mouseCurrentPos = rayHit.point;

            //This function should only be responsible for mouse position on the grid. 
            //Input actions will be taken out from here to an input manager as well as using the new input system
            if (Input.GetMouseButtonDown(0))
            {
                dragTimer = 0.5f;
                clickPosition = rayHit.point;
                if (GameManager.gameState == GameState.Build)
                {
                    GridGenDeleteMe.PlaceItem(entityData, new Vector3Int((int)clickPosition.x, (int)clickPosition.y, (int)clickPosition.z));
                    return;
                }
                else
                {
                    if (rayHit.transform.gameObject.GetComponent<Agent>() != null)
                    {
                        AgentSignals.Instance.onSelectAgent?.Invoke(rayHit.transform.gameObject);
                    }
                    else Debug.LogWarning(rayHit.transform.gameObject.name);
                }
            }

            if (dragTimer > 0.2f && dragTimer < 0.4f)
            {
                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                dragTimer = 0;
            }
        }

        mouse.transform.position = new Vector3((int)mouseCurrentPos.x, (int)mouseCurrentPos.y, (int)mouseCurrentPos.z);
        mouse.GetComponent<MeshRenderer>().material.color = GridGenDeleteMe.CheckFreeSpace(entityData.entitySize, Vector3Int.FloorToInt(mouse.transform.position)) ? Color.green : Color.red;
    }

    private void MoveCamera()
    {
        float verticalSpeed = 0f;
        float horizontalSpeed = 0f;
        float clampX = Mathf.Min(Screen.width, mousePos.x);
        float clampY = Mathf.Min(Screen.height, mousePos.y);

        if (mousePos.x > Screen.width - padding && cam.transform.position.x < 90)
        {
            float min = Screen.width - padding;
            verticalSpeed = cameraSpeed * Mathf.Abs(clampX - min);
        }

        else if (mousePos.x < padding && cam.transform.position.x > 5)
        {
            verticalSpeed = -cameraSpeed * Mathf.Abs(clampX - padding);
        }

        if (mousePos.y > Screen.height - padding && cam.transform.position.z < 90)
        {
            float min = Screen.height - padding;
            horizontalSpeed = cameraSpeed * Mathf.Abs(clampY - min);
        }

        else if (mousePos.y < padding && cam.transform.position.z > -5)
        {
            horizontalSpeed = -cameraSpeed * Mathf.Abs(clampY - padding);
        }

        horizontalSpeed = Mathf.Clamp(horizontalSpeed, -cameraSpeed * 100, cameraSpeed * 100);
        verticalSpeed = Mathf.Clamp(verticalSpeed, -cameraSpeed * 100, cameraSpeed * 100);

        Vector3 movement = new Vector3(Mathf.Abs(verticalSpeed), 0, Mathf.Abs(horizontalSpeed)).normalized;
        cam.transform.position += new Vector3(verticalSpeed * movement.x * Time.deltaTime, 0, horizontalSpeed * movement.z * Time.deltaTime);
    }

    public Vector3 GetMousePosition()
    {
        return mouse.transform.position;
    }

    private void OnGUI()
    {
        if (isDragging)
        {
            float BoxWidth = cam.WorldToScreenPoint(clickPosition).x - cam.WorldToScreenPoint(mouseCurrentPos).x;
            float BoxHeight = cam.WorldToScreenPoint(clickPosition).y - cam.WorldToScreenPoint(mouseCurrentPos).y;
            float boxRight = (Screen.height - Input.mousePosition.y) - BoxHeight;
            GUI.Box(new Rect(
                mousePos.x,
                boxRight,
                BoxWidth,
                BoxHeight), "", mouseSelectBoxStyle);
        }
    }
}