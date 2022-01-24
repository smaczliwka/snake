//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

    private Vector2 _direction = Vector2.up;
    private Vector2 _prev = Vector2.up;

    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;

    private SpriteRenderer _spriteRenderer;
    public Sprite[] sprites;
    private int _spriteIndex = 0;

    private int initialSize = 10; // should be greater than 1

    public Sprite tail, body, corner1, corner2;

    // Start is called before the first frame update
    void Start() {
        ResetState();
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void AnimateSprite() {
        _spriteIndex++;
        if (_spriteIndex >= sprites.Length) {
            _spriteIndex = 0;
        }
        _spriteRenderer.sprite = sprites[_spriteIndex];
    }

    private Quaternion RotationFromDirection(Vector2 direction) {
        if (direction == Vector2.right) return Quaternion.Euler(0, 0, -90);
        else if (direction == Vector2.down) return Quaternion.Euler(0, 0, 180);
        else if (direction == Vector2.left) return Quaternion.Euler(0, 0, 90);
        else return Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.W) && _prev != Vector2.down) {
            _direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _prev != Vector2.up) {
            _direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _prev != Vector2.right) {
            _direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _prev != Vector2.left) {
            _direction = Vector2.right;
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void FixedUpdate() {
        Vector2 direction = new Vector2(_direction.x, _direction.y);

        for (int i = _segments.Count - 1; i > 1; i--) {
            _segments[i].position = _segments[i - 1].position;
            _segments[i].rotation = _segments[i - 1].rotation;
            if (i == _segments.Count - 1) {
                _segments[i].gameObject.GetComponent<SpriteRenderer>().sprite = tail;
            }
            else { // if not tail and not just after head
                _segments[i].gameObject.GetComponent<SpriteRenderer>().sprite 
                    = _segments[i - 1].gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }

        // first segment adter head
        _segments[1].rotation = RotationFromDirection(direction); // same direction as head
        _segments[1].position = this.transform.position;        
        if (_prev.x * direction.y - _prev.y * direction.x < 0) { // turn right
            _segments[1].gameObject.GetComponent<SpriteRenderer>().sprite = corner1;
        }
        else if (_prev.x * direction.y - _prev.y * direction.x > 0) { // turn left
            _segments[1].gameObject.GetComponent<SpriteRenderer>().sprite = corner2;
        }
        else { // go stright forward
            _segments[1].gameObject.GetComponent<SpriteRenderer>().sprite = body;
        }

        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + direction.x,
            Mathf.Round(this.transform.position.y) + direction.y,
            0.0f
        );
        this.transform.rotation = RotationFromDirection(direction);
        _prev = direction;
    }

    private void Grow() {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        segment.rotation = _segments[_segments.Count - 1].rotation;
        _segments.Add(segment);
    }

    private void ResetState() {
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        _direction = Vector2.up;

        for (int i = 1; i < _segments.Count; i++) {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Food") {
            Grow();
        }
        else if (other.tag == "Obstacle") {
            ResetState();
        }
    }
}
