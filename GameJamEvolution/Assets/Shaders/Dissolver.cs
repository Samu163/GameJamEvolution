using System.Collections;
using UnityEngine;

public class DissolveManager : MonoBehaviour
{
    public float dissolveDuration = 2f;
    public Material baseDissolveMaterial;

    public void StartDissolve(GameObject target, System.Action onComplete = null)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material dissolveMaterial = new Material(baseDissolveMaterial);
            renderer.material = dissolveMaterial;

            if (dissolveMaterial.HasProperty("_DissolveStrength"))
            {
                StartCoroutine(DissolveEffect(dissolveMaterial, onComplete, target));
            }
            else
            {
                Debug.LogError($"El material de {renderer.gameObject.name} no tiene el parámetro '_DissolveStrength'.");
            }
        }
    }

    private IEnumerator DissolveEffect(Material dissolveMaterial, System.Action onComplete, GameObject target)
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            float dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            dissolveMaterial.SetFloat("_DissolveStrength", dissolveStrength);

            yield return null;
        }

        Debug.Log($"Efecto de disolución completado para {target.name}.");

        onComplete?.Invoke();
    }
}

