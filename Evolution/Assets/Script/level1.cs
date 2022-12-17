using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using static System.Math;
using Random=UnityEngine.Random;
using UnityEngine.SceneManagement;

enum TileType
{
    TREES = 0,
    FLOOR = 1,
    WATER = 2,
    HERBS = 3,
    BEARS = 4,
    MOUNT = 5,
}
// I have taken the Level.cs and tried to replicate it here 
public class level1 : MonoBehaviour
{
    public int width = 16;   // size of level (default 16 x 16 blocks)
    public int length = 16;
    public float tree_height = 2.5f;   // height of trees
    public float bear_speed = 3.0f;     // bear velocity
    public GameObject kk_prefab;        //player prefab//king_kong prefab
    public GameObject bear_prefab;     //virus prefab// bear or we can do animals (make an array of animals and randomize it)prefab
    public GameObject water_prefab;    // 
    public GameObject cave_prefab;     //house prefab
    public GameObject text_box;
    public GameObject scroll_bar;
    private AudioSource source;
    public Canvas new_screen;
    public Material grass;

    // fields/variables accessible from other scripts
    internal GameObject fps_player_obj;   // instance of FPS template
    internal float player_health = 1.0f;  // player health in range [0.0, 1.0]
    // internal int num_virus_hit_concurrently = 0;            // how many viruses hit the player before washing them off
    // internal bool virus_landed_on_player_recently = false;  // has virus hit the player? if yes, a timer of 5sec starts before infection
    // internal float timestamp_virus_landed = float.MaxValue; // timestamp to check how many sec passed since the virus landed on player
    //
    internal bool herb_landed_on_player_recently = false;   // has herb(drug) collided with player?
    internal bool player_is_on_water = false;               // is player on water block
    internal bool player_entered_cave = false;             // has player arrived in cave(house)?
    // Start is called before the first frame update

