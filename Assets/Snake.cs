//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour {

    private Vector2 _direction = Vector2.right;
    private float _rotation = 90.0f;
    private List<Transform> _segments;
    public Transform segmentPrefab;

    private SpriteRenderer _spriteRenderer;
    public Sprite[] sprites;
    private int _spriteIndex = 0;

    // Start is called before the first frame update
    void Start() {
        _segments = new List<Transform>();
        _segments.Add(this.transform);
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

    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            _direction = Vector2.up;
            _rotation = 180.0f;
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            _direction = Vector2.down;
            _rotation = 0.0f;
        }
        else if (Input.GetKeyDown(KeyCode.A)) {
            _direction = Vector2.left;
            _rotation = -90.0f;
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            _direction = Vector2.right;
            _rotation = 90.0f;
        }
    }

    private void FixedUpdate() {
        for (int i = _segments.Count - 1; i > 0; i--) {
            _segments[i].position = _segments[i - 1].position;
            _segments[i].rotation = _segments[i - 1].rotation;
        }
        this.transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f
        );
        this.transform.rotation = Quaternion.Euler(0, 0, _rotation);
    }

    private void Grow() {
        Transform segment = Instantiate(this.segmentPrefab);
        segment.position = _segments[_segments.Count - 1].position;
        _segments.Add(segment);
    }

    private void ResetState() {
        for (int i = 1; i < _segments.Count; i++) {
            Destroy(_segments[i].gameObject);
        }
        _segments.Clear();
        _segments.Add(this.transform);
        this.transform.position = Vector3.zero;
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
