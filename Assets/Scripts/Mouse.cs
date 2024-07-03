using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class Mouse : MonoBehaviour
{
    [SerializeField]
    private GameObject mouse;
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
        mouse.SetActive(GameManager.gameState == GameState.Build);
        if (EventSystem.current.IsPointerOverGameObject()) return;

        mousePos = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit rayHit, 999f, mouseLayerMask))
        {
            mouseCurrentPos = rayHit.point;

            //This function should only be responsible for mouse position on the grid. 
            //Input actions will be taken out from here to an input manager as well as using the new input system
            if (Input.GetMouseButtonDown(0))
            {
                clickPosition = rayHit.point;

                switch (GameManager.gameState)
                {
                    case GameState.Build:
                        GridGenDeleteMe.PlaceItem(entityData, new Vector3Int((int)clickPosition.x, (int)clickPosition.y, (int)clickPosition.z));
                        break;

                    case GameState.Manage:
                        //Check if shift is pressed, if so don't do anything, else just have the selected agents be diselected
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            //Unselect the selected agents
                            AgentSignals.Instance.onDeselectAgents?.Invoke();
                        }

                        if (rayHit.transform.gameObject.GetComponent<Agent>() != null)
                        {
                            //Update UI, agent's highlights, and add the agent to the list of selected agents
                            AgentSignals.Instance.onSelectAgent?.Invoke(rayHit.transform.gameObject);
                        }

                        else
                        {
                            //This will later be responsible for the buildings to respond, although it is prefered to combine them as Entity
                            Debug.LogWarning(rayHit.transform.gameObject.name);
                        }
                        break;

                    default: break;
                }
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

        verticalSpeed = CameraVerticalMovement(verticalSpeed, clampX);
        horizontalSpeed = CameraHorizontalMovement(horizontalSpeed, clampY);

        Vector3 movement = new Vector3(Mathf.Abs(verticalSpeed), 0, Mathf.Abs(horizontalSpeed)).normalized;
        cam.transform.position += new Vector3(verticalSpeed * movement.x * Time.deltaTime, 0, horizontalSpeed * movement.z * Time.deltaTime);
    }

    private float CameraVerticalMovement(float verticalSpeed, float clampX)
    {
        if (mousePos.x > Screen.width - padding && cam.transform.position.x < 90)
        {
            float min = Screen.width - padding;
            verticalSpeed = cameraSpeed * Mathf.Abs(clampX - min);
        }

        else if (mousePos.x < padding && cam.transform.position.x > 5)
        {
            verticalSpeed = -cameraSpeed * Mathf.Abs(clampX - padding);
        }

        verticalSpeed = Mathf.Clamp(verticalSpeed, -cameraSpeed * 100, cameraSpeed * 100);
        return verticalSpeed;
    }

    private float CameraHorizontalMovement(float horizontalSpeed, float clampY)
    {
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
        return horizontalSpeed;
    }

    public Vector3 GetMousePosition()
    {
        return mouse.transform.position;
    }
}