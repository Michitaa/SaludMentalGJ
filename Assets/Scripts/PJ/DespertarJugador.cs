using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DespertarInteractivo : MonoBehaviour
{
    public RectTransform topLid;
    public RectTransform bottomLid;
    public Button wakeButton;

    public int clicksToWakeUp = 3;
    public int currentClicks = 0;

    public float openSpeed = 300f;
    private bool isAwake = false;

    private Vector2 topClosedPos;
    private Vector2 bottomClosedPos;
    private Vector2 topOpenPos;
    private Vector2 bottomOpenPos;
    public PensamientosTristes pensamientosTristes;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Posiciones iniciales
        topClosedPos = topLid.anchoredPosition;
        bottomClosedPos = bottomLid.anchoredPosition;

        float screenHeight = Screen.height;

        topOpenPos = new Vector2(topClosedPos.x, topClosedPos.y + screenHeight);
        bottomOpenPos = new Vector2(bottomClosedPos.x, bottomClosedPos.y - screenHeight);

        wakeButton.gameObject.SetActive(false);
        wakeButton.onClick.AddListener(OnWakeButtonClick);

        // Mostrar bot�n luego de 3 segundos
        StartCoroutine(ShowWakeButtonAfterDelay(3f));
    }

    public void OnWakeButtonClick()
    {
        Debug.Log("Click detectado en el bot�n");

        currentClicks++;

        if (currentClicks >= clicksToWakeUp && !isAwake)
        {
            isAwake = true;
            wakeButton.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Lanzamos la corrutina que primero abre los ojos y luego muestra el pensamiento
            StartCoroutine(AbrirOjosYMostrarPensamiento());
        }
        else
        {
            StartCoroutine(ShakeAndHideButton()); // nueva animaci�n
            StartCoroutine(ShowWakeButtonAfterDelay(0.5f));
        }
    }
    IEnumerator ShakeAndHideButton()
    {
        // Cambia color a rojo
        wakeButton.image.color = Color.red;

        Vector3 originalPos = wakeButton.transform.localPosition;

        // Sacudida simple por 0.3s
        float duration = 0.3f;
        float strength = 10f;
        float time = 0f;

        while (time < duration)
        {
            Vector3 offset = new Vector3(
                Random.Range(-strength, strength),
                Random.Range(-strength, strength),
                0);
            wakeButton.transform.localPosition = originalPos + offset;
            time += Time.deltaTime;
            yield return null;
        }

        // Reset posici�n y color
        wakeButton.transform.localPosition = originalPos;
        wakeButton.image.color = Color.white;

        wakeButton.gameObject.SetActive(false);
    }
    private IEnumerator AbrirOjosYMostrarPensamiento()
    {
        // Esperar a que termine la animaci�n de abrir ojos
        yield return StartCoroutine(OpenEyes());

        // Esperar 3 segundos m�s
        yield return new WaitForSeconds(0.5f);

        // Mostrar el pensamiento
        if (pensamientosTristes != null)
        {
            pensamientosTristes.MostrarPensamiento("�Por qu� me levant�? �Qu� sentido tiene...?");
        }
    }

    public IEnumerator OpenEyes()
    {
        while (Vector2.Distance(topLid.anchoredPosition, topOpenPos) > 0.5f)
        {
            topLid.anchoredPosition = Vector2.MoveTowards(topLid.anchoredPosition, topOpenPos, openSpeed * Time.deltaTime);
            bottomLid.anchoredPosition = Vector2.MoveTowards(bottomLid.anchoredPosition, bottomOpenPos, openSpeed * Time.deltaTime);
            yield return null;
        }

        topLid.gameObject.SetActive(false);
        bottomLid.gameObject.SetActive(false);
    }

    IEnumerator ShowWakeButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isAwake)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Reubicar el bot�n a posici�n aleatoria en pantalla
            Vector2 randomPos = new Vector2(
                Random.Range(100f, Screen.width - 100f),
                Random.Range(100f, Screen.height - 100f)
            );

            Vector2 localPoint;
            RectTransform canvasRect = wakeButton.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, randomPos, null, out localPoint);

            wakeButton.GetComponent<RectTransform>().anchoredPosition = localPoint;

            wakeButton.gameObject.SetActive(true);
        }
    }
   
}
