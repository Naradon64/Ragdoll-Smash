using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MapSelectionUI : MonoBehaviour
{
    public Image mapImage; // The central image that displays the map
    public Sprite[] mapSprites; // List of map images
    private int currentMapIndex = 0;

    public TMP_Text mapNameText;
    void Start()
    {
        mapNameText = transform.Find("Map Name").GetComponent<TMP_Text>();
        UpdateMapDisplay();
    }

    public void NextMap()
    {
        currentMapIndex = (currentMapIndex + 1) % mapSprites.Length;
        UpdateMapDisplay();
    }

    public void PreviousMap()
    {
        currentMapIndex = (currentMapIndex - 1 + mapSprites.Length) % mapSprites.Length;
        UpdateMapDisplay();
    }
    string GetSceneNameFromIndex(int index)
    {
        // Assuming you have a list of scene names, or you can derive it from the index.
        // Here, I'm using a simple array of scene names as an example:
        string[] sceneNames = { "Greenwood", "Demo Map"};

        return sceneNames[index]; // Return the corresponding scene name based on the index
    }

    void UpdateMapDisplay()
    {
        mapImage.sprite = mapSprites[currentMapIndex];
        string sceneName = GetSceneNameFromIndex(currentMapIndex);
    
        // Update the map name text
        mapNameText.text = sceneName;
    }

    public void LoadSelectedMap()
{
        int sceneIndex = currentMapIndex + 1; // Offset by 1 to skip menu (0)
        SceneManager.LoadSceneAsync(sceneIndex);
}

}
