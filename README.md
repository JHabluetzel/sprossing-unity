# sprossing-unity

https://axulart.itch.io/small-8-direction-characters
https://www.youtube.com/watch?v=FwOxLkJTXag
https://www.youtube.com/watch?v=AiZ4z4qKy44
https://www.youtube.com/@GameCodeLibrary/videos
https://www.youtube.com/watch?v=I3_i-x9nCjs
https://www.youtube.com/watch?v=TeEWLC-QKjw
https://www.youtube.com/watch?v=snUe2oa_iM0
https://www.youtube.com/watch?v=mZfyt03LDH4&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=3

UNITY

Create new project
Select 2D (Built-In Render Pipeline), change name & uncheck „connect to unity cloud“ & „unity version control“

OPTIONAL (Remove unneeded packages)

Window > Package Manager
Select the Packages in project (Dropdown menu next to top left plus button)
Remove unnecessary packages e.g. Visual Scripting, JetBrains Rider Editor (if your are using a different editor), Visual Studio Code Editor (even if you’re using VSCode like me (don’t ask me why😭) but then keep Visual Studio Editor), Timeline, Version Control

Window > TextMeshPro > Import TMP Essential Resources
Create folder & name it „Plugins“
Drag & Drop „TextMesh Pro“ into „Plugins“

2d Object > Tilemap > Rectangular
Rename „Tilemap“ to „ground1“
Duplicate 5x
Name it: „overlay1“, „ground2“, „overlay2“ …
Set „order in layer“ to: 0, 1, 3, 4, 6, 7
Select all tile maps & change transform to (x: 0.5, y: 0, z: 0)
Select „grid“ object & change cell size from (1, 1, 0) to (1, 0.8, 0)

Create folder & name it „Sprites“
Drag & drop „ground.png“, „player-base.png“, „player-blue.png“, „player-orange.png“
Select them all
Change sprite type from „Single“ to „Multiple“
Change pixels per unit from „100“ to „15“
Change filter mode from „bilinear“ to „point (no filter)“
Change compression from „Normal quality“ to „None“
Hit „apply“

Select „seasonalground“ & hit the „sprite editor“ button
Select „Slice“
Change type to „grid by cell size“ & set pixel size to x: 15, y: 12 & press „slice“
Hit apply & close window

Select „seasonalwater“ & hit the „sprite editor“ button
Select „Slice“
Change type to „grid by cell size“ & set pixel size to x: 15, y: 12 & press „slice“
Hit apply & close window

For each „player-…“
Select „Slice“
Set „pivot“ to „bottom center“
Change type to „grid by cell count“ & set columns & rows to c: 8, r: 3
Set padding to x: 1, y: 0 & press „slice“
Hit apply & close window

Create new folder & name it „Scripts“
Create new C# script
name it „seasons“

CODE EDITOR

Open the “seasons“ script & replace its contents with:

public enum Seasons
{
   Spring = 0, Summer = 1, Autumn = 2, Winter = 3
}

UNITY

Select „scripts“ folder
Create > 2d > Tiles > Custom rule tile script
Name it „SeasonalRuleTile“

CODE EDITOR

Open the “SeasonalRuleTile“ script & replace its contents with:

