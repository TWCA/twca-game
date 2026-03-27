using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.ResumeDialogue();
                hasTriggered = true;
            }
        }
    }
}