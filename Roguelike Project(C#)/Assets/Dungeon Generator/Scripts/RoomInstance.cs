using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomInstance : MonoBehaviour {
	public Texture2D tex;
	[HideInInspector]
	public Vector2 gridPos;
	public int type; // 0: normal, 1: enter
    //[HideInInspector]
    //public Vector2[] enemySpawn;
    //该房间是否还有怪物(BOSS不算)
    [HideInInspector]
    public bool isClean;
    [HideInInspector]
    //是否是第一次进这个房间
    public bool firstEnter;
    [HideInInspector]
	public bool doorTop, doorBot, doorLeft, doorRight;
    [SerializeField]
    GameObject doorU, doorD, doorL, doorR, wallU, wallD, wallL, wallR, cornerUL, cornerDL, cornerDR, cornerUR, obstacle;
    //[SerializeField]
    //GameObject enemy1;
    float tileSize = 20.8f;
	Vector2 roomSizeInTiles = new Vector2(9,17);
	public void Setup(Texture2D _tex, Vector2 _gridPos, int _type, bool _doorTop, bool _doorBot, bool _doorLeft, bool _doorRight){
		tex = _tex;
		gridPos = _gridPos;
        type = _type;
        //enemySpawn = _enemySpawn;
        doorTop = _doorTop;
		doorBot = _doorBot;
		doorLeft = _doorLeft;
		doorRight = _doorRight;
		MakeDoors();
		GenerateRoomTiles();
	}
    void Start()
    {
        if(type == 1)
        {
            isClean = true;
            firstEnter = true;
        }
        else
        {
            isClean = false;
            firstEnter = true;
        }
    }
    //private Transform player;
    //private void Start()
    //{
    //    player = GameObject.Find("Player").GetComponent<Transform>();
    //}
    //private void Update()
    //{
    //    if(Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) < 170)
    //    {
    //        type = 1;
    //    }
    //    else
    //    {
    //        type = 0;
    //    }
    //}
    //生成上下左右四个点的门或墙
    void MakeDoors(){
		//top door, get position then spawn
		Vector3 spawnPos = transform.position + Vector3.up*(roomSizeInTiles.y/4 * tileSize) - Vector3.up*(tileSize/4);
		PlaceDoor(spawnPos, doorTop, doorU, wallU);
		//bottom door
		spawnPos = transform.position + Vector3.down*(roomSizeInTiles.y/4 * tileSize) - Vector3.down*(tileSize/4);
		PlaceDoor(spawnPos, doorBot, doorD, wallD);
		//right door
		spawnPos = transform.position + Vector3.right*(roomSizeInTiles.x * tileSize) - Vector3.right*(tileSize);
		PlaceDoor(spawnPos, doorRight, doorR, wallR);
		//left door
		spawnPos = transform.position + Vector3.left*(roomSizeInTiles.x * tileSize) - Vector3.left*(tileSize);
		PlaceDoor(spawnPos, doorLeft, doorL, wallL);
	}
    //生成door(检测上下左右4个点)
	void PlaceDoor(Vector3 spawnPos, bool door, GameObject doorSpawn, GameObject wallSpawn){
		// check whether its a door or wall, then spawn
		if (door){
			Instantiate(doorSpawn, spawnPos, Quaternion.identity).transform.parent = transform;
		}else{
			Instantiate(wallSpawn, spawnPos, Quaternion.identity).transform.parent = transform;
		}
	}
    //生成所有wall和corner
    void GenerateRoomTiles()
    {
        //loop through every pixel of the texture
        for (int x = 0; x < tex.width; x++)
        {
            if(x == 0 || x == tex.width - 1)
            {
                if(x == 0)
                {
                    for (int y = 0; y < tex.height; y++)
                    {
                        if(y == 0)
                        {
                            GenerateULCornerTile(x, y);
                        }
                        else if(y == tex.height - 1)
                        {
                            GenerateDLCornerTile(x, y);
                        }
                        else
                        {
                            if (y != (tex.height - 1) / 2)
                            {
                                GenerateLeftWallTile(x, y);
                            }
                        }
                    }
                }
                if(x == tex.width - 1)
                {
                    for (int y = 0; y < tex.height; y++)
                    {
                        if (y == 0)
                        {
                            GenerateURCornerTile(x, y);
                        }
                        else if (y == tex.height - 1)
                        {
                            GenerateDRCornerTile(x, y);
                        }
                        else
                        {
                            if (y != (tex.height - 1) / 2)
                            {
                                GenerateRightWallTile(x, y);
                            }
                        }
                    }
                }
            }
            else
            {
                //除了出生的默认房间外
                if(type != 1)
                {
                    if (SceneManager.GetActiveScene().name != Game.Instance.StaticData.Level2)
                    {
                        //排除最外的两圈
                        if (x != 1 && x != tex.width - 2)
                        {
                            for (int y = 0; y < tex.height; y++)
                            {
                                if (y != 0 && y != 1 && y != tex.height - 1 && y != tex.height - 2)
                                {
                                    //生成障碍物
                                    int ran = Random.Range(0, 10);
                                    if (ran == 0)
                                    {
                                        GenerateObstacleTile(x, y);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //排除最外的三圈
                        if (x != 1 &&  x != 2 && x != tex.width - 2 && x != tex.width - 3)
                        {
                            for (int y = 0; y < tex.height; y++)
                            {
                                if (y != 0 && y != 1 && y != 2 && y != tex.height - 1 && y != tex.height - 2 && y != tex.height - 3)
                                {
                                    //生成障碍物
                                    int ran = Random.Range(0, 10);
                                    if (ran == 0)
                                    {
                                        GenerateObstacleTile(x, y);
                                    }
                                }
                            }
                        }
                    }
                }
                for (int y = 0; y < tex.height; y++)
                {
                    if(y == 0)
                    {
                        if (x != (tex.width - 1) / 2)
                        {
                            GenerateUpWallTile(x, y);
                        }
                    }
                    if(y == tex.height - 1)
                    {
                        if (x != (tex.width - 1) / 2)
                        {
                            GenerateButtomWallTile(x, y);
                        }
                    }
                }
            }
        }
    }
    //实例化一个obstacle到格子
    void GenerateObstacleTile(int x, int y)
    {
        GeneraPrefab(x, y, obstacle);
    }
    //实例化一个wall到格子
    void GenerateUpWallTile(int x, int y)
    {
        GeneraPrefab(x, y, wallU);
    }
    void GenerateButtomWallTile(int x, int y)
    {
        GeneraPrefab(x, y, wallD);
    }
    void GenerateLeftWallTile(int x, int y)
    {
        GeneraPrefab(x, y, wallL);
    }
    void GenerateRightWallTile(int x, int y)
    {
        GeneraPrefab(x, y, wallR);
    }
    //实例化一个corner到格子
    void GenerateULCornerTile(int x, int y)
    {
        GeneraPrefab(x, y, cornerUL);
    }
    void GenerateURCornerTile(int x, int y)
    {
        GeneraPrefab(x, y, cornerUR);
    }
    void GenerateDLCornerTile(int x, int y)
    {
        GeneraPrefab(x, y, cornerDL);
    }
    void GenerateDRCornerTile(int x, int y)
    {
        GeneraPrefab(x, y, cornerDR);
    }
    //void GenerateEnemyTile(int x, int y)
    //{
    //    GeneraPrefab(x, y, enemy1);
    //}

    void GeneraPrefab(int x, int y, GameObject go)
    {
        Vector3 spawnPos = positionFromTileGrid(x, y);
        Instantiate(go, spawnPos, Quaternion.identity).transform.parent = this.transform;
    }
    //找到房间里所有格子的position
    Vector3 positionFromTileGrid(int x, int y){
		Vector3 ret;
		//find difference between the corner of the texture and the center of this object
		Vector3 offset = new Vector3((-roomSizeInTiles.x + 1)*tileSize, (roomSizeInTiles.y/4)*tileSize - (tileSize/4), 0);
		//find scaled up position at the offset
		ret = new Vector3(tileSize * (float) x, -tileSize * (float) y, 0) + offset + transform.position;
		return ret;
	}
}