using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Seasonal/Seasonal Rule Tile")]
public class SeasonalRuleTile : RuleTile<SeasonalRuleTile>
{
    public Seasons season;
    public bool alwaysConnect;
    public TileBase[] tilesToConnect;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Any = 3;
        public const int Specific = 4;
        public const int NotSpecific = 5;
        public const int Nothing = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.This: return Check_This(tile);
            case Neighbor.NotThis: return Check_NotThis(tile);
            case Neighbor.Any: return Check_Any(tile);
            case Neighbor.Specific: return Check_Specific(tile);
            case Neighbor.NotSpecific: return Check_NotSpecific(tile);
            case Neighbor.Nothing: return Check_Nothing(tile);
        }

        return base.RuleMatch(neighbor, tile);
    }

    private bool Check_This(TileBase tile)
    {
        if (!alwaysConnect)
        {
            return tile == this;
        }
        else
        {
            return tilesToConnect.Contains(tile) || tile == this;
        }
    }


    private bool Check_NotThis(TileBase tile)
    {
        return tile != this;
    }

    private bool Check_Any(TileBase tile)
    {
        return tile != null || tile != this;
    }

    private bool Check_Specific(TileBase tile)
    {
        return tilesToConnect.Contains(tile);
    }

    private bool Check_NotSpecific(TileBase tile)
    {
        return tile == null || tile == this;
    }

    private bool Check_Nothing(TileBase tile)
    {
        return tile == null;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        var iden = Matrix4x4.identity;

        tileData.sprite = m_DefaultSprite;
        tileData.gameObject = m_DefaultGameObject;
        tileData.colliderType = m_DefaultColliderType;
        tileData.flags = TileFlags.LockTransform;
        tileData.transform = iden;

        Matrix4x4 transform = iden;

        foreach (TilingRule rule in m_TilingRules)
        {
            if (RuleMatches(rule, position, tilemap, ref transform))
            {
                switch (rule.m_Output)
                {
                    case TilingRuleOutput.OutputSprite.Single:
                    case TilingRuleOutput.OutputSprite.Animation:
                        tileData.sprite = rule.m_Sprites[0];
                        break;
                    case TilingRuleOutput.OutputSprite.Random:
                        tileData.sprite = rule.m_Sprites[(int)season];
                        if (rule.m_RandomTransform != TilingRuleOutput.Transform.Fixed)
                            transform = ApplyRandomTransform(rule.m_RandomTransform, transform, rule.m_PerlinScale, position);
                        break;
                }
                tileData.transform = transform;
                tileData.gameObject = rule.m_GameObject;
                tileData.colliderType = rule.m_ColliderType;
                break;
            }
        }
    }
}

UNITY

Create new folder & name it „Tiles“

Select folder
Create > Custom > Rule Tile
Name it „SeasonalCliffs“
Set a sprite for „Default Sprite“
Set „Number of Tiling rules“ to 4
Set the sprites for each rule & setup rules

Create > Seasonal > Connected Rule Tile
Name the rule tile „SeasonalGrass“
Set a sprite for „default sprite“
Add „SeasonalCliffs“ to „Tiles To Connect“
Set „number of tiling rules“ to 13
Set the sprites for each rule

Create > Custom > Rule Tile
Name it „SeasonalWater“
Set a sprite for „Default Sprite“
Set „Number of Tiling rules“ to 47
Set the sprites for each rule & setup rules

Window > 2d >Tile Palette
Drag „tile palette“ window into „inspector“ view for easy future access
Create new palette
Name the palette „AllTiles“
Change the cell size from „Automatic“ to „Manual“
Set the cell size to x: 1, y: 0.8, z: 0 & hit „create“
Select the „Tiles“ folder before hitting „choose“
Drag „grassrules“ from the „Tiles“ folder in the section between the palette dropdown menu & brush dropdown menu
Make sure „Tiles“ is selected before hitting „choose“
Press B & select the sprite & draw different shapes in the scene view (to test tile rules)
Press S to exit „brush mode“

Select „SeasonalGrass“ & setup the rules

Expand „player-base“ & drop „player-base-12“ into the scene
Reset the transform & under „Additional Settings“ set the „order in layer“ to 1
Rename it to „player“

Edit > Project Settings
Go to „Graphics“ & change „transparency sort mode“ to „custom axis“
Set the „transparency sort axis“ to x: 0, y: 1, z: 0 (y-sorting)
Close window

Select „scripts“ folder
Create new C# script
Name it „WorldManager“ & attach it to „Grid“ object

CODE EDITOR

Open the “WorldManager“ script & replace its contents with:

