using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    public Rigidbody playerRigidbody;
    public TMP_Text speedText;
    public string displayFormat = "Speed: {0:F1}";
    public Color textColor = Color.black;
    private PlayerMovement playerMovement;
    
    void Start()
    {
        
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerRigidbody = playerMovement.GetComponent<Rigidbody>();
        }
        CreateSpeedText();
        speedText.color = textColor;
    }
    
    void Update()
    {
        float speed;
        speed = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z).magnitude;
        speedText.text = string.Format(displayFormat, speed);
    }
    
    void CreateSpeedText()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        
        GameObject canvasObj = new GameObject("Speed Screen");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        GameObject textObj = new GameObject("Speed Canvas");
        textObj.transform.SetParent(canvas.transform, false);
        
        speedText = textObj.AddComponent<TextMeshProUGUI>();
        speedText.alignment = TextAlignmentOptions.TopLeft;
        speedText.fontSize = 24;

        RectTransform rectTransform = speedText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(20, -20);
        rectTransform.sizeDelta = new Vector2(200, 50);
    }
}