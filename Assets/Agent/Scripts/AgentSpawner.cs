using UnityEngine;
using UnityEngine.InputSystem;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField] AIAgent[] agents;
    [SerializeField] LayerMask layerMask = Physics.AllLayers;

    Camera activeCamera;
    int agentIndex = 0;

    void Start()
    {
        activeCamera = Camera.main; 
    }

    void Update()
    {
        if(Keyboard.current.tabKey.wasPressedThisFrame)
        {
            agentIndex = (agentIndex + 1) % agents.Length;
        }
        if (Mouse.current.leftButton.wasPressedThisFrame ||
           (Mouse.current.leftButton.IsPressed() && Keyboard.current.leftCtrlKey.isPressed))
        {
            Ray ray = activeCamera.ScreenPointToRay(Mouse.current.position.value);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100.0f, layerMask))
            {
                Instantiate(agents[agentIndex], hitInfo.point, Quaternion.Euler(0, Random.Range(0, 360), 0));
            }
        }
    }
}
