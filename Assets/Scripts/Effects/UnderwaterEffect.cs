using UnityEngine;

[ExecuteInEditMode]
public sealed class UnderwaterEffect : MonoBehaviour
{
    public Shader underwaterShader;
    private Material underwaterMaterial;

    private void Start()
    {
        if (!underwaterShader)
        {
            Debug.LogError("Please assign a shader in the inspector.");
            enabled = false;
            return;
        }

        underwaterMaterial = new Material(underwaterShader);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (underwaterMaterial)
        {
            Graphics.Blit(src, dest, underwaterMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
