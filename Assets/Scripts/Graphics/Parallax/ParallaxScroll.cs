using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxScroll : MonoBehaviour {
    // background 1: 4.41 | background 2: 14.08137
    // midground 1: 5.605 | midground 2: 17.665

    public GameObject[] backgroundElements;

    public GameObject[][] backgroundElements2;

    public GameObject[] midgroundElements;
    public GameObject[] foregroundElements;
    public Tilemap[] levelTileSet;
    public Camera cam;
    public static ParallaxScroll instance;

    [HideInInspector]
    public bool speedUp = false;
    [HideInInspector]
    public bool slowDown = false;

    public float baseSpeed;
    public float parallaxScrollSpeed = 10.0f;
    public float backgroundRate = 1.0f;
    public float midgroundRate = 3.0f;
    public float foregroundRate = 5.0f;
    public float levelRate = 4.0f;
    public float boostSpeed = 5.0f;
    // default values; change in unity editor

    private float tilemapSize;
    private Rigidbody2D tilemapRigidBody;

    //public MidgroundElements stuff;

    private Vector3 backgroundOrigin1;
    private Vector3 backgroundOrigin2;
    private Vector3 midgroundOrigin1;
    private Vector3 midgroundOrigin2;
    private Vector3 foregroundOrigin1;
    private Vector3 foregroundOrigin2;
    // values ~MUST~ be applied in unity editor (UPDATE: not true as they are now privated variables)
    // these values are the x, y, and z coordinates of the images for the background, midground, and foreground

    private Vector3[] backgroundOrigins;
    private Vector3[] midgroundOrigins;
    private Vector3[] foregroundOrigins;

    private bool backgroundsLooped = false;
    private bool midgroundsLooped = false;
    private bool foregroundsLooped = false;


    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        baseSpeed = parallaxScrollSpeed;
        if (levelTileSet.Length >= 1) {
            tilemapSize = levelTileSet[0].size.y;
            tilemapRigidBody = levelTileSet[0].GetComponent<Rigidbody2D>();
        }
        if (!(tilemapSize % 2 == 0)) {
            tilemapSize += 1;
        }

        if (backgroundElements.Length > 1) {
            backgroundOrigin1 = new Vector3(0.0f, -9.0f, backgroundElements[0].transform.position.z);
            backgroundOrigin2 = new Vector3(0.0f, backgroundElements[0].GetComponent<SpriteRenderer>().bounds.size.y - 9.0f, backgroundElements[1].transform.position.z);
            print(backgroundElements[0].GetComponent<SpriteRenderer>().bounds.size.y);

            backgroundElements[0].transform.position = backgroundOrigin1;
            backgroundElements[1].transform.position = backgroundOrigin2;
        }

        if (midgroundElements.Length > 1) {
            midgroundOrigin1 = new Vector3(0.0f, -9.0f, midgroundElements[0].transform.position.z);
            midgroundOrigin2 = new Vector3(0.0f, midgroundElements[0].GetComponent<SpriteRenderer>().bounds.size.y - 9.0f, midgroundElements[1].transform.position.z);
            print(midgroundElements[0].GetComponent<SpriteRenderer>().bounds.size.y / 2);

            midgroundElements[0].transform.position = midgroundOrigin1;
            midgroundElements[1].transform.position = midgroundOrigin2;
        }

        if (foregroundElements.Length > 1) {
            foregroundOrigin1 = new Vector3(0.0f, -9.0f, foregroundElements[0].transform.position.z);
            foregroundOrigin2 = new Vector3(0.0f, foregroundElements[0].GetComponent<SpriteRenderer>().bounds.size.y - 9.0f, foregroundElements[1].transform.position.z);
        }
    }

    void Update() {
        if (speedUp) {
            if (parallaxScrollSpeed < baseSpeed + boostSpeed) {
                parallaxScrollSpeed += boostSpeed * Time.unscaledDeltaTime;
                if (parallaxScrollSpeed > baseSpeed + boostSpeed) {
                    parallaxScrollSpeed = baseSpeed + boostSpeed;
                }
            }
        } else if (slowDown) {
            if (parallaxScrollSpeed > baseSpeed) {
                parallaxScrollSpeed -= boostSpeed * Time.unscaledDeltaTime;
                if (parallaxScrollSpeed < baseSpeed) {
                    parallaxScrollSpeed = baseSpeed;
                }
            }
        }
        if (parallaxScrollSpeed == baseSpeed) {
            speedUp = false;
            slowDown = false;
        }

        if (backgroundElements.Length == 2) {
            backgroundUpdate();
        }
        if (midgroundElements.Length == 2) {
            midgroundUpdate();
        }
        if (foregroundElements.Length == 2) {
            foregroundUpdate();
        }
    }
    public void FixedUpdate() {
        if (levelTileSet.Length > 0) {
            tileMapUpdate();
        }
    }

    void backgroundUpdate() {
        if (!backgroundsLooped) {
            if (backgroundElements[1].transform.localPosition.y < backgroundOrigin1.y) {
                backgroundElements[0].transform.localPosition = backgroundOrigin2;
                backgroundsLooped = true;
            }
        } else {
            if (backgroundElements[0].transform.localPosition.y < backgroundOrigin1.y) {
                backgroundElements[1].transform.localPosition = backgroundOrigin2;
                backgroundsLooped = false;
            }
        }
        //if (backgroundElements[1].transform.localPosition.y <= backgroundOrigin1.y) {
        //    if (backgroundElements[0].transform.localPosition.y < backgroundElements[1].transform.localPosition.y) {
        //        //backgroundElements[1].transform.localPosition = backgroundOrigin1;
        //        backgroundElements[0].transform.localPosition = backgroundOrigin2;
        //    } else {
        //        //backgroundElements[0].transform.localPosition = backgroundOrigin1;
        //        backgroundElements[1].transform.localPosition = backgroundOrigin2;
        //    }
        //}
        for (int i = 0; i < backgroundElements.Length; i++) {
            backgroundElements[i].transform.position = backgroundElements[i].transform.position - new Vector3(0, parallaxScrollSpeed * backgroundRate * Time.deltaTime, 0);
        }
    }

    void midgroundUpdate() {
        if (!midgroundsLooped) {
            if (midgroundElements[1].transform.localPosition.y < midgroundOrigin1.y) {
                midgroundElements[0].transform.localPosition = midgroundOrigin2;
                midgroundsLooped = true;
            }
        } else {
            if (midgroundElements[0].transform.localPosition.y < midgroundOrigin1.y) {
                midgroundElements[1].transform.localPosition = midgroundOrigin2;
                midgroundsLooped = false;
            }
        }
        for (int i = 0; i < midgroundElements.Length; i++) {
            midgroundElements[i].transform.position = midgroundElements[i].transform.position - new Vector3(0, parallaxScrollSpeed * midgroundRate * Time.deltaTime, 0);
        }
    }

    void foregroundUpdate() {
        if (foregroundElements[1].transform.localPosition.y <= foregroundOrigin1.y) {
            if (foregroundElements[0].transform.localPosition.y < foregroundElements[1].transform.localPosition.y) {
                //foregroundElements[1].transform.localPosition = foregroundOrigin1;
                foregroundElements[0].transform.localPosition = foregroundOrigin2;
            } else {
                //foregroundElements[0].transform.localPosition = foregroundOrigin1;
                foregroundElements[1].transform.localPosition = foregroundOrigin2;
            }
        }
        for (int i = 0; i < foregroundElements.Length; i++) {
            foregroundElements[i].transform.position = foregroundElements[i].transform.position - new Vector3(0, parallaxScrollSpeed * foregroundRate * Time.deltaTime, 0);
        }
    }

    void tileMapUpdate() {
        if (levelTileSet[0].transform.position.y > -tilemapSize + 21.0f) {
            tilemapRigidBody.MovePosition(new Vector2(levelTileSet[0].transform.position.x, levelTileSet[0].transform.position.y - (parallaxScrollSpeed * levelRate * Time.fixedDeltaTime)));
        }
        if (levelTileSet[0].transform.position.y < -tilemapSize + 21.0f) {
            tilemapRigidBody.MovePosition(new Vector2(levelTileSet[0].transform.position.z, -tilemapSize + 21.0f));
        }
    }
}