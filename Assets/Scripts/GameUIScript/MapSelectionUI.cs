using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSelectionUI : MonoBehaviour
{
    public Image mapImage; // The central image that displays the map
    public Sprite[] mapSprites; // List of map images
    private int currentMapIndex = 0;

    void Start()
    {
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

    void UpdateMapDisplay()
    {
        mapImage.sprite = mapSprites[currentMapIndex];
    }

    public void LoadSelectedMap()
{
        int sceneIndex = currentMapIndex + 1; // Offset by 1 to skip menu (0)
        SceneManager.LoadSceneAsync(sceneIndex);
}

}
