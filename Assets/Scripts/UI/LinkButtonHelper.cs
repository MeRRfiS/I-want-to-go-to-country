using UnityEngine;

public class LinkButtonHelper : MonoBehaviour
{
    [SerializeField] private string _url;

    public void OpenLink()
    {
        Application.OpenURL(_url);
    }
}