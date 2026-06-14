using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class Game : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public int mineCount = 55;

    private Board board;
    private CellGrid grid;
    private bool gameover;
    private bool generated;
    public bool isMine;
    public bool isPuase;
    private bool isWin = false;
    private float currentProgress = 0f;
    private PlayerManager pm;
    private void OnValidate()
    {
        mineCount = Mathf.Clamp(mineCount, 0, width * height);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        board = GetComponentInChildren<Board>();
        pm = PlayerManager.Instance;
    }

    public void NewGame()
    {
        StopAllCoroutines();

        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);

        GameManager.Instance.revealTiles = 0;
        GameManager.Instance.rightFlags = 0;
        GameManager.Instance.explodeCount = 0;

        currentProgress = 0f;

        gameover = false;
        generated = false;
        isPuase = false;

        grid = new CellGrid(width, height);
        board.Draw(grid);
    }

    private void Update()
    {

        if (!gameover && isMine && !isPuase && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            if (Input.GetMouseButtonDown(0)) {
                Reveal();
            } else if (Input.GetMouseButtonDown(1)) {
                Flag();
            } else if (Input.GetMouseButton(2)) {
                Chord();
            } else if (Input.GetMouseButtonUp(2)) {
                Unchord();
            }
        }
        if (gameover)
        {
            GameManager.Instance.EndGame(isWin);
        }
    }

    private void CalculateRightFlags()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].flagged && grid[x,y].exploded)
                    GameManager.Instance.rightFlags++;
            }
        }
    }

    private void Reveal()
    {
        if (TryGetCellAtMousePosition(out Cell cell))
        {
            if (!generated)
            {
                grid.GenerateMines(cell, mineCount);
                grid.GenerateNumbers();
                generated = true;
            }

            Reveal(cell);
        }
    }

    private void Reveal(Cell cell)
    {
        if (cell.revealed) return;
        if (cell.flagged) return;

        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;

            case Cell.Type.Empty:
                StartCoroutine(Flood(cell));
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                GameManager.Instance.revealTiles++;
                SFXManager.Instance.PlayRevealSFX();
                CheckWinCondition();
                break;
        }

        board.Draw(grid);

        UpdateProgress();
    }

    private void UpdateProgress()
    {
        float revealCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].revealed)
                    revealCount++;
            }
        }
        currentProgress = revealCount / ((width * height) - mineCount);
        UIManager.Instance.SliderProgressUI(currentProgress);
        if (PlayerManager.Instance.UsableItem() && currentProgress >= 0.5f)
        {
            PlayerManager.Instance.GetItem();
            UIManager.Instance.ActiveItemUI();
            SFXManager.Instance.PlayGetItemSFX();
        }
    }

    private IEnumerator Flood(Cell cell)
    {
        if (gameover) yield break;
        if (cell.revealed) yield break;
        if (cell.type == Cell.Type.Mine) yield break;

        cell.revealed = true;
        GameManager.Instance.revealTiles++;

        UpdateProgress();
        SFXManager.Instance.PlayRevealSFX();
        board.Draw(grid);

        yield return null;

        if (cell.type == Cell.Type.Empty)
        {
            if (grid.TryGetCell(cell.position.x - 1, cell.position.y, out Cell left)) {
                StartCoroutine(Flood(left));
            }
            if (grid.TryGetCell(cell.position.x + 1, cell.position.y, out Cell right)) {
                StartCoroutine(Flood(right));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y - 1, out Cell down)) {
                StartCoroutine(Flood(down));
            }
            if (grid.TryGetCell(cell.position.x, cell.position.y + 1, out Cell up)) {
                StartCoroutine(Flood(up));
            }
        }
    }

    private void Flag()
    {
        if (!TryGetCellAtMousePosition(out Cell cell)) return;
        if (cell.revealed) return;

        cell.flagged = !cell.flagged;

        UpdateProgress();
        board.Draw(grid);
    }

    private void Chord()
    {
        // unchord previous cells
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].chorded = false;
            }
        }

        // chord new cells
        if (TryGetCellAtMousePosition(out Cell chord))
        {
            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    int x = chord.position.x + adjacentX;
                    int y = chord.position.y + adjacentY;

                    if (grid.TryGetCell(x, y, out Cell cell)) {
                        cell.chorded = !cell.revealed && !cell.flagged;
                    }
                }
            }
        }

        board.Draw(grid);
    }

    private void Unchord()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.chorded) {
                    Unchord(cell);
                }
            }
        }

        board.Draw(grid);
    }

    private void Unchord(Cell chord)
    {
        chord.chorded = false;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }

                int x = chord.position.x + adjacentX;
                int y = chord.position.y + adjacentY;

                if (grid.TryGetCell(x, y, out Cell cell))
                {
                    if (cell.revealed && cell.type == Cell.Type.Number)
                    {
                        if (grid.CountAdjacentFlags(cell) >= cell.number)
                        {
                            Reveal(chord);
                            return;
                        }
                    }
                }
            }
        }
    }

    private void Explode(Cell cell)
    {
        GameManager.Instance.explodeCount++;
        if (pm.CurrentLives > 1)
        {
            cell.exploded = true;
            cell.revealed = true;
            // 폭발 소리 추가
            SFXManager.Instance.PlayExplodeSFX();
            pm.LoseLife();
        }
        else
        {
            CalculateRightFlags();
            gameover = true;
            isWin = false;
            // Set the mine as exploded
            cell.exploded = true;
            cell.revealed = true;

            // Reveal all other mines
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cell = grid[x, y];

                    if (cell.type == Cell.Type.Mine)
                    {
                        // 폭발 소리 추가

                        cell.revealed = true;
                    }
                }
            }
        }
    }

    private void CheckWinCondition()
    {
        if (currentProgress < 1f) {
            return; // no win
        }
        isWin = true;
        CalculateRightFlags();
        gameover = true;

        // Flag all the mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];

                if (cell.type == Cell.Type.Mine) {
                    cell.flagged = true;
                }
            }
        }
    }

    private bool TryGetCellAtMousePosition(out Cell cell)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        return grid.TryGetCell(cellPosition.x, cellPosition.y, out cell);
    }

}
