using UnityEngine;

public class LinkGameSetup : MonoBehaviour
{
    [SerializeField] private Board boardPrefab;
    [SerializeField] private LinkManager linkManagerPrefab;
    [SerializeField] private Transform linkPrefab;
    
    [SerializeField] private bool setupInEditor = true;
    
    private Board board;
    private LinkManager linkManager;
    private ScoreCounter scoreCounter;
    
    private void Start()
    {
        if (!setupInEditor)
            SetupGame();
    }
    
    public void SetupGame()
    {
        SetupBoard();
        SetupLinkManager();
        scoreCounter = ScoreCounter.Instance;
    }
    
    private void SetupBoard()
    {
        if (board != null)
            return;
            
        board = FindObjectOfType<Board>();
        if (board == null && boardPrefab != null)
        {
            GameObject boardGO = Instantiate(boardPrefab.gameObject);
            boardGO.name = "Board";
            board = boardGO.GetComponent<Board>();
        }
    }
    
    private void SetupLinkManager()
    {
        if (linkManager != null)
            return;
            
        linkManager = FindObjectOfType<LinkManager>();
        if (linkManager == null && linkManagerPrefab != null)
        {
            GameObject linkManagerGO = Instantiate(linkManagerPrefab.gameObject);
            linkManagerGO.name = "LinkManager";
            linkManager = linkManagerGO.GetComponent<LinkManager>();
            
            if (linkManager != null && linkPrefab != null)
            {
                var field = linkManager.GetType().GetField("linkPrefab", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(linkManager, linkPrefab);
                }
            }
        }
    }

    public void ResetGame()
    {
        if (board != null)
        {
            DestroyImmediate(board.gameObject);
            board = null;
        }
        
        if (linkManager != null)
        {
            DestroyImmediate(linkManager.gameObject);
            linkManager = null;
        }
        
        if (ScoreCounter.Instance != null)
            ScoreCounter.Instance.Score = 0;
        
        scoreCounter = null;
    }
}