using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private TileBase grassTiles;
    [SerializeField] private TileBase cliffTiles;
    [SerializeField] private TileBase waterTiles;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            ConnectedRuleTile gt = (ConnectedRuleTile)grassTiles;
            switch (gt.season)
            {
                case Seasons.Spring:
                    gt.season = Seasons.Summer;
                    break;
                case Seasons.Summer:
                    gt.season = Seasons.Autumn;
                    break;
                case Seasons.Autumn:
                    gt.season = Seasons.Winter;
                    break;
                case Seasons.Winter:
                    gt.season = Seasons.Spring;
                    break;
            }

            SeasonalRuleTile ct = (SeasonalRuleTile)cliffTiles;
            ct.season = gt.season;
            SeasonalRuleTile wt = (SeasonalRuleTile)waterTiles;
            wt.season = gt.season;


            foreach (Tilemap map in GetComponentsInChildren<Tilemap>())
            {
                map.RefreshAllTiles();
            }
        }
    }
}

UNITY

Select „Inspector“ view & fill out „WorldManager“ section

Select „scripts“ folder
Create new C# script
Name it „playercontroller“

CODE EDITOR

Open the “playercontroller“ script & replace its contents with:

using System.Collections;
using UnityEngine;

public class playercontroller : MonoBehaviour
{
    [SerializeField] private Grid grid;
    private bool isMoving;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float timeToMove = 0.4f;
    private Vector3Int input;

    private void Start()
    {
        input = Vector3Int.zero;
    }

    private void Update()
    {
        if (isMoving)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        input.x = Mathf.RoundToInt(moveX);
        input.y = Mathf.RoundToInt(moveY);

        if (input.x != 0 || input.y != 0)
        {
            StartCoroutine(MovePlayer(input));
        }
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        isMoving = true;

        float elapsedTime = 0f;

        Vector3Int gridPosition = grid.WorldToCell(transform.position);

        startPosition = transform.position;
        targetPosition = grid.CellToWorld(gridPosition + direction);

        //TODO: Fix diagonal movement
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        isMoving = false;
    }
}

UNITY

Select „Player“, add „Movement controller“ script & assign „grid“ field

Select „sprites“ folder, create new folder & name it „SpriteLibraries“
Create > 2d > Sprite library asset
Name it „base“ & press the „open in sprite Library Editor“
Add a category & name it „base“
Drag & drop „player-base“ into the window
(Make sure the sprites are ordered correctly!)

Select „player“
Add „Sprite Library“ & „Sprite Resolver“ components
Add „animator“ component
Create new folder & name it „Animations“
Create animationController & name it „player“
Create „idle“, „move“ & „turn“ folders

https://www.youtube.com/watch?v=4xPHqEEa-V0
Select player & fill empty fields
Window > Animation > Animation
„Create new clip…“
Add Property, right click on „sprite resolver“ & select „Add Properties“
Use „recording mode“ to create all animations
Drag & drop in matching folders

Select „scripts“ folder
Create new script & name it „animationController“
Attach it to player

CODE EDITOR

Open the “animationController“ script & replace its contents with:

using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdleAnimation(string direction)
    {
        animator.Play($"Player_Idle{direction}");
    }

    public void PlayWalkAnimation(string direction)
    {
        animator.Play($"Player_Move{direction}");
    }
}

Open the “playercontroller“ script & replace its contents with:

using System.Collections;
using UnityEngine;

