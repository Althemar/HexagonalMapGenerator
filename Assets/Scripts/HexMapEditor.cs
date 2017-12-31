using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour {

    public HexGrid hexGrid;

    private int activeTerrainTypeIndex;

    private int activeElevation;
    private bool applyElevation = true;

    private int brushSize;

    private void Update() {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            HandleInput();
        }
    }

    private void HandleInput() {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    private void EditCells(HexCell center) {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++) {
            for (int x = centerX - r; x <= centerX + brushSize; x++) {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++) {
            for (int x = centerX - brushSize; x <= centerX + r; x++) {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private void EditCell(HexCell cell) {
        if (cell) {
            if (activeTerrainTypeIndex >= 0) {
                cell.TerrainTypeIndex = activeTerrainTypeIndex;
            }
            if (applyElevation) {
                cell.Elevation = activeElevation;
            }
        }
    }

    public void SetTerrainTypeIndex(int index) {
        activeTerrainTypeIndex = index;
    }
    
    public void SetElevation(float elevation) { 
         activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle) {
        applyElevation = toggle;
    }

    public void SetBrushSize(float size) {
        brushSize = (int)size;
    }

    public void ShowUI(bool visible) {
        hexGrid.ShowUI(visible);
    }
}
