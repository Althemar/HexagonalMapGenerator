using UnityEngine;

public class HexCell : MonoBehaviour {

    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;

    private int elevation = int.MinValue;
    private int terrainTypeIndex;

    public int Elevation
    {
        get {
            return elevation;
        }
        set {
            if (elevation == value) {
                return;
            }

            elevation = value;
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.elevationStep;
            transform.localPosition = position;

            Vector3 uiPosition = uiRect.localPosition;
            uiPosition.z = value * -HexMetrics.elevationStep;
            uiRect.localPosition = uiPosition;

            Refresh();
        }
    }

    public Color Color
    {
        get {
            return HexMetrics.colors[terrainTypeIndex];
        }
    }

    public int TerrainTypeIndex
    {
        get {
            return terrainTypeIndex;
        }
        set {
            if (terrainTypeIndex != value) {
                terrainTypeIndex = value;
                Refresh();
            }
        }
    }

    public void SetTerrainTypeIndex(int value) {
        terrainTypeIndex = value;
    }

    public void SetElevation(int value) {
        elevation = value;
        Vector3 position = transform.localPosition;
        position.y = value * HexMetrics.elevationStep;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = value * -HexMetrics.elevationStep;
        uiRect.localPosition = uiPosition;
    }
    
    

    

    [SerializeField]
    private HexCell[] neighbors;

    public HexCell GetNeighbor(HexDirection direction) {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction) {
        return HexMetrics.GetEdgeType(elevation, neighbors[(int)direction].elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell) {
        return HexMetrics.GetEdgeType(elevation, otherCell.elevation);
    }

    public void Refresh() {
        if (chunk) {
            chunk.Refresh();
            for (int i = 0; i < neighbors.Length; i++) {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk) {
                    neighbor.chunk.Refresh();
                }
            }
        }
    }
}