public class playercontroller : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private float timeToMove = 0.4f;
    private bool isMoving;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3Int input;

    private AnimationController animator;

    private void Start()
    {
        animator = GetComponent<AnimationController>();
        input = Vector3Int.zero;
    }

    private void Update()
    {
        if (isMoving)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        input.x = Mathf.RoundToInt(moveX);
        input.y = Mathf.RoundToInt(moveY);

        if (input.x != 0 || input.y != 0)
        {
            StartCoroutine(MovePlayer(input));
        }
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        isMoving = true;

        string dir = GetDirection(direction);

        animator.PlayWalkAnimation(dir);

        float elapsedTime = 0f;

        Vector3Int gridPosition = grid.WorldToCell(transform.position);

        startPosition = transform.position;
        targetPosition = grid.CellToWorld(gridPosition + direction);

        //TODO: Fix diagonal movement
        while (elapsedTime < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        animator.PlayIdleAnimation(dir);

        isMoving = false;
    }

    private string GetDirection(Vector3Int direction)
    {
        if (direction.x == 1)
        {
            if (direction.y == 1)
            {
                return "NE";
            }
            else if (direction.y == -1)
            {
                return "SE";
            }
            else
            {
                return "E";
            }
        }
        else if (direction.x == -1)
        {
            if (direction.y == 1)
            {
                return "NW";
            }
            else if (direction.y == -1)
            {
                return "SW";
            }
            else
            {
                return "W";
            }
        }
        else
        {
            if (direction.y == 1)
            {
                return "N";
            }
            else if (direction.y == -1)
            {
                return "S";
            }
        }

        return "S";
    }
}

Open the “animationController“ script & replace its contents with:

using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdleAnimation(string direction)
    {
        animator.Play($"Player_Idle{direction}");
    }

    public void PlayTurnAnimation(string direction)
    {
        animator.Play($"Player_Turn{direction}");
    }

    public void PlayWalkAnimation(string direction)
    {
        animator.Play($"Player_Move{direction}");
    }
}

Open the “playercontroller“ script & replace its contents with:

using System.Collections;
using UnityEngine;

public class playercontroller : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private float timeToMove = 0.4f;
    [SerializeField] private float timeToTurn = 0.1f;
    private bool isMoving;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3Int input;
    private Vector3Int lastDirection;

    private AnimationController animator;

    private void Start()
    {
        animator = GetComponent<AnimationController>();
        input = Vector3Int.zero;

        lastDirection = new Vector3Int(0, -1, 0);
    }

    private void Update()
    {
        if (isMoving)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        input.x = Mathf.RoundToInt(moveX);
        input.y = Mathf.RoundToInt(moveY);

        if (input.x != 0 || input.y != 0)
        {
            StartCoroutine(MovePlayer(input));
        }
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        isMoving = true;

        string dir = GetDirection(direction);

        if (direction != lastDirection) //turn first
        {
            //https://discussions.unity.com/t/vector2-angle-how-do-i-get-if-its-cw-or-ccw/101180/5
            bool clockwise = Mathf.Sign(lastDirection.x * direction.y - lastDirection.y * direction.x) <= 0;
            int nrOfTurns = Mathf.RoundToInt(Vector3.Angle(direction, lastDirection) / 45f);

            string[] turns = GetTurns(GetDirection(lastDirection), dir, nrOfTurns, clockwise);
            
            for (int i = 0; i < nrOfTurns; i++)
            {
                animator.PlayTurnAnimation(turns[i]);
                yield return new WaitForSeconds(timeToTurn);
            }
        }
        else
        {
            animator.PlayWalkAnimation(dir);

            float elapsedTime = 0f;

            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            startPosition = transform.position;
            targetPosition = grid.CellToWorld(gridPosition + direction);

            //TODO: Fix diagonal movement
            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
        }

        lastDirection = direction;
        animator.PlayIdleAnimation(dir);

        isMoving = false;
    }

    private string GetDirection(Vector3Int direction)
    {
        if (direction.x == 1)
        {
            if (direction.y == 1)
            {
                return "NE";
            }
            else if (direction.y == -1)
            {
                return "SE";
            }
            else
            {
                return "E";
            }
        }
        else if (direction.x == -1)
        {
            if (direction.y == 1)
            {
                return "NW";
            }
            else if (direction.y == -1)
            {
                return "SW";
            }
            else
            {
                return "W";
            }
        }
        else
        {
            if (direction.y == 1)
            {
                return "N";
            }
            else if (direction.y == -1)
            {
                return "S";
            }
        }

        return "S";
    }

    private string[] GetTurns(string start, string target, int nrOfTurns, bool clockwise)
    {
        string[] turns = new string[nrOfTurns];

        string from = start;
        string to = "";

        for (int i = 0; i < nrOfTurns; i++)
        {
            switch (from)
            {
                case "S":
                    if (clockwise)
                    {
                        to = "SW";
                    }
                    else
                    {
                        to = "SE";
                    }

                    break;
                case "SW":
                    if (clockwise)
                    {
                        to = "W";
                    }
                    else
                    {
                        to = "S";
                    }

                    break;
                case "W":
                    if (clockwise)
                    {
                        to = "NW";
                    }
                    else
                    {
                        to = "SW";
                    }

                    break;
                case "NW":
                    if (clockwise)
                    {
                        to = "N";
                    }
                    else
                    {
                        to = "W";
                    }

                    break;
                case "N":
                    if (clockwise)
                    {
                        to = "NE";
                    }
                    else
                    {
                        to = "NW";
                    }

                    break;
                case "NE":
                    if (clockwise)
                    {
                        to = "E";
                    }
                    else
                    {
                        to = "N";
                    }

                    break;
                case "E":
                    if (clockwise)
                    {
                        to = "SE";
                    }
                    else
                    {
                        to = "NE";
                    }

                    break;
                case "SE":
                    if (clockwise)
                    {
                        to = "S";
                    }
                    else
                    {
                        to = "E";
                    }

                    break;
            }

            turns[i] = clockwise ? from + to : to + from;
            from = to;
        }

        return turns;
    }
}

