using UnityEngine;
using System.Collections;

public class LifeGameController : MonoBehaviour {
	public const int MAX_HEIGHT = 512;
	public const int MAX_WIDTH = 512;
	private Texture2D curMap;
	private Texture2D nextMap;
	private bool evaluating = false;
	private static Color32 alive = Color.yellow;
	private static Color32 dead = Color.white;

	void Start () {
		this.Initialize();
	}

	void Initialize() {
		Color32[] pixels = new Color32[MAX_WIDTH * MAX_HEIGHT];
		for (int x = 0; x < MAX_WIDTH; ++x) {
			for (int y = 0; y < MAX_HEIGHT; ++y) {
				pixels[x + MAX_WIDTH * y] = (Random.value < 0.05f) ? alive : dead;
			}
		}
		this.curMap = new Texture2D(MAX_WIDTH, MAX_HEIGHT);
		this.curMap.SetPixels32(pixels);
		this.curMap.Apply();
		this.nextMap = new Texture2D(MAX_WIDTH, MAX_HEIGHT);
	}
	
	void Update () {
		if (!evaluating) {
			evaluating = true;
			StartCoroutine("Evaluate");
		}
	}

	bool InField(int x, int y) {
		return (0 <= x && x < MAX_WIDTH && 0 <= y && y < MAX_HEIGHT);
	}

	int GetPixelPosition(int x, int y) {
		return x + MAX_WIDTH * y;
	}

	// 生きているセルには青を入れないようにしている
	bool IsAlive(ref Color32 c) {
		return (c.b == alive.b);
	}

	int GetNumAlive(int x, int y, ref Color32[] pixels) {
		int cnt = 0;
		for (int dx = -1; dx <= 1; ++dx) {
			for (int dy = -1; dy <= 1; ++dy) {
				if (((dx|dy) != 0) && this.InField(x+dx, y+dy)) {
					if(this.IsAlive(ref pixels[this.GetPixelPosition(x+dx, y+dy)])) ++cnt;
				}
			}
		}
		return cnt;
	}

	IEnumerator Evaluate() {
		yield return new WaitForSeconds(0.3f);
		Color32[] curPixels = this.curMap.GetPixels32();
		Color32[] nextPixels = curPixels;
		for (int x = 0; x < MAX_WIDTH; ++x) {
			for (int y = 0; y < MAX_HEIGHT; ++y) {
				int cnt = this.GetNumAlive(x, y, ref curPixels);
				if (this.IsAlive(ref curPixels[this.GetPixelPosition(x, y)])) {
					nextPixels[this.GetPixelPosition(x, y)] = ((cnt == 2) || (cnt == 3)) ? alive : dead;
				} else { // Dead
					nextPixels[this.GetPixelPosition(x, y)] = (cnt == 3) ? alive : dead;
				}
			}
		}
		this.curMap.SetPixels32(nextPixels);
		this.curMap.Apply();
		this.evaluating = false;
		yield return null;
	}

	void OnGUI() {
		GUI.DrawTexture(new Rect(0, 0, MAX_WIDTH, MAX_HEIGHT), this.curMap);
	}
}
