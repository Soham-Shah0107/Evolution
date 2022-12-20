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
    BUSHES = 5,
    MOUNT = 6,
    
}
// I have taken the Level.cs and tried to replicate it here 
public class Stage_1 : MonoBehaviour
{
    public int width = 16;   // size of level (default 16 x 16 blocks)
    public int length = 16;
    public float tree_height = 1.5f;   // height of trees
    public float mountain_height = 0.5f;
    public float bear_speed = 2.0f;     // bear velocity
    public float fox_speed = 3.0f; 
    public float tiger_speed = 4.0f; 
    //public GameObject kk_prefab;        //player prefab//king_kong prefab
    public GameObject bear_prefab;     //virus prefab// bear or we can do animals (make an array of animals and randomize it)prefab
    public GameObject water_prefab;    // 
    public GameObject cave_prefab;     //house prefab
    public GameObject tree_prefab;
    public GameObject text_box;
    public GameObject scroll_bar;
    public GameObject bushes_prefab;
    public GameObject mountain_prefab;
    public GameObject tiger_prefab;
    public GameObject kid; 
    private AudioSource source;
    public Canvas new_screen;
    public Material grass;
    int wr;
    int lr; 
    int wee;
    int lee; 

    // fields/variables accessible from other scripts
    internal GameObject kk_obj;   // instance of FPS template
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
                            List<TileType> candidate_assignments = new List<TileType> { TileType.TREES, TileType.FLOOR, TileType.WATER, TileType.HERBS, TileType.BUSHES}; // Removed TIleType.Mount
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
        int[] number_of_assigned_elements = new int[] { 0, 0, 0, 0, 0, 0, 0};
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
             (number_of_assigned_elements[(int)TileType.MOUNT] > num_bears * 10) ||
             (number_of_assigned_elements[(int)TileType.BUSHES] > (width + length)/4))
            return true;
        else
            return false;
    }

    bool TooFew(List<TileType>[,] grid){
        int[] number_of_potential_assignments = new int[] { 0, 0, 0, 0, 0, 0, 0};
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
        // bool areWeConsistent = !TooFew(grid) && !TooMany(grid);

        grid[w, l] = new List<TileType>();
        grid[w, l].AddRange(old_assignment);
        // return areWeConsistent;
        return true;
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

    void assignInitial(List<TileType>[,] solution){
        while (true){
            wr = Random.Range(1, width - 1);
            lr = Random.Range(1, length - 1);
            if (solution[wr, lr][0] == TileType.FLOOR)
            {
                float x = bounds.min[0] + (float)wr * (bounds.size[0] / (float)width);
                float z = bounds.min[2] + (float)lr * (bounds.size[2] / (float)length);
                //fps_player_obj = Instantiate(kk_prefab);
                //fps_player_obj.name = "PLAYER";
                // character is placed above the level so that in the beginning, he appears to fall down onto the maze
                //fps_player_obj.transform.position = new Vector3(x + 0.5f, 2.0f * tree_height, z + 0.5f); 
                break;
            }
        }
    }
    int assignFinal(){
        int max_dist = -1;
        while (true){
            if (wee != -1)
                break;
            for (int we = 0; we < width; we++){
                for (int le = 0; le < length; le++){
                    // skip corners
                    if (we == 0 && le == 0)
                        continue;
                    if (we == 0 && le == length - 1)
                        continue;
                    if (we == width - 1 && le == 0)
                        continue;
                    if (we == width - 1 && le == length - 1)
                        continue;

                    if (we == 0 || le == 0 || wee == length - 1 || lee == length - 1){
                        // randomize selection
                        if (Random.Range(0.0f, 1.0f) < 0.1f){
                            int dist = System.Math.Abs(wr - we) + System.Math.Abs(lr - le);
                            if (dist > max_dist) // must be placed far away from the player
                            {
                                wee = we;
                                lee = le;
                                max_dist = dist;
                            }
                        }
                    }
                }
            }
        }
        
       return max_dist;

    }   
    
     
    void DrawDungeon(List<TileType>[,] solution){
        int count = 0 ;
        // GetComponent<Renderer>().material = grass;
        //GetComponent<Renderer>().material.color = Color.grey;

        assignInitial(solution); // Assigning initial position
        int max_dist = assignFinal(); // Assigning Final Position

        //Djikstra

        //Instantiating board- 
        int w = 0;
        for (float x = bounds.min[0]; x < bounds.max[0]; x += bounds.size[0] / (float)width - 1e-6f, w++)
        {
            int l = 0;
            for (float z = bounds.min[2]; z < bounds.max[2]; z += bounds.size[2] / (float)length - 1e-6f, l++)
            {
                if ((w >= width) || (l >= width))
                    continue;

                float y = bounds.min[1];
                //Debug.Log(w + " " + l + " " + h);
                if ((w == wee-3) && (l == lee-3)) // this is the exit
                {
                    GameObject cave = Instantiate(cave_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                    cave.name = "CAVE";
                    cave.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
                    if (l == 0)
                        cave.transform.Rotate(0.0f, 270.0f, 0.0f);
                    else if (w == 0)
                        cave.transform.Rotate(0.0f, 0.0f, 0.0f);
                    else if (l == length - 1)
                        cave.transform.Rotate(0.0f, 90.0f, 0.0f);
                    else if (w == width - 1)
                        cave.transform.Rotate(0.0f, 180.0f, 0.0f);

                    cave.AddComponent<BoxCollider>();
                    cave.GetComponent<BoxCollider>().isTrigger = true;
                    cave.GetComponent<BoxCollider>().size = new Vector3(3.0f, 3.0f, 3.0f);
                    //cave.AddComponent<House>();
                }
                else if (solution[w, l][0] == TileType.MOUNT)
                {
                    // Debug.Log(count);
                    count = count+1; 
                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject mountain = Instantiate(mountain_prefab, new Vector3(0, -5, 0), Quaternion.identity);
                    mountain.name = "MOUNT";
                    mountain.transform.localScale = new Vector3(0.2f ,0.6f, 0.2f);
                // y= y + mountain_height / 2.0f
                    mountain.transform.position = new Vector3(x + 0.5f, -2, z + 0.5f);
                    mountain.AddComponent<BoxCollider>();
                    //cube.GetComponent<Renderer>().material.color = new Color(0.6f, 0.8f, 0.8f);
                }
                else if (solution[w, l][0] == TileType.TREES)
                {
                    // Debug.Log(count);
                    count = count+1; 
                    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    GameObject tree = Instantiate(tree_prefab, new Vector3(0, -5, 0), Quaternion.identity);
                    tree.name = "TREE";
                    tree.transform.localScale = new Vector3(0.2f ,0.6f, 0.2f);
                // y= y + mountain_height / 2.0f
                    tree.transform.position = new Vector3(x + 0.5f, -2, z + 0.5f);
                    BoxCollider boxc = tree.AddComponent<BoxCollider>();
                    boxc.center = new Vector3(0,0,0);
                    boxc.size = new Vector3(1,1,1);
                    tree.AddComponent<noPassingThrough>();
                    boxc.isTrigger = true;
                    // Rigidbody rigid = tree.AddComponent<Rigidbody> (); // To make trees fall
                    // rigid.collisionDetectionMode = CollisionDetectionMode.Continuous;
                    // rigid.isKinematic = false;
                    //cube.GetComponent<Renderer>().material.color = new Color(0.6f, 0.8f, 0.8f);
                }
                // else if (solution[w, l][0] == TileType.BUSHES)
                // {
                //     Debug.Log(count);
                //     count = count+1; 
                //     //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //     GameObject bush = Instantiate(bushes_prefab, new Vector3(0, -5, 0), Quaternion.identity);
                //     bush.name = "BUSH";
                //     bush.transform.localScale = new Vector3(0.2f ,0.6f, 0.2f);
                // // y= y + mountain_height / 2.0f
                //     bush.transform.position = new Vector3(x + 0.5f, -2, z + 0.5f);
                //     bush.AddComponent<BoxCollider>();
                //     //cube.GetComponent<Renderer>().material.color = new Color(0.6f, 0.8f, 0.8f);
                // }
                
                // else if (solution[w, l][0] == TileType.VIRUS)
                // {
                //     GameObject virus = Instantiate(virus_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                //     virus.name = "COVID";
                //     virus.transform.position = new Vector3(x + 0.5f, y + Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);
                //     virus.AddComponent<Virus>();
                //     virus.GetComponent<Rigidbody>().mass = 10000;
                // }
                // else if (solution[w, l][0] == TileType.DRUG)
                // {
                //     GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                //     capsule.name = "DRUG";
                //     capsule.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                //     capsule.transform.position = new Vector3(x + 0.5f, y + Random.Range(1.0f, storey_height / 2.0f), z + 0.5f);
                //     capsule.GetComponent<Renderer>().material.color = Color.green;
                //     capsule.AddComponent<Drug>();
                // }
                // else if (solution[w, l][0] == TileType.WATER)
                // {
                //     GameObject water = Instantiate(water_prefab, new Vector3(0, 0, 0), Quaternion.identity);
                //     water.name = "WATER";
                //     water.transform.localScale = new Vector3(0.5f * bounds.size[0] / (float)width, 1.0f, 0.5f * bounds.size[2] / (float)length);
                //     water.transform.position = new Vector3(x + 0.5f, y + 0.1f, z + 0.5f);

                //     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //     cube.name = "WATER_BOX";
                //     cube.transform.localScale = new Vector3(bounds.size[0] / (float)width, storey_height / 20.0f, bounds.size[2] / (float)length);
                //     cube.transform.position = new Vector3(x + 0.5f, y, z + 0.5f);
                //     cube.GetComponent<Renderer>().material.color = Color.grey;
                //     cube.GetComponent<BoxCollider>().size = new Vector3(1.1f, 20.0f * storey_height, 1.1f);
                //     cube.GetComponent<BoxCollider>().isTrigger = true;
                //     cube.AddComponent<Water>();
                // }
            }
        }
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }
}