UNITY

Select „scripts“ folder
Create new c# script & name it „cameracontroller“
Select „Main Camera“ & attach new script to it

CODE EDITOR

Open the “cameracontroller“ script & replace its contents with:

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }
}

UNITY

Select „main camera“ & assign target

CODE EDITOR

Open the “playercontroller“ script & replace its contents with:

using System.Collections;
using UnityEngine;

public class playercontroller : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private float timeToMove = 0.4f;
    [SerializeField] private float timeToTurn = 0.1f;
    private bool isMoving;
    private bool isButtonDown;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3Int input;
    private Vector3Int lastDirection;

    private AnimationController animator;

    private void Start()
    {
        animator = GetComponent<AnimationController>();
        input = Vector3Int.zero;

        lastDirection = new Vector3Int(0, -1, 0);
        animator.PlayIdleAnimation(GetDirection(lastDirection));
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isButtonDown = false;
        }

        if (isMoving)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        input.x = Mathf.RoundToInt(moveX);
        input.y = Mathf.RoundToInt(moveY);

        if ((input.x != 0 || input.y != 0) && !isButtonDown)
        {
            StartCoroutine(MovePlayer(input));
        }
        else if (Input.GetMouseButtonDown(0))
        {
            isButtonDown = true;
        }
        else if (isButtonDown)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Vector3 direction = (mousePosition - transform.position).normalized;

            if (direction.x != 0f || moveY != 0f)
            {
                input.x = Mathf.RoundToInt(direction.x);
                input.y = Mathf.RoundToInt(direction.y);

                StartCoroutine(MovePlayer(input));
            }
        }
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        isMoving = true;

        string dir = GetDirection(direction);

        if (direction != lastDirection) //turn first
        {
            //https://discussions.unity.com/t/vector2-angle-how-do-i-get-if-its-cw-or-ccw/101180/5
            bool clockwise = Mathf.Sign(lastDirection.x * direction.y - lastDirection.y * direction.x) <= 0;
            int nrOfTurns = Mathf.RoundToInt(Vector3.Angle(direction, lastDirection) / 45f);

            string[] turns = GetTurns(GetDirection(lastDirection), dir, nrOfTurns, clockwise);
            
            for (int i = 0; i < nrOfTurns; i++)
            {
                animator.PlayTurnAnimation(turns[i]);
                yield return new WaitForSeconds(timeToTurn);
            }
        }
        else
        {
            animator.PlayWalkAnimation(dir);

            float elapsedTime = 0f;

            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            startPosition = transform.position;
            targetPosition = grid.CellToWorld(gridPosition + direction);

            //TODO: Fix diagonal movement
            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
        }

        lastDirection = direction;
        animator.PlayIdleAnimation(dir);

        isMoving = false;
    }

    private string GetDirection(Vector3Int direction)
    {
        if (direction.x == 1)
        {
            if (direction.y == 1)
            {
                return "NE";
            }
            else if (direction.y == -1)
            {
                return "SE";
            }
            else
            {
                return "E";
            }
        }
        else if (direction.x == -1)
        {
            if (direction.y == 1)
            {
                return "NW";
            }
            else if (direction.y == -1)
            {
                return "SW";
            }
            else
            {
                return "W";
            }
        }
        else
        {
            if (direction.y == 1)
            {
                return "N";
            }
            else if (direction.y == -1)
            {
                return "S";
            }
        }

        return "S";
    }

    private string[] GetTurns(string start, string target, int nrOfTurns, bool clockwise)
    {
        string[] turns = new string[nrOfTurns];

        string from = start;
        string to = "";

        for (int i = 0; i < nrOfTurns; i++)
        {
            switch (from)
            {
                case "S":
                    if (clockwise)
                    {
                        to = "SW";
                    }
                    else
                    {
                        to = "SE";
                    }

                    break;
                case "SW":
                    if (clockwise)
                    {
                        to = "W";
                    }
                    else
                    {
                        to = "S";
                    }

                    break;
                case "W":
                    if (clockwise)
                    {
                        to = "NW";
                    }
                    else
                    {
                        to = "SW";
                    }

                    break;
                case "NW":
                    if (clockwise)
                    {
                        to = "N";
                    }
                    else
                    {
                        to = "W";
                    }

                    break;
                case "N":
                    if (clockwise)
                    {
                        to = "NE";
                    }
                    else
                    {
                        to = "NW";
                    }

                    break;
                case "NE":
                    if (clockwise)
                    {
                        to = "E";
                    }
                    else
                    {
                        to = "N";
                    }

                    break;
                case "E":
                    if (clockwise)
                    {
                        to = "SE";
                    }
                    else
                    {
                        to = "NE";
                    }

                    break;
                case "SE":
                    if (clockwise)
                    {
                        to = "S";
                    }
                    else
                    {
                        to = "E";
                    }

                    break;
            }

            turns[i] = clockwise ? from + to : to + from;
            from = to;
        }

        return turns;
    }
}

