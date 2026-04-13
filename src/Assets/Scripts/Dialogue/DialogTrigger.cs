using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.ResumeDialogue();
                BarkManager.Instance.OnDialogTriggered();
                hasTriggered = true;
            }
        }
    }
}