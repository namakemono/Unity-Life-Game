using UnityEngine;
using System.Collections;

public class LifeGameController : MonoBehaviour {
	private static int MAX_WIDTH;
	private static int MAX_HEIGHT;
	private static int SCREEN_WIDTH;
	private static int NUM_CELLS;
	private static int CELL_SIZE = 3;
	private float frequency = 0.05f;
	private bool evaluating = false;
	private bool[][] pairMap;
	private static Color32 alive = Color.yellow;
	private static Color32 dead = Color.white;
	private Color32[] pixels;
	private Texture2D texture;
	private static int[] neighbors;
	private int turn = 0;

	void Start () {
		MAX_HEIGHT = Screen.height / CELL_SIZE;
		MAX_WIDTH = Screen.width / CELL_SIZE;
		SCREEN_WIDTH = MAX_WIDTH * CELL_SIZE;
		NUM_CELLS = MAX_WIDTH * MAX_HEIGHT;
		neighbors = new int[]{-MAX_WIDTH - 1, -MAX_WIDTH, -MAX_WIDTH + 1, -1, 1, MAX_WIDTH - 1, MAX_WIDTH, MAX_WIDTH + 1};
		this.pairMap = new bool[2][];
		for (int i = 0; i < this.pairMap.Length; ++i)
			this.pairMap[i] = new bool[NUM_CELLS];
		for (int i = 0; i < (int)(NUM_CELLS * frequency); ++i)
			this.pairMap[0][Random.Range(0, NUM_CELLS)] = true;
		this.pixels = new Color32[NUM_CELLS * CELL_SIZE * CELL_SIZE];
		this.texture = new Texture2D(MAX_WIDTH * CELL_SIZE, MAX_HEIGHT * CELL_SIZE);
	}
	
	void Update () {
		if (!evaluating) {
			evaluating = true;
			StartCoroutine("Simulate");
		}
	}

	int GetNumAlive(int position, ref bool[] map) {
		int cnt = 0;
		for (int i = 0; i < neighbors.Length; ++i)
			if (map[(position + neighbors[i] + NUM_CELLS) % NUM_CELLS])
				++cnt;
		return cnt;
	}

	IEnumerator Simulate() {
		yield return new WaitForSeconds(0.1f);
		this.Evaluate(ref this.pairMap[this.turn % 2], ref this.pairMap[(this.turn + 1) % 2]);
		this.Render(ref this.pairMap[this.turn % 2]);
		this.evaluating = false;
		++this.turn;
		yield return null;
	}

	void Evaluate(ref bool[] curMap, ref bool[] nextMap) {
		for (int i = 0; i < NUM_CELLS; ++i) {
			int cnt = this.GetNumAlive(i, ref curMap);
			nextMap[i] = ((curMap[i] && (cnt == 2)) || (cnt == 3));
		}
	}

	void Render(ref bool[] map) {
		for (int x = 0; x < MAX_WIDTH; ++x) {
			for (int y = 0; y < MAX_HEIGHT; ++y) {
				Color32 deadOrAlive = map[x + MAX_WIDTH * y] ? alive : dead;
				for (int dx = 0; dx < CELL_SIZE; ++dx) {
					for (int dy = 0; dy < CELL_SIZE; ++dy) {
						int pos = (x + y * SCREEN_WIDTH) * CELL_SIZE + (dx + dy * SCREEN_WIDTH);
						this.pixels[pos] = deadOrAlive;
					}
				}
			}
		}
		this.texture.SetPixels32(this.pixels);
		this.texture.Apply();
	}

	void OnGUI() {
		GUI.DrawTexture(new Rect(0, 0, this.texture.width, this.texture.height), this.texture);
	}
}
