using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlash : MonoBehaviour
{
    public Image flashImage;
    public float flashDuration = 0.3f;

    public void Flash()
    {
        StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        flashImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        flashImage.gameObject.SetActive(false);
    }
}
