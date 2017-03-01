using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    #region ▼ 定数・変数

    /// <summary>プレイヤーの状態保持</summary>
    public static string _PlayerState = "wait";

    /// <summary>移動方向</summary>
    private Vector3 _direction = Vector3.zero;

    /// <summary>ジャンプ力</summary>
    private float _jumpForce = 42.0f;

    /// <summary>壁ジャンプ力</summary>
    private float _climbJumpForce = 33.0f;

    /// <summary>壁ジャンプ力横軸</summary>
    private float _climbJumpSideForce = 12.0f;

    /// <summary>壁ジャンプ力ウェイト</summary>
    private float _climbJumpWait = 0.08f;

    /// <summary>移動スピード</summary>
    private float _Speed = 8.0f;

    /// <summary>加速度一時保存</summary>
    private Vector2 _tmpVelocity = Vector2.zero;

    /// <summary>空中フラグ</summary>
    public static bool _playerAirFlag = false;

    /// <summary>消滅フラグ</summary>
    public static bool _playerActiveFlag = true;

    #endregion

    #region ▼ オブジェクト

    /// <summary>プレイヤー</summary>
    [SerializeField]
    private Rigidbody2D _player = null;

    /// <summary>アニメーション</summary>
    [SerializeField]
    private Animator _playerAnimator = null;

    /// <summary>状態</summary>
    [SerializeField]
    private Text text = null;

    /// <summary>状態</summary>
    [SerializeField]
    private Text air = null;

    #endregion

    void Start ()
    {
        _PlayerState = "wait";
        _playerAirFlag = false;
        _playerActiveFlag = true;
    }
	
	void Update ()
    {
        text.text = _direction.ToString();
        air.text = _PlayerState;
    }

    void OnEnable()
    {
        StartCoroutine("PlayerMove");
    }

    #region ▼ プレイヤー移動処理

    private IEnumerator PlayerMove()
    {
        Vector2 tmp = Vector2.zero;
        bool ReversalDir = false;

        while (true)
        {
            // 移動方向に進む
            if(_PlayerState != "wait")
            {
                // 方向転換
                if(tmp != (Vector2)_direction && !ReversalDir)
                {
                    // 加算後の加速度が規定値を超える時は、加算を行わない
                    // かつ、キャラが壁に貼り付いていない時
                    if (25.0f > Mathf.Abs(_player.velocity.x + (_direction.x * _Speed)))
                    {
                        //_player.velocity += (Vector2)_direction * _Speed;                        
                        _player.velocity += new Vector2(_direction.x, 0.0f) * _Speed;
                    }
                    ReversalDir = true;
                }
                if (Mathf.Abs(_player.velocity.x) < 20.0f)
                {
                    // 加算後の加速度が規定値を超える時は、加算を行わない
                    // かつ、キャラが壁に貼り付いていない時
                    if (25.0f > Mathf.Abs(_player.velocity.x + (_direction.x * _Speed)))
                    {
                        //_player.velocity += (Vector2)_direction * _Speed;
                        _player.velocity += new Vector2(_direction.x, 0.0f) * _Speed;
                        tmp = _direction;
                    }                    
                }
            }
            else
            {
                _player.velocity = Vector2.MoveTowards(_player.velocity, new Vector2(0, 0), 0.3f);
            }

            ReversalDir = false;
            yield return null;
        }
    }

    #endregion

    #region ▼ 移動・ジャンプ処理

    /// <summary>
    /// プレイヤー移動停止処理(ボタン操作)
    /// </summary>
    public void StopPlayer()
    {
        _direction = Vector3.zero.normalized;

        // ボタンを離した時の入力判定でwait状態にする
        if(!_playerAirFlag)
        {
            _playerAnimator.Play(Animator.StringToHash("wait"));
            _PlayerState = "wait";
        }        
    }

    /// <summary>
    /// プレイヤー左方向移動処理
    /// </summary>
    public void LefttoPlayer()
    {
        if (_PlayerState == "wait")
        {
            _PlayerState = "walk";
            _playerAnimator.Play(Animator.StringToHash("walk"));
        }
        _direction = Vector3.left.normalized;
        _player.transform.localScale = new Vector3(-1, 1, 1); 
    }

    /// <summary>
    /// プレイヤー右方向移動処理
    /// </summary>
    public void RighttoPlayer()
    {
        if (_PlayerState == "wait")
        {
            _PlayerState = "walk";
            _playerAnimator.Play(Animator.StringToHash("walk"));
        }
        _direction = Vector3.right.normalized;
        _player.transform.localScale = new Vector3(1, 1, 1);      
    }

    /// <summary>
    /// プレイヤージャンプ処理
    /// </summary>
    public void JumptoPlayer()
    {
        if(!_playerAirFlag || (IceBlockClimbController._charaStay && _PlayerState == "jump"))
        {
            _playerAirFlag = true;
            _PlayerState = "jump";
            _playerAnimator.Play(Animator.StringToHash("jump"));

            // 通常ジャンプ
            if (!IceBlockClimbController._charaStay)
            {
                _player.velocity += new Vector2(0.0f, _jumpForce);
                if (_PlayerState != "walk")
                {
                    _direction = Vector3.zero.normalized;
                }
            }
            // 壁ジャンプ
            else
            {
                // 壁ジャンプ用コルーチン
                StartCoroutine("ClimbJump");
                IceBlockClimbController._charaStay = false;
            }
        }
    }

    #endregion

    #region ▼ 壁ジャンプ処理

    IEnumerator ClimbJump()
    {
        // 壁に対して逆方向に飛ぶ
        //_player.velocity = new Vector2(25.0f * IceBlockClimbController._climbDir, _climbJumpForce);

        if (_player.velocity.y < 0.0f)
        {
            // 一度逆方向に加速度を付与したあとに、順方向に力を加える
            // 某アクションをイメージした壁ジャンプ
            _player.velocity = new Vector2(_climbJumpSideForce * IceBlockClimbController._climbDir, _climbJumpForce);
            yield return new WaitForSeconds(_climbJumpWait);
            _player.velocity = new Vector2(_climbJumpSideForce * -IceBlockClimbController._climbDir, _climbJumpForce);
        }
        yield break;
    }

    #endregion

    #region ▼ プレイヤー着地処理

    public void LandingPlayer()
    {
        _playerAirFlag = false;
        IceBlockClimbController._charaStay = false;

        if (_direction != Vector3.zero.normalized)
        {
            _playerAnimator.Play(Animator.StringToHash("walk"));
            _PlayerState = "walk";
        }
        else
        {
            _direction = Vector3.zero.normalized;
            _playerAnimator.Play(Animator.StringToHash("wait"));
            _PlayerState = "wait";
        }
    }

    #endregion

    #region ▼ 当たり判定処理

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ground" || collision.gameObject.name ==  "IceBlock_floor")
        {
            if (_PlayerState == "jump")
            {
                LandingPlayer();
            }
        }

        if(collision.gameObject.name == "IceBlock_bottom")
        {
            _playerActiveFlag = false;
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region ▼ 復活コマンド

    public void Resurrection()
    {
        _player.transform.localPosition = new Vector3(0.0f, 500.0f, 0.0f);
        _playerActiveFlag = true;
        _player.gameObject.SetActive(true);
    }

    #endregion
}