UNITY

Select „scenes“ folder
Rename „SampleScene“ to „GameScene“
Create new scene & name it „MenuScene“
Open it (double click)

File > Build Settings…
„Add open scenes“
Close window

UI > button - textmeshpro
Select „canvas“
Change „ui scale mode“ from „constant pixel size“ to „scale with screen size“
Change „refrence resolution“ to (x: 1280, y: 720)
Select „button“
Position to center

Select „scripts“ folder
Create new c# script & name it „Menumanager“
Attach it to „canvas“ object

CODE EDITOR

Open the “menu manager“ script & replace its contents with:

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}

UNITY

Select „button“ & add MenuManager.Playgame() to „onclick()“

Window > Package Manager
Add „localization“ package
Close window

Edit > Project settings
Select „localization“
Press „create“
Create „localization“ folder to save newly created file
Press „add locale“
Select locales & press „add locales“
Choose „localization“ folder in pop up
Set „project locale identifier“

Window > Asset management > localization tables
Press „new table collection“
Name it „strings“ & save it in „localization“ folder
Press „new entry“
Rename key to „save“ & fill out translations
Repeat for key „playGame“
Close window

OPTIONAL (Fix locale selection for this unity version)

Select „scripts“ folder
Create new c# script & name it „CustomLocaleSelector“

CODE EDITOR

Open the “CustomLocaleSelector“ script & replace its contents with:

using System;

namespace UnityEngine.Localization.Settings
{
    [Serializable]
    public class CustomLocaleSelector : IStartupLocaleSelector
    {
        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                if (LocalizationSettings.AvailableLocales.Locales[i].ToString().Split(' ')[0] == Application.systemLanguage.ToString())
                {
                    return LocalizationSettings.AvailableLocales.Locales[i];
                }
            }
            