     // fields/variables needed only from this script
    private Bounds bounds;                   // size of ground plane in world space coordinates 
    private float timestamp_last_msg = 0.0f; // timestamp used to record when last message on GUI happened (after 7 sec, default msg appears)
    private int function_calls = 0;          // number of function calls during backtracking for solving the CSP
    private int num_bears = 0;             // number of viruses in the level
    private List<int[]> pos_bear;         // stores their location in the grid(bear == virus)
    // a helper function that randomly shuffles the elements of a list (useful to randomize the solution to the CSP)
    private void Shuffle<T>(ref List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


    void Start()
    {
        bounds = GetComponent<Collider>().bounds; 
        timestamp_last_msg = 0.0f;
        function_calls = 0;
        num_bears = 0;
        player_health = 1.0f;
        // num_virus_hit_concurrently = 0;
        // virus_landed_on_player_recently = false;
        // timestamp_virus_landed = float.MaxValue;
        herb_landed_on_player_recently = false;
        player_is_on_water = false;
        player_entered_cave = false;

        // initialize 2D grid
        List<TileType>[,] grid = new List<TileType>[width, length];

        // useful to keep variables that are unassigned so far
        List<int[]> unassigned = new List<int[]>();
        
        num_bears = width * length / 25 + 1;
        pos_bear = new List<int[]>();

        bool success = false;
        while (!success){
            for (int v = 0; v < num_bears; v++)
            {
                while (true) // try until virus placement is successful (unlikely that there will no places)
                {
                    // try a random location in the grid
                    int wr = Random.Range(1, width - 1);
                    int lr = Random.Range(1, length - 1);

                    // if grid location is empty/free, place it there
                    if (grid[wr, lr] == null)
                    {
                        grid[wr, lr] = new List<TileType> { TileType.BEARS };
                        pos_bear.Add(new int[2] { wr, lr });
                        break;
                    }
                }
            }
            for (int w = 0; w < width; w++)
                for (int l = 0; l < length; l++)
                    if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                        grid[w, l] = new List<TileType> { TileType.MOUNT };
                    else{
                        if (grid[w, l] == null){ // does not have virus already or some other assignment from previous run
                            // CSP will involve assigning variables to one of the following four values (VIRUS is predefined for some tiles)
                            List<TileType> candidate_assignments = new List<TileType> { TileType.TREES, TileType.FLOOR, TileType.WATER, TileType.HERBS, TileType.MOUNT};
                            Shuffle<TileType>(ref candidate_assignments);

                            grid[w, l] = candidate_assignments;
                            unassigned.Add(new int[] { w, l });
                        }
                    }
                success = BackTrackingSearch(grid, unassigned);
                if(!success){
                Debug.Log("Could not find valid solution - will try again");
                unassigned.Clear();
                grid = new List<TileType>[width, length];
                function_calls = 0;
                }
        }
        DrawDungeon(grid);
    }

    bool TooMany(List<TileType>[,] grid){
        int[] number_of_assigned_elements = new int[] { 0, 0, 0, 0, 0, 0};
        for (int w = 0; w < width; w++)
            for (int l = 0; l < length; l++) 
            {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    continue;
                if (grid[w, l].Count == 1)
                    number_of_assigned_elements[(int)grid[w, l][0]]++;
            }

        if ((number_of_assigned_elements[(int)TileType.TREES] > num_bears * 10) ||
             (number_of_assigned_elements[(int)TileType.WATER] > (width + length) / 4) ||
             (number_of_assigned_elements[(int)TileType.HERBS] >= num_bears / 2) ||
             (number_of_assigned_elements[(int)TileType.MOUNT] > num_bears * 10))
            return true;
        else
            return false;
    }

    bool TooFew(List<TileType>[,] grid){
        int[] number_of_potential_assignments = new int[] { 0, 0, 0, 0, 0, 0 };
        for (int w = 0; w < width; w++)
            for (int l = 0; l < length; l++)
            {
                if (w == 0 || l == 0 || w == width - 1 || l == length - 1)
                    continue;
                for (int i = 0; i < grid[w, l].Count; i++)
                    number_of_potential_assignments[(int)grid[w, l][i]]++;
            }

        if ((number_of_potential_assignments[(int)TileType.TREES] < (width * length) / 4) ||
             (number_of_potential_assignments[(int)TileType.WATER] < num_bears / 4) ||
             (number_of_potential_assignments[(int)TileType.HERBS] < num_bears / 4)||
             (number_of_potential_assignments[(int)TileType.MOUNT] < (width * length) / 4))
            return true;
        else
            return false;
    }

    bool CheckConsistency(List<TileType>[,] grid, int[] cell_pos, TileType t)
    {
        int w = cell_pos[0];
        int l = cell_pos[1];

        List<TileType> old_assignment = new List<TileType>();
        old_assignment.AddRange(grid[w, l]);
        grid[w, l] = new List<TileType> { t };

		// note that we negate the functions here i.e., check if we are consistent with the constraints we want
        bool areWeConsistent = !TooFew(grid) && !TooMany(grid);

        grid[w, l] = new List<TileType>();
        grid[w, l].AddRange(old_assignment);
        return areWeConsistent;
    }


    bool BackTrackingSearch(List<TileType>[,] grid, List<int[]> unassigned)
    {
        // if there are too many recursive function evaluations, then backtracking has become too slow (or constraints cannot be satisfied)
        // to provide a reasonable amount of time to start the level, we put a limit on the total number of recursive calls
        // if the number of calls exceed the limit, then it's better to try a different initialization
        if (function_calls++ > 100000)       
            return false;

        // we are done!
        if (unassigned.Count == 0)
            return true;
        /*** implement the rest ! */
            int w = unassigned[0][0];
            int l = unassigned[0][1];
            int[] temp = unassigned[0];
            unassigned.RemoveAt(0);
            for(int j = 0;j < grid[w,l].Count; j++){//I am assigning all possible values to the tile
                if(CheckConsistency(grid,temp,grid[w,l][j])){
                    TileType store = grid[w, l][0];
                    grid[w,l][0] = grid[w,l][j];
                    bool result = BackTrackingSearch(grid,unassigned);
                    if(result)
                        return result;
                    grid[w,l][0] = store;
                }
            }
            return false;
    }

    bool pathexist(List<TileType>[,] grid,int sw,int sl,int ew,int el,List<string> visited){
        
        if(visited.Contains(sw + " " + sl)){
            return false;
        }
        if((sw == ew) && (sl == el)){
            return true;
        }
        if(grid[sw, sl][0] == TileType.MOUNT){
            return false;
        }
        visited.Add(sw + " " + sl);
        return (pathexist(grid,sw + 1,sl,ew,el,visited) || pathexist(grid,sw - 1,sl,ew,el,visited)|| pathexist(grid,sw,sl+1,ew,el,visited)|| pathexist(grid,sw,sl - 1,ew,el,visited));
    }

    
    void DrawDungeon(List<TileType>[,] solution){
        // GetComponent<Renderer>().material = grass;
        GetComponent<Renderer>().material.color = Color.green;
    }
    
    
    // bool NoWallsCloseToBear(List<TileType>[,] grid)
    // {
    //     /*** implement the rest ! */
    //     foreach(int[] pos in pos_bear)
    //     {
    //         for(int i = pos[0] - 1; i <= pos[0] + 1; i++)
    //         {
    //             for(int j = pos[1] - 1; j <= pos[1] + 1; j++){
    //                 if(i < 0 || j < 0 || i > length - 1 || j > width -1)
    //                     continue;
    //                 if(grid[i,j].Count == 1 && grid[i,j][0] == (TileType.MOUNT))
    //                     return false;
    //             }
    //         }

    //     }
    //     return true;
    // }


    // Update is called once per frame
    void Update()
    {
        
        
    }
}
