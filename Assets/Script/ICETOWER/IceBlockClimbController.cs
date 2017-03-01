using UnityEngine;
using System.Collections;

public class IceBlockClimbController : MonoBehaviour {

    public static bool _charaStay = false;
    public static int _climbDir = 1;

    private GameObject _parent = null;
    private Rigidbody2D _player = null;

	void Start () {
        _parent = transform.parent.gameObject;
        _player = GameObject.Find("Charactor").GetComponent<Rigidbody2D>();
	}
	
	void Update () {
        if (_charaStay)
        {
            if(_player.velocity.y < -10.0f)
            {
                _player.velocity = new Vector2(_player.velocity.x, -10.0f);
            }
        }
	}

    #region ▼ 当たり判定

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Charactor")
        {
            if (!_charaStay && PlayerController._PlayerState == "jump" || PlayerController._playerAirFlag)
            {
                // 壁から逆の方向に飛ぶ
                // 壁に貼り付いている時は壁と逆方向に
                if (_player.transform.localPosition.x < _parent.transform.localPosition.x)
                {
                    _climbDir = -1;
                }
                if (_player.transform.localPosition.x > _parent.transform.localPosition.x)
                {
                    _climbDir = 1;
                }
                //_player.transform.localScale = new Vector3(_climbDir, 1, 1);
                
                _charaStay = true;
            }
        }
    }

    #endregion

    #region ▼ 当たり判定処理

    void OnTriggerExit2D(Collider2D collision)
    {
        _charaStay = false;
    }

    #endregion
}