            // No locale could be found.
            return null;
        }
    }
}

UNITY

Select „scripts“ folder
Edit > Project Settings…
Select „Localization“
Add „customlocalselector“ to „locale selectors“
Place it in 2nd place

Select „Text (TMP)“ (child of button)
Add „localize string event“ component
Set „string reference“ to „playGame“
Hit plus in „update string (string)“ section
Drag & drop  „Text (TMP)“
Textmeshprogui > text (on top)

Open „gameScene“
UI > button - textmeshpro
Select „canvas“
Change „ui scale mode“ from „constant pixel size“ to „scale with screen size“
Change „refrence resolution“ to (x: 1280, y: 720)
Select „button“
Position to top left
Duplicae „button“ twice & position them nicely
Select each „Text (TMP)“ (child of button) & change text to „clear“, „save“ & „load“

Select „scripts“ folder
Create new c# script & name it „savedata“
Create new c# script & name it „savemanager“

CODE EDITOR

Open the “savemanager“ script & replace its contents with:

using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static void SaveData(SaveData saveData)
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/testLevel.json", json);
    }

    public static SaveData LoadData()
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/testLevel.json");
        return JsonUtility.FromJson<SaveData>(json);
    }
}

Open the “savedata“ script & replace its contents with:

using System.Collections.Generic;

public class SaveData
{
    public List<SavedTile> layer0;
    public List<SavedTile> layer1;
    public List<SavedTile> layer2;
    public List<SavedTile> layer3;
    public List<SavedTile> layer4;
    public List<SavedTile> layer5;
}

Open the “Worldmanager“ script & replace its contents with:

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private SeasonalRuleTile grassTiles;
    [SerializeField] private SeasonalRuleTile cliffTiles;
    [SerializeField] private SeasonalRuleTile waterTiles;

    private Tilemap[] tilemaps;

    private void Start()
    {
        tilemaps = GetComponentsInChildren<Tilemap>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.N))
        {
            switch (grassTiles.season)
            {
                case Seasons.Spring:
                    grassTiles.season = Seasons.Summer;
                    break;
                case Seasons.Summer:
                    grassTiles.season = Seasons.Autumn;
                    break;
                case Seasons.Autumn:
                    grassTiles.season = Seasons.Winter;
                    break;
                case Seasons.Winter:
                    grassTiles.season = Seasons.Spring;
                    break;
            }

            cliffTiles.season = grassTiles.season;
            waterTiles.season = grassTiles.season;

            foreach (Tilemap map in tilemaps)
            {
                map.RefreshAllTiles();
            }
        }
    }

    public void SaveMap()
    {
        SaveData saveData = new SaveData();

        for (int i = 0; i < tilemaps.Length; i++)
        {
            List<SavedTile> tiles = new List<SavedTile>();
            BoundsInt bounds = tilemaps[i].cellBounds;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    SavedTile savedTile = new SavedTile();
                    SeasonalRuleTile ruleTile = tilemaps[i].GetTile<SeasonalRuleTile>(new Vector3Int(x, y, 0));
                    
                    if (ruleTile != null)
                    {
                        savedTile.position = new Vector3Int(x, y, 0);
                        savedTile.tileType = ruleTile.tileType;

                        tiles.Add(savedTile);
                    }
                }
            }

            switch (i)
            {
                case 0:
                    saveData.layer0 = tiles;
                    break;
                case 1:
                    saveData.layer1 = tiles;
                    break;
                case 2:
                    saveData.layer2 = tiles;
                    break;
                case 3:
                    saveData.layer3 = tiles;
                    break;
                case 4:
                    saveData.layer4 = tiles;
                    break;
                case 5:
                    saveData.layer5 = tiles;
                    break;
            }
        }

        SaveManager.SaveData(saveData);
    }

    public void ClearMap()
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.ClearAllTiles();
        }
    }

    public void LoadMap()
    {
        SaveData saveData = SaveManager.LoadData();

        for (int i = 0; i < tilemaps.Length; i++)
        {
            List<SavedTile> savedTiles = new List<SavedTile>();

            switch (i)
            {
                case 0:
                    savedTiles = saveData.layer0;

                    break;
                case 1:
                    savedTiles = saveData.layer1;

                    break;
                case 2:
                    savedTiles = saveData.layer2;

                    break;
                case 3:
                    savedTiles = saveData.layer3;

                    break;
                case 4:
                    savedTiles = saveData.layer4;

                    break;
                case 5:
                    savedTiles = saveData.layer5;

                    break;
            }

            for (int j = 0; j < savedTiles.Count; j++)
            {
                switch (savedTiles[j].tileType)
                {
                    case TileType.Water:
                        tilemaps[i].SetTile(savedTiles[j].position, waterTiles);
                        break;
                    case TileType.Cliff:
                        tilemaps[i].SetTile(savedTiles[j].position, cliffTiles);
                        break;
                    default:
                        tilemaps[i].SetTile(savedTiles[j].position, grassTiles);
                        break;
                }
            }
        }
    }
}

