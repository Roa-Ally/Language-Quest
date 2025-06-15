using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float _startPos,_length;
    [SerializeField] private GameObject _cam;
    [SerializeField] private float _parallaxEffect;


    void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = _cam.transform.position.x * _parallaxEffect;
        float movement = _cam.transform.position.x * (1 - _parallaxEffect);

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);
        if(movement > _startPos + _length)
        {
            _startPos += _length;

        } else if(movement < _startPos - _length)
        {
            _startPos -= _length;
        }
    }
}
