using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public Transform transformCamera;
    public Transform spawnPoint;
    public Transform leakPoint;
    public GameObject playButtonLockPanel;

    [Header("Build Mode")]
    public int gold = 50;
    public LayerMask stageLayerMask;
    public Transform highlighter;
    public RectTransform towerSellingPanel;
    public TMP_Text sellRefundText;
    public TMP_Text currentGoldText;
    public Color selectedBuildButtonColor = new Color(.2f, .8f, .2f);

    [Header("Play Mode")]
    public GameObject buildButtonPanel;
    public GameObject gameLostPanel;
    public TMP_Text gameLostPanelInfoText;
    public GameObject playButton;
    public Transform enemyHolder;
    public Enemy groundEnemyPrefab;
    public Enemy flyingEnemyPrefab;
    public float enemySpawnRate = .35f;
    public int flyingLevelInterval = 4;
    public int enemiesPerLevel = 15;
    public int goldRewardPerLevel = 12;
    public static int level = 1;
    public static int remainingLives = 40;

    private int enemiesSpawnedThisLevel = 0; 
    private Vector3 lastMousePosition;
    private int goldLastFrame;
    private bool cursorIsOverStage = false;
    private Tower towerPrefabToBuild = null;
    private Image selectedBuildButtonImage = null;
    private Tower selectedTower = null;
    private Vector3 targetPosition;
    private enum Mode
    {
        Build,
        Play
    }
    private Mode mode = Mode.Build;
    private Dictionary<Vector3, Tower> towers = new Dictionary<Vector3, Tower>();

    // UI events
    public void DeselectTower()
    {
        selectedTower = null;
        towerSellingPanel.gameObject.SetActive(false);
    }
    public void OnSellTowerButtonClicked()
    {
        if (selectedTower != null)
        {
            SellTower(selectedTower);
        }
    }
    public void OnBuildButtonClicked(Tower associatedTower)
    {
        towerPrefabToBuild = associatedTower;
        DeselectTower();
    }
    public void SetSelectedBuildButton(Image clickedButtonImage)
    {
        selectedBuildButtonImage = clickedButtonImage;
        clickedButtonImage.color = selectedBuildButtonColor;
    }
    public void StartLevel()
    {
        GoToPlayMode();
        InvokeRepeating("SpawnEnemy", .5f, enemySpawnRate);
    }


    // Build Mode
    void BuildModeLogic()
    {
        PositionHighlighter();
        PositionSellPanel();
        UpdateCurrentGold();
        if (cursorIsOverStage && Input.GetMouseButtonDown(0))
        {
            OnStageClicked();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectTower();
            DeselectBuildButton();
        }
    }
    void PositionHighlighter()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stageLayerMask.value))
            {
                Vector3 point = hit.point;
                point.x = Mathf.Round(hit.point.x * 0.1f) * 10;
                point.z = Mathf.Round(hit.point.z * 0.1f) * 10;

                point.z = Mathf.Clamp(point.z, -80, 80);

                point.y = .2f;

                highlighter.position = point;
                highlighter.gameObject.SetActive(true);
                cursorIsOverStage = true;
            }
            else
            {
                cursorIsOverStage = false;
                highlighter.gameObject.SetActive(false);
            }
        }
        lastMousePosition = Input.mousePosition;
    }
    void PositionSellPanel()
    {
        if (selectedTower != null)
        {
            var screenPosition = Camera.main.WorldToScreenPoint(selectedTower.transform.position + Vector3.forward * 8);
            towerSellingPanel.position = screenPosition;
        }
    }
    void UpdateCurrentGold()
    {
        if (gold != goldLastFrame)
        {
            currentGoldText.text = gold + " gold";
        }
        goldLastFrame = gold;
    }
    void OnStageClicked()
    {
        if (towerPrefabToBuild != null)
        {
            if (!towers.ContainsKey(highlighter.position) && gold >= towerPrefabToBuild.goldCost)
            {
                BuildTower(towerPrefabToBuild, highlighter.position);
            }
        }
        else
        {
            if (towers.ContainsKey(highlighter.position))
            {
                selectedTower = towers[highlighter.position];
                sellRefundText.text = "for " + Mathf.CeilToInt(selectedTower.goldCost * selectedTower.refundFactor) + " gold";
                towerSellingPanel.gameObject.SetActive(true);
            }
        }
    }
    void DeselectBuildButton()
    {
        towerPrefabToBuild = null;
        if (selectedBuildButtonImage != null)
        {
            selectedBuildButtonImage.color = Color.white;
            selectedBuildButtonImage = null;
        }
    }
    void BuildTower(Tower prefab, Vector3 position)
    {
        towers[position] = Instantiate<Tower>(prefab, position, Quaternion.identity);
        gold -= towerPrefabToBuild.goldCost;
        UpdateEnemyPath();
    }
    void SellTower(Tower tower)
    {
        DeselectTower();
        gold += Mathf.CeilToInt(tower.goldCost * tower.refundFactor);
        towers.Remove(tower.transform.position);
        Destroy(tower.gameObject);
        UpdateEnemyPath();
    }
    void UpdateEnemyPath() 
    {
        Invoke("PerformPathfinding", 0.1f);
    }
    void PerformPathfinding()
    {
        NavMesh.CalculatePath(spawnPoint.position, leakPoint.position, NavMesh.AllAreas, GroundEnemy.path);
        if (GroundEnemy.path.status == NavMeshPathStatus.PathComplete)
        {
            playButtonLockPanel.SetActive(false);
        }
        else
        {
            playButtonLockPanel.SetActive(true);
        }
    }
    
    // Build Mode
    public void PlayModeLogic()
    {
        if (enemyHolder.childCount == 0 && enemiesSpawnedThisLevel >= enemiesPerLevel)
        {
            if (remainingLives > 0)
            {
                GoToBuildMode();
            }
            else
            {
                gameLostPanelInfoText.text = "You had " + remainingLives + " lives by the end and made it to level " + level + ".";
                gameLostPanel.SetActive(true);
            }
        }
    }
    void GoToPlayMode()
    {
        mode = Mode.Play;

        buildButtonPanel.SetActive(false);
        playButton.SetActive(false);
        highlighter.gameObject.SetActive(false);
    }
    void GoToBuildMode()
    {
        mode = Mode.Build;
        
        buildButtonPanel.SetActive(true);
        playButton.SetActive(true);

        enemiesSpawnedThisLevel = 0;
        level++;
        gold += goldRewardPerLevel;
    }
    void SpawnEnemy()
    {
        Enemy enemy = null;
        if (level % flyingLevelInterval == 0)
        {
            enemy = Instantiate(flyingEnemyPrefab, spawnPoint.position + (Vector3.up * 18), Quaternion.LookRotation(Vector3.back));
        }
        else
        {
            enemy = Instantiate(groundEnemyPrefab, spawnPoint.position, Quaternion.LookRotation(Vector3.back));
        }
        enemy.trans.SetParent(enemyHolder);
        enemiesSpawnedThisLevel += 1;
        if (enemiesSpawnedThisLevel >= enemiesPerLevel)
        {
            CancelInvoke("SpawnEnemy");
        }
    }

    void Start()
    {
        targetPosition = transformCamera.position;
        GroundEnemy.path = new NavMeshPath();
        UpdateEnemyPath();
    }
    void Update()
    {

        if (mode == Mode.Build)
        {
            BuildModeLogic();
        }
        else
        {
            PlayModeLogic();
        }
    }
}
