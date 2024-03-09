using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    private Player player;
    private Tilemap tilemap;
    private Dictionary<Vector3Int, TileBase> originalTiles = new Dictionary<Vector3Int, TileBase>();
    private List<SpriteRenderer> mapLayerSprites = new List<SpriteRenderer>();
    public int tileRadius = 10;
    public int iconRadius = 20; // Radius to reveal/hide icon images
    private bool isStarted = false;

    // Start is called before the first frame update
    public void StartFog()
    {
        player = SessionManager.player;
        tilemap = GetComponent<Tilemap>();
        HideAllTiles();
        HideAllMapLayerSprites();
        isStarted = true;
        Debug.Log("Fog Started");
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted)
        {
            UpdateVisibleTiles();
            UpdateVisibleMapLayerSprites();
        }
    }

    void HideAllTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                originalTiles[pos] = tile;
                tilemap.SetTile(pos, null); // Remove tile from the map
            }
        }
    }

    void HideAllMapLayerSprites()
    {
        SpriteRenderer[] spriteRenderers = FindObjectsOfType<SpriteRenderer>();
        foreach (SpriteRenderer renderer in spriteRenderers)
        {
            if (renderer.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                mapLayerSprites.Add(renderer);
                renderer.enabled = false; // Hide the sprite renderer
            }
        }
    }

    void UpdateVisibleTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (Vector3.Distance(tilemap.CellToWorld(pos), player.transform.position) <= tileRadius)
            {
                if (originalTiles.ContainsKey(pos))
                {
                    tilemap.SetTile(pos, originalTiles[pos]); // Add back the original tile
                }
            }
        }
    }

    void UpdateVisibleMapLayerSprites()
    {
        // Create a temporary list to store destroyed SpriteRenderers
        List<SpriteRenderer> destroyedSprites = new List<SpriteRenderer>();

        // Iterate through each SpriteRenderer in the list
        foreach (SpriteRenderer spriteRenderer in mapLayerSprites)
        {
            // Check if the SpriteRenderer is null or destroyed
            if (spriteRenderer == null || spriteRenderer.Equals(null))
            {
                // If it's destroyed, add it to the temporary list
                destroyedSprites.Add(spriteRenderer);
                continue; // Skip to the next SpriteRenderer
            }

            // Check the distance between the sprite's position and the player's position
            if (Vector3.Distance(spriteRenderer.transform.position, player.transform.position) <= iconRadius)
            {
                spriteRenderer.enabled = true; // Reveal the sprite renderer
            }
            else
            {
                spriteRenderer.enabled = false; // Hide the sprite renderer
            }
        }

        // Remove any destroyed SpriteRenderers from the main list
        foreach (SpriteRenderer destroyedSprite in destroyedSprites)
        {
            mapLayerSprites.Remove(destroyedSprite);
        }
    }
}
