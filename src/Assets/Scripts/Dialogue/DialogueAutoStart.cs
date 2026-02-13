using UnityEngine;

public class DialogAutoStart : MonoBehaviour
{
    public string dialogKnot;
    private void Start()
    {
        DialogManager.Instance.StartDialog(dialogKnot);
    }
}
