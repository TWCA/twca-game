using UnityEngine;

public class DialogAutoStart : MonoBehaviour
{
    private void Start()
    {
        DialogManager.Instance.StartDialog("checkup_text");
    }
}
