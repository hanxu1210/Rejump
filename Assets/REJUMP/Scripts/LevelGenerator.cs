using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelGenerator : MonoBehaviour 
{
    public Transform startBlock;                        //Start block transform;
    public Blocks[] blocksPool;                         //Game blocks transforms;
    public Offset blocksPositionOffset = new Offset();  //Game blocks position offsets
    public ChillBlocks chillBlocks;                     //Chill blocks settings;

    private Vector3 blockPos;
    private Transform playerT;
    private Block start;
    private Collider2D startCol;
    private int blockCounter;

	// Use this for initialization
	void Start () 
	{
        //Cache components;
        playerT = GameObject.FindGameObjectWithTag("Player").transform;
        start = startBlock.GetComponent<Block>();
        startCol = startBlock.GetComponent<Collider2D>();

        //Place blocks
        PlaceBlocks();
        //Check if block passed by the player;
        InvokeRepeating("ReplaceBlocks", 1, 0.5F);
	}

    //Place blocks function;
    public void PlaceBlocks()
    {
        //Enable start block's collider because it desable after player jump;
        startCol.enabled = true;

        //Calculate blocks placement based on start block position and offset values;
        blockPos = startBlock.position + Vector3.right * Random.Range(blocksPositionOffset.min, blocksPositionOffset.max);

        //Null blocks counter;
        blockCounter = 0;

        //Iterate through  blocks array
        for(int i = 0; i < blocksPool.Length; i++)
        {
            //Place block to calculated position;
            blocksPool[i].blockTransform.position = blockPos;

            //Enable block's collider;
            blocksPool[i].blockCollider.enabled = true;

            //Check if chill blocks enabled, and if so set block type to chill with desired chance;
            if (chillBlocks.enabled && Random.value < chillBlocks.chance / 100 && IsChill())
                blocksPool[i].block.SetChillBlock(true);
            else
                blocksPool[i].block.SetChillBlock(false);

            //Increase block position for next block;
            blockPos.x += Random.Range(blocksPositionOffset.min, blocksPositionOffset.max);

            //Increase blocks counter;
            blockCounter++;
        }
    }

    //Replace blocks function;
    void ReplaceBlocks()
    {
        //Iterate through  blocks array
        for(int i = 0; i < blocksPool.Length; i++)
        {
            //If block position is less then player position by 15
            if (blocksPool[i].blockTransform.position.x < playerT.position.x - 15)
            {
                //Move block to last calculated position;
                blocksPool[i].blockTransform.position = blockPos;

                //Enable its collider;
                blocksPool[i].blockCollider.enabled = true;

                //Check if chill blocks enabled, and if so set block type to chill with desired chance;
                if (chillBlocks.enabled && Random.value < chillBlocks.chance / 100 && IsChill())
                    blocksPool[i].block.SetChillBlock(true);
                else
                    blocksPool[i].block.SetChillBlock(false);

                //Increase block position for next block;
                blockPos.x += Random.Range(blocksPositionOffset.min, blocksPositionOffset.max);
                //Increase blocks counter;
                blockCounter++;
            }
        }
    }

    //Check if block can be chill based on density;
    bool IsChill()
    {
        return blockCounter > 0 && (blockCounter+1) % chillBlocks.density == 0;
    }

    //Get block status function. This function check if chill block is true and returns bool value;
    public bool GetBlockStatus(Collider2D block)
    {
        if (!chillBlocks.enabled && block != startCol)
            return false;
        else if (block != startCol)
        {
            for (int i = 0; i < blocksPool.Length; i++)
                if (blocksPool[i].blockCollider == block)
                    return blocksPool[i].block.chillBlock;
            return false;
        }
        else
            return start.chillBlock;
    }
}

//Blocks placement offsets;
[System.Serializable]
public class Offset
{
    public float min = 5.5F;        //Minimum distance between blocks;
    public float max = 13.5F;       //Maximum distance between blocks;
}

//Blocks pool class;
[System.Serializable]
public class Blocks
{
    public Transform blockTransform;        //Block object transform;
    public Block block;                     //Block component;
    public BoxCollider2D blockCollider;     //Block box collider;
}

//Chill blocks settings;
[System.Serializable]
public class ChillBlocks
{
    public bool enabled;                    //Enable chill blocks or not;
    [Range(2, 10)]
    public int density = 2;                 //Chil blocks dencity;
    [Range(1, 100)]
    public int chance = 10;                 //Chill block chance;
}