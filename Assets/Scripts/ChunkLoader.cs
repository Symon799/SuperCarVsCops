using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [System.Serializable]
 public class GameObjectArray
 {
     public GameObject[] items;
 }

public class ChunkLoader : MonoBehaviour {

	public GameObject[] SkillsPrefabs;
	public GameObject[] floorPrefabs;
	public GameObjectArray[] biomeObstacles;
	public GameObject coinPrefab;
	private Transform player;
    private GameManager gameManager;
	private List<Chunk> chunks = new List<Chunk>();
	private Vector3 lastLocation = new Vector3(53f, 0.5f, 53f);
	private Vector3 lastChunkPos = Vector3.zero;
	private int coins = 0;
	public int currentBiome = 0;


	public class Chunk
	{
		public GameObject floor;
		public bool markForDestoy;

		public Chunk(GameObject floor, bool markForDestoy)
		{
			this.floor = floor;
			this.markForDestoy = markForDestoy;
		}
	}

	// Use this for initialization
	void Start () {
        gameManager = GetComponent<GameManager>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		player.position = lastLocation;
		UpdateFloor(GetCurrentChunkPos());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if ((player.position - lastLocation).magnitude >= 1)
		{
			Vector3 chunkPos = GetCurrentChunkPos();
			if (lastChunkPos != chunkPos)
			{
				lastChunkPos = chunkPos;
				UpdateFloor(chunkPos);
			}
		}
	}

	void UpdateFloor(Vector3 centerChunkPos)
	{
		currentBiome = gameManager.GetScore() / 100;
		while (currentBiome > floorPrefabs.Length - 1)
			currentBiome -= floorPrefabs.Length;

		//SetChunks to destroy
		foreach (Chunk chunk in chunks)
			chunk.markForDestoy = true;

		
		addChunk(centerChunkPos);
		addChunksAound(centerChunkPos);

		//destroy old chunks
		foreach (Chunk chunk in chunks.ToArray())
		{
			if (chunk.markForDestoy == true)
			{
				Destroy(chunk.floor);
				chunks.Remove(chunk);
			}
		}
	}

	void addChunksAound(Vector3 pos)
	{
		addChunk(pos + new Vector3(100, 0f, 0f));
		addChunk(pos + new Vector3(0f, 0f, 100));
		addChunk(pos + new Vector3(-100, 0f, 0f));
		addChunk(pos + new Vector3(0, 0f, -100));
		addChunk(pos + new Vector3(100, 0f, 100));
		addChunk(pos + new Vector3(-100, 0f, -100));
		addChunk(pos + new Vector3(100, 0f, -100));
		addChunk(pos + new Vector3(-100, 0f, 100));
	}

	void addChunk(Vector3 pos)
	{
		foreach (Chunk chunk in chunks)
		{
			if (pos == chunk.floor.transform.position)
			{
				chunk.markForDestoy = false;
				return;
			}
		}
		GameObject floorChunk = GameObject.Instantiate(floorPrefabs[currentBiome], pos, Quaternion.identity);
		chunks.Add(new Chunk(floorChunk, false));
		GameObject[] items = biomeObstacles[currentBiome].items;

		for (int i = 0; i < 15; i++)
			SpawnPrefabRandom(items[Random.Range(0, items.Length)], floorChunk);

		SpawnPrefabRandom(coinPrefab, floorChunk, 4);
		SpawnPrefabRandom(SkillsPrefabs[currentBiome], floorChunk, Random.Range(0,4));
	}

	Vector3 GetCurrentChunkPos()
	{
		float x = player.transform.position.x;
		float z = player.transform.position.z;
		Vector3 chunkPos = new Vector3(x + 100 - (x % 100), 0, z + 100 - (z % 100));

		//center chunk
		chunkPos -= new Vector3(50, 0, 50);

		if (chunkPos.x > 0)
		{
			if (chunkPos.z < 0)
				chunkPos -= new Vector3(0, 0, 100);

			if (z > -100 && z < 0)
				chunkPos -= new Vector3(0, 0, 100);

			if (x > -100 && x < 0)
				chunkPos -= new Vector3(100, 0, 0);
		}
		else
		{
			if (chunkPos.z < 0)
				chunkPos -= new Vector3(100, 0, 100);
			else
				chunkPos -= new Vector3(100, 0, 0);
			
			if (z > -100 && z < 0)
				chunkPos -= new Vector3(0, 0, 100);

		}
		return chunkPos;
	}

	void SpawnPrefabRandom(GameObject prefab, GameObject chunk, int count = 1)
	{
		for (int i = 0; i < count; i++)
		{
			//space the objects
			float xRange = (int)chunk.transform.position.x + Random.Range(-7, 7) * 7;
			float zRange = (int)chunk.transform.position.z + Random.Range(-7, 7) * 7;

        	foreach (Transform child in chunk.transform)
			{
				if (child.position.x == xRange && child.position.z == zRange)
				{
					SpawnPrefabRandom(prefab, chunk);
					return;
				}
			}

			GameObject obj = GameObject.Instantiate(prefab, new Vector3(xRange, 0f, zRange), Quaternion.identity);
			obj.transform.parent = chunk.transform;
		}
	}

	public void ResetChunks()
	{
		foreach (Chunk chunk in chunks.ToArray())
		{
			Destroy(chunk.floor);
			chunks.Remove(chunk);
		}
		currentBiome = 0;
		player.transform.position = new Vector3(53f, 0.5f, 53f);
		UpdateFloor(GetCurrentChunkPos());
	}
}