UNITY

Select each „button“ & add the matching functions from the „world manager“ to „onclick()“

(Click on 3 dots on tile map to compress)

UNITY

Select „button“ & add MenuManager.Playgame() to „onclick()“

https://www.youtube.com/watch?v=mZfyt03LDH4&list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW&index=3

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NodeGrid : MonoBehaviour
{
    [SerializeField] private Grid grid;

    public Node[,] nodes;
    private Vector2Int gridSize;
    private Vector3 bottomLeft;
    private Tilemap[] tilemaps;

    public List<Node> path;

    private void Awake()
    {
        gridSize = Vector2Int.zero;
        tilemaps = new Tilemap[grid.transform.childCount];

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            tilemaps[i] = grid.transform.GetChild(i).GetComponent<Tilemap>();

            if (tilemaps[i].cellBounds.size.x > gridSize.x)
            {
                gridSize.x = tilemaps[i].cellBounds.size.x;
            }

            if (tilemaps[i].cellBounds.size.y > gridSize.y)
            {
                gridSize.y = tilemaps[i].cellBounds.size.y;
            }
        }

        bottomLeft = new Vector3(-gridSize.x / 2f * grid.cellSize.x + grid.cellSize.x, -gridSize.y / 2f * grid.cellSize.y, 0f);

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        nodes = new Node[gridSize.x, gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int i = 0; i < tilemaps.Length; i++) //bottom up
                {
                    SeasonalRuleTile tile = tilemaps[i].GetTile<SeasonalRuleTile>(new Vector3Int(x - gridSize.x / 2, y - gridSize.y / 2, 0));
                    if (tile != null)
                    {
                        nodes[x,y] = new Node(tile.tileType == TileType.Grass, bottomLeft + new Vector3(x * grid.cellSize.x, y * grid.cellSize.y, 0), x, y);
                        break;
                    }
                }
            }
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemaps[0].WorldToCell(worldPosition);
        int checkX = cellPosition.x + gridSize.x / 2;
        int checkY = cellPosition.y + gridSize.y / 2;

        if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
        {
            return nodes[checkX,checkY];
        }

        return null;
    }

    public List<Node> GetNeighbors(Node centerNode)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 || y != 0)
                {
                    int checkX = centerNode.gridX + x;
                    int checkY = centerNode.gridY + y;

                    if (checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y)
                    {
                        if ((x + y)%2 == 0) //is diagonal
                        {
                            if (!nodes[checkX, centerNode.gridY].isWalkable || !nodes[centerNode.gridX, checkY].isWalkable)
                            {
                                continue;
                            }
                        }

                        neighbors.Add(nodes[checkX, checkY]);
                    }
                }
            }
        }

        return neighbors;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count; i++)
            {
                Gizmos.DrawCube(path[i].worldPosition, Vector3.one * 0.25f);
            }
        }
    }
}



Wave function collapse
Hand crafted tiles