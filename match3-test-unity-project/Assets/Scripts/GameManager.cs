using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI move_text, level_text, time_text, score_text;
    public bool can_move, is_pressing_tile = false, has_started = false, is_paused = false;
    public float time_remaining = 120;
    public int level = 1, score = 0, score_goal = 60;
    public Vector2 click_position, mouse_position, result_position;
    public GameObject pieces_pivot;
    public Camera cam;
    public Tile[] tiles = new Tile[7];
    [SerializeField]
    public List<BoardTile> board_tiles = new List<BoardTile>();

    RaycastHit2D hit_;
    // Start is called before the first frame update
    void Awake()
    {

        instance = this;

    }

    // Update is called once per frame
    void Update()
    {
        if (float.Parse(Screen.width.ToString()) / float.Parse(Screen.height.ToString()) < (2f / 3f))
        {
            // 0 = 17,2
            // 0.5 = 10;
            // 0.666 = 7.6

            // 9.6
            
            cam.orthographicSize = (1 - (float.Parse(Screen.width.ToString()) / float.Parse(Screen.height.ToString()) / (2f / 3f))) * 9.6f + 7.6f;

        }
        else
        {
            cam.orthographicSize = 7.6f;

        }

        if (has_started)
        {

            if (!is_paused)
            {
                if (time_remaining > 0)
                {

                    time_remaining -= Time.deltaTime;

                    level_text.text = "Level " + level;

                    time_text.text = Mathf.RoundToInt(time_remaining).ToString() + (Mathf.RoundToInt(time_remaining) > 1 ? " secs" : " sec");
                    score_text.text = score + "/" + score_goal;

                    if (score >= score_goal && can_move)
                    {

                        shuffleBoard();

                        level++;

                        score = 0;
                        score_goal += 40;

                        time_remaining = 120;

                        popGameObjetc(level_text.gameObject);
                        popGameObjetc(time_text.gameObject);
                        popGameObjetc(score_text.gameObject);

                    }


                }
                else
                {

                    time_text.text = "---";


                    MenuManager.instance.openTimeOutMenu();

                    can_move = false;


                }

            }

            if (can_move)
            {

                move_text.text = "MOVE!";

            }
            else
            {

                move_text.text = "...";

            }


            if (!canva.instance.IsMouseOverUI())
            {

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {

                    if (can_move)
                    {

                        hit_ = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                        if (hit_.transform != null)
                        {

                            if (hit_.transform.CompareTag("tile"))
                            {

                                is_pressing_tile = true;

                                Debug.Log(hit_);
                                Debug.Log("--- " + hit_.transform.name);

                                click_position = Input.mousePosition;

                            }




                        }
                        else
                        {

                            Debug.Log("--- nothing selected");

                        }

                    }
                    else
                    {

                        Debug.Log("- CAN'T MOVE!");

                    }

                }

            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {

                if (is_pressing_tile)
                {

                    mouse_position = Input.mousePosition;

                    result_position = mouse_position - click_position;



                    if (Vector2.Distance(click_position, mouse_position) > 20)
                    {

                        result_position = mouse_position - click_position;

                        Debug.Log("- ABS : " + Mathf.Abs(result_position.x) + "," + Mathf.Abs(result_position.y));

                        if (Mathf.Abs(result_position.x) > Mathf.Abs(result_position.y))
                        {

                            if (result_position.x > 0)
                            {

                                Debug.Log("- RIGHT swipe!");
                                StartCoroutine(makeMove(hit_.transform.GetComponent<TileInfos>().board_tile, Vector2.right));

                            }
                            else
                            {

                                Debug.Log("- LEFT swipe!");
                                StartCoroutine(makeMove(hit_.transform.GetComponent<TileInfos>().board_tile, Vector2.left));

                            }

                        }
                        else
                        {


                            if (result_position.y > 0)
                            {

                                Debug.Log("- UP swipe!");
                                StartCoroutine(makeMove(hit_.transform.GetComponent<TileInfos>().board_tile, Vector2.up));

                            }
                            else
                            {

                                Debug.Log("- DOWN swipe!");
                                StartCoroutine(makeMove(hit_.transform.GetComponent<TileInfos>().board_tile, Vector2.down));

                            }

                        }

                    }

                }

                is_pressing_tile = false;

            }





            if (Input.GetKeyDown(KeyCode.W))
            {

                StartCoroutine(dealWithState());

            }
            if (Input.GetKeyDown(KeyCode.E))
            {

                List<int> list_ = spacesInTop();

                if (list_ != null)
                {

                    Debug.Log("spaces");

                    instantiateTopTiles(list_);

                }
                else
                {

                    Debug.Log("no spaces");

                }

            }

            if (Input.GetKeyDown(KeyCode.R))
            {

                shuffleBoard();

            }

            if (Input.GetKeyDown(KeyCode.S))
            {

                foreach (BoardTile tile_ in board_tiles)
                {

                    tile_.tile_obj.transform.LeanMove(new Vector3(pieces_pivot.transform.position.x + tile_.pos_x, pieces_pivot.transform.position.y + tile_.pos_y), 1f);

                }

            }
        }
    }

    public void startGame()
    {
        StopAllCoroutines();

        has_started = true;
        is_paused = false;

        score = 0;
        time_remaining = 120;
        score_goal = 60;

        StartCoroutine(dealWithState());
        AudioManager.instance.playAudio("theme");

    }

    public void restartGame()
    {
        StopAllCoroutines();

        foreach (BoardTile tile_ in board_tiles)
        {

            Destroy(tile_.tile_obj);

        }

        board_tiles = new List<BoardTile>();
        AudioManager.instance.stopAudio("theme");

        has_started = true;
        is_paused = false;

        score = 0;
        time_remaining = 120;
        score_goal = 60;

        StartCoroutine(dealWithState());
        AudioManager.instance.playAudio("theme");

    }

    public void leaveGame()
    {
        StopAllCoroutines();

        has_started = false;
        is_paused = false;

        foreach (BoardTile tile_ in board_tiles)
        {

            Destroy(tile_.tile_obj);

        }

        board_tiles = new List<BoardTile>();
        AudioManager.instance.stopAudio("theme");

    }


    BoardTile getTileByPosition(int pos_x_, int pos_y_, List<BoardTile> board_tiles_)
    {

        BoardTile tile_ = null;

        foreach (BoardTile t_ in board_tiles_)
        {

            if (t_.pos_x == pos_x_ && t_.pos_y == pos_y_)
            {

                tile_ = t_;

            }

        }

        return tile_;

    }

    bool needBeDestroyed(BoardTile tile_, List<BoardTile> board_tiles_)
    {

        bool need = false;

        for (int i_ = -2; i_ < 1; i_++)
        {

            bool has_unlike_in_x = false;

            for (int x_ = i_; x_ < i_ + 3; x_++)
            {

                BoardTile tile_2_ = getTileByPosition(tile_.pos_x + x_, tile_.pos_y, board_tiles_);

                if (tile_2_ != null)
                {

                    if (tile_2_.tile_id != tile_.tile_id)
                    {

                        has_unlike_in_x = true;

                    }

                }
                else
                {

                    has_unlike_in_x = true;

                }

            }

            bool has_unlike_in_y = false;

            for (int y_ = i_; y_ < i_ + 3; y_++)
            {

                BoardTile tile_2_ = getTileByPosition(tile_.pos_x, tile_.pos_y + y_, board_tiles_);

                if (tile_2_ != null)
                {

                    if (tile_2_.tile_id != tile_.tile_id)
                    {

                        has_unlike_in_y = true;

                    }

                }
                else
                {

                    has_unlike_in_y = true;

                }

            }

            if (!has_unlike_in_x || !has_unlike_in_y)
            {

                need = true;

            }

        }


        return need;

    }

    List<int> spacesInTop()
    {

        List<int> spaces = new List<int>();

        for (int i_ = 0; i_ < 10; i_++)
        {

            if (getTileByPosition(i_, 9, board_tiles) == null)
            {

                spaces.Add(i_);

            }

        }

        if (spaces.Count == 0)
        {

            Debug.Log("nullll");

            spaces = null;

        }

        return spaces;

    }

    void instantiateTopTiles(List<int> list_)
    {

        foreach (int i_ in list_)
        {

            int random_index = Random.Range(0, tiles.Length);

            GameObject inst_ = Instantiate(tiles[random_index].prefab, pieces_pivot.transform.position + new Vector3(i_, 9), new Quaternion(), pieces_pivot.transform);

            BoardTile current_board_tile = new BoardTile();

            current_board_tile.tile_id = random_index;
            current_board_tile.tile_obj = inst_;
            current_board_tile.pos_x = i_;
            current_board_tile.pos_y = 9;

            TileInfos tile_info = inst_.AddComponent<TileInfos>();

            board_tiles.Add(current_board_tile);

            tile_info.board_tile = current_board_tile;
            tile_info.tile_id = random_index;

            inst_.transform.localScale = Vector3.zero;

            inst_.transform.LeanScale(Vector3.one, 0.15f).setEaseOutElastic();

        }

    }

    void popGameObjetc(GameObject obj_)
    {

        StartCoroutine(pop(obj_));

    }

    IEnumerator pop(GameObject obj_)
    {

        obj_.transform.LeanScale(Vector3.one * 1.5f, 0.1f).setEaseOutCirc();

        yield return new WaitForSecondsRealtime(0.1f);

        obj_.transform.LeanScale(Vector3.one, 0.4f).setEaseOutCirc();

    }



    List<Vector2> spacesToMoveDown()
    {

        List<Vector2> spaces_ = new List<Vector2>();

        foreach (BoardTile t_ in board_tiles)
        {

            if (t_.pos_y != 0)
            {

                if (getTileByPosition(t_.pos_x, t_.pos_y - 1, board_tiles) == null)
                {

                    spaces_.Add(new Vector2(t_.pos_x, t_.pos_y - 1));

                }

            }

        }

        List<Vector2> spaces_to_remove_ = new List<Vector2>();

        foreach (Vector2 space_1_ in spaces_)
        {

            foreach (Vector2 space_2_ in spaces_)
            {

                if (space_1_ != space_2_)
                {

                    if (space_1_.x == space_2_.x)
                    {

                        if (space_1_.y > space_2_.y)
                        {

                            spaces_to_remove_.Add(space_1_);

                        }
                        else
                        {

                            spaces_to_remove_.Add(space_2_);

                        }

                    }

                }

            }

        }

        foreach (Vector2 space_ in spaces_to_remove_)
        {

            spaces_.Remove(space_);

        }

        if (spaces_.Count == 0)
        {

            spaces_ = null;

        }

        return spaces_;

    }

    void moveDownTilesBySpaces(List<Vector2> spaces_)
    {

        foreach (Vector2 space_ in spaces_)
        {

            foreach (BoardTile t_ in board_tiles)
            {

                if (t_.pos_x == space_.x && t_.pos_y > space_.y)
                {

                    t_.tile_obj.transform.LeanMove(t_.tile_obj.transform.position + new Vector3(0, -1), 0.1f).setEaseOutElastic();
                    t_.pos_y -= 1;

                }

            }

        }

    }

    BoardTile switchTiles(BoardTile tile_, Vector2 to_position_, List<BoardTile> board_tiles_)
    {

        BoardTile tile_2_ = getTileByPosition(Mathf.RoundToInt(tile_.pos_x + to_position_.x), Mathf.RoundToInt(tile_.pos_y + to_position_.y), board_tiles_);

        if (tile_2_ != null)
        {

            tile_.pos_x += Mathf.RoundToInt(to_position_.x);
            tile_.pos_y += Mathf.RoundToInt(to_position_.y);


            tile_2_.pos_x -= Mathf.RoundToInt(to_position_.x);
            tile_2_.pos_y -= Mathf.RoundToInt(to_position_.y);

        }

        return tile_2_;
    }

    bool hasMovings()
    {

        bool has_ = false;

        Vector2[] possible_moves_ = new Vector2[4] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (BoardTile tile_ in board_tiles)
        {

            foreach (Vector2 move_ in possible_moves_)
            {

                List<BoardTile> temp_list_ = new List<BoardTile>();

                foreach (BoardTile t_ in board_tiles)
                {

                    temp_list_.Add(t_.DeepClone());

                }


                BoardTile temp_tile_ = getTileByPosition(tile_.pos_x, tile_.pos_y, temp_list_);

                switchTiles(temp_tile_, move_, temp_list_);

                foreach (BoardTile temp_tile_2_ in temp_list_)
                {

                    if (needBeDestroyed(temp_tile_2_, temp_list_))
                    {

                        has_ = true;

                        return true;

                    }

                }

            }

        }

        Debug.Log("- has = " + has_);


        return has_;

    }

    void shuffleBoard()
    {

        foreach (BoardTile tile_ in board_tiles)
        {

            tile_.tile_obj.transform.LeanScale(Vector3.zero, 0.25f);

            Destroy(tile_.tile_obj, 0.25f);

        }

        board_tiles = new List<BoardTile>();

        StartCoroutine(dealWithState());

    }

    IEnumerator makeMove(BoardTile tile_, Vector2 to_direction_)
    {
        BoardTile tile_2_ = switchTiles(tile_, to_direction_, board_tiles);

        if (tile_2_ != null)
        {

            can_move = false;

            tile_.tile_obj.transform.LeanMove(tile_.tile_obj.transform.position + new Vector3(to_direction_.x, to_direction_.y), 0.25f).setEaseOutElastic();
            tile_2_.tile_obj.transform.LeanMove(tile_2_.tile_obj.transform.position - new Vector3(to_direction_.x, to_direction_.y), 0.25f).setEaseOutElastic();

            AudioManager.instance.playAudio("swap");

            //Debug.Log("WAITING FOR swipe");
            yield return new WaitForSecondsRealtime(0.25f);

            bool need_revert = true;

            foreach (BoardTile t_ in board_tiles)
            {

                if (needBeDestroyed(t_, board_tiles))
                {

                    need_revert = false;

                }

            }

            if (need_revert)
            {

                switchTiles(tile_, to_direction_ * -1, board_tiles);

                tile_.tile_obj.transform.LeanMove(tile_.tile_obj.transform.position + (new Vector3(to_direction_.x, to_direction_.y) * -1), 0.25f).setEaseOutElastic();
                tile_2_.tile_obj.transform.LeanMove(tile_2_.tile_obj.transform.position - (new Vector3(to_direction_.x, to_direction_.y) * -1), 0.25f).setEaseOutElastic();

                AudioManager.instance.playAudio("swap");

                //Debug.Log("WAITING FOR revert");
                yield return new WaitForSecondsRealtime(0.25f);

                can_move = true;


            }
            else
            {

                StartCoroutine(dealWithState());

            }

        }

        yield return null;

    }

    IEnumerator dealWithState()
    {

        can_move = false;

        bool is_all_done = false, is_moving_done = false, is_matching_done = false;

        while (!is_all_done)
        {

            while (!is_moving_done)
            {
                bool no_top_spaces = false, no_spaces_to_move = false;

                List<int> to_spaces_list_ = spacesInTop();

                if (to_spaces_list_ != null)
                {

                    instantiateTopTiles(to_spaces_list_);

                    AudioManager.instance.playAudio("swap");


                    //Debug.Log("WAITING FOR instantiateTopTiles" + to_spaces_list_.Count);
                    yield return new WaitForSecondsRealtime(0.15f);
                    no_top_spaces = false;

                }
                else
                {

                    no_top_spaces = true;

                }

                List<Vector2> move_list_ = spacesToMoveDown();

                if (move_list_ != null)
                {

                    moveDownTilesBySpaces(move_list_);

                    //Debug.Log("WAITING FOR moveDownTilesBySpaces");
                    yield return new WaitForSecondsRealtime(0.1f);
                    no_spaces_to_move = false;

                }
                else
                {

                    no_spaces_to_move = true;

                }

                if (no_top_spaces && no_spaces_to_move)
                {

                    is_moving_done = true;

                }

            }


            List<Vector2> pos_list_ = new List<Vector2>();

            foreach (BoardTile tile_ in board_tiles)
            {

                if (needBeDestroyed(tile_, board_tiles))
                {

                    pos_list_.Add(new Vector2(tile_.pos_x, tile_.pos_y));

                }

            }

            foreach (Vector2 pos_ in pos_list_)
            {

                BoardTile tile_ = getTileByPosition(int.Parse(pos_.x.ToString()), int.Parse(pos_.y.ToString()), board_tiles);

                tile_.tile_obj.transform.LeanScale(Vector3.zero, 1f).setEaseOutQuart();

                Destroy(tile_.tile_obj, 1f);

                board_tiles.Remove(tile_);

            }

            Debug.Log("+" + pos_list_.Count);
            score += pos_list_.Count;

            if (pos_list_.Count > 0)
            {

                popGameObjetc(score_text.gameObject);

            }

            if (pos_list_.Count > 0)
            {

                //Debug.Log("WAITING FOR destroying");

                //playClearSound(pos_list_.Count);

                AudioManager.instance.playAudio("clear");

                yield return new WaitForSecondsRealtime(1f);


            }


            if (pos_list_.Count > 0)
            {

                is_moving_done = false;
                is_matching_done = false;

            }
            else
            {

                is_matching_done = true;

            }



            if (is_moving_done && is_moving_done)
            {

                is_all_done = true;

            }

        }

        Debug.Log("- ...");

        if (hasMovings())
        {

            can_move = true;

        }
        else
        {

            shuffleBoard();

        }


    }


}

[System.Serializable]
public class BoardTile
{

    public int tile_id;
    public int pos_x;
    public int pos_y;

    public GameObject tile_obj;

}

public static class ClassExtensions
{
    public static T DeepClone<T>(this T source) where T : class
    {
        T clone = JsonUtility.FromJson<T>(JsonUtility.ToJson(source));

        return clone;
    }
}