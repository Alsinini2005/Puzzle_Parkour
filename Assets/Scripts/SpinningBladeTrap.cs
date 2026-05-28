using UnityEngine;

public class SpinningBladeTrap : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 360f, 0);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            
            if (player != null)
            {
                player.SendMessage("DropItem", SendMessageOptions.DontRequireReceiver);
            }

            UIManager uiManager = Object.FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.TriggerFail();
                Debug.Log("Player hit the blade! TriggerFail executed.");
            }
        }
    }
}