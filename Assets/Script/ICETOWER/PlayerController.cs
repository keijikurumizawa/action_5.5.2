using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    /// <summary>
    /// 入力の状態管理
    /// </summary>
    public enum AxisState : int
    {
        InputtoRight = 1,
        InputtoLeft = 2,
        InputNothing = 3
    }

    #region ▼ 定数・変数    

    /// <summary>プレイヤー標準スケール</summary>
    private Vector3 _PlayerDefaultScale = Vector3.zero;

    /// <summary>最大加速量</summary>
    [SerializeField]
    private float _MaxAccelaration = 360.0f;

    /// <summary>移動スピード</summary>
    [SerializeField]
    private float _Speed = 100.0f;

    /// <summary>ジャンプ力</summary>
    [SerializeField]
    private float _jumpForce = 550.0f;

    /// <summary>壁ジャンプ力</summary>
    [SerializeField]
    private float _climbJumpForce = 330.0f;

    /// <summary>壁ジャンプ力横軸</summary>
    private float _climbJumpSideForce = 1000.0f;

    /// <summary>壁ジャンプ力ウェイト</summary>
    private float _climbJumpWait = 0.08f;    

    /// <summary>入力フラグ</summary>
    private bool InputFlag = false;

    /// <summary>着地モーション判定</summary>
    private bool _playerLandingFlag = false;

    #endregion

    #region ▼ 静的

    /// <summary>プレイヤーの状態保持</summary>
    public static string _PlayerState = "wait";

    /// <summary>移動方向</summary>
    public static Vector3 _direction = Vector3.zero;

    /// <summary>空中フラグ</summary>
    public static bool _playerAirFlag = false;

    /// <summary>壁上りフラグ</summary>
    public static bool _playerClimbJumpFlag = false;

    /// <summary>消滅フラグ</summary>
    public static bool _playerActiveFlag = true;

    /// <summary>スティック入力値</summary>
    public static float Vertical = 0.0f;
    public static float Horizontal = 0.0f;

    private static int axisNumber = 0;
    private static int beforeaxisNumber = 3;

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
    private Text text2 = null;

    /// <summary>カメラ</summary>
    [SerializeField]
    private GameObject camera = null;

    /// <summary>アニメーション管理</summary>
    private PlayerAnimController _playerAnimCtl = null;

    #endregion

    void Start ()
    {
        _PlayerState = "ready";
        _direction = Vector3.zero;
        _playerAirFlag = false;
        Vertical = 0.0f;
        Horizontal = 0.0f;
        axisNumber = -1;
        beforeaxisNumber = 3;

        _PlayerDefaultScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        StartCoroutine(test());

        _playerAnimCtl = GetComponent<PlayerAnimController>();
    }

    IEnumerator test()
    {
        while (true)
        {            
            text.text = _player.velocity.y.ToString();
            text2.text = _PlayerState.ToString();
            yield return null;
        }       
    }

    void Update ()
    {
        Horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        Vertical = CrossPlatformInputManager.GetAxis("Vertical");
        Horizontal = Mathf.Clamp(Horizontal, -1.0f, 1.0f);
        Vertical = Mathf.Clamp(Vertical, -1.0f, 1.0f);

        // スティックの入力から処理を切り替える
        // 前回の入力から変更があった時にプレイヤーの状態・向きの情報を切り替える
        if (Horizontal > 0.3f)
        {
            axisNumber = (int)AxisState.InputtoRight;
            if (axisNumber != beforeaxisNumber && _PlayerState != "jump_attack")
            {
                if (!_playerAirFlag)
                {
                    _PlayerState = "Rightwalk";
                    _playerAnimCtl.SendMessage("AnimControl", "walk_start");
                }
                InputFlag = true;
                _direction = Vector3.right.normalized;
                _player.transform.localScale = new Vector3(_PlayerDefaultScale.x, _PlayerDefaultScale.y, _PlayerDefaultScale.z);
                camera.SendMessage("SendDir", "right");
                beforeaxisNumber = (int)AxisState.InputtoRight;
            }            
        }
        else if(Horizontal < -0.3f)
        {
            axisNumber = (int)AxisState.InputtoLeft;
            if(axisNumber != beforeaxisNumber && _PlayerState != "jump_attack")
            {
                if (!_playerAirFlag)
                {
                    _PlayerState = "Leftwalk";
                    _playerAnimCtl.SendMessage("AnimControl", "walk_start");
                }
                InputFlag = true;
                _direction = Vector3.left.normalized;
                _player.transform.localScale = new Vector3(-_PlayerDefaultScale.x, _PlayerDefaultScale.y, _PlayerDefaultScale.z);
                camera.SendMessage("SendDir", "left");
                beforeaxisNumber = (int)AxisState.InputtoLeft;
            }            
        }
        else
        {
            axisNumber = (int)AxisState.InputNothing;
            if (axisNumber != beforeaxisNumber)
            {
                if (!_playerAirFlag)
                {
                    _PlayerState = "wait";
                    _playerAnimCtl.SendMessage("AnimControl", "walk_end");
                }
                InputFlag = false;
                _direction = Vector3.zero.normalized;
                beforeaxisNumber = (int)AxisState.InputNothing;
            }
        }
    }

    #region ▼ 移動処理

    void FixedUpdate()
    {
        switch (_PlayerState)
        {
            case "ready":
                _player.velocity = new Vector2(0, _player.velocity.y);
                break;

            case "wait":
                _player.velocity = new Vector2(0, 0);
                break;

            case "Leftwalk":
            case "Rightwalk":
                // プレイヤーの加速度が一定値以上にならないように加算する
                if (_MaxAccelaration > Mathf.Abs(_player.velocity.x))
                {
                    _player.velocity = new Vector2(Horizontal * _Speed, 0.0f);
                }
                else
                {
                    _player.velocity = new Vector2(_MaxAccelaration * _direction.x, 0.0f);
                }
                break;

            case "jump":
            case "climbjump":
                if (!_playerLandingFlag)
                {
                    _player.velocity = new Vector2(_Speed * Horizontal, _player.velocity.y);
                }
                break;

            case "jump_attack":
                if (!_playerLandingFlag)
                {
                    _player.velocity = new Vector2(0.0f, _player.velocity.y);
                }
                break;

            case "climb":
                if (!_playerLandingFlag)
                {
                    _player.velocity = new Vector2(_player.velocity.x, _player.velocity.y);
                }
                break;            
        }
    }

    #endregion

    #region ▼ 移動・ジャンプ処理
    /*

    /// <summary>
    /// プレイヤー移動停止処理(ボタン操作)
    /// </summary>
    public void StopPlayer()
    {
        _direction = Vector3.zero.normalized;
        InputFlag = false;

        // ボタンを離した時の入力判定でwait状態にする
        if (!_playerAirFlag)
        {
            _playerAnimCtl.SendMessage("AnimControl", "walk_end");
            _PlayerState = "wait";
        }        
    }

    /// <summary>
    /// プレイヤー左方向移動処理
    /// </summary>
    public void LefttoPlayer()
    {
        if (!_playerAirFlag)
        {
            _PlayerState = "Leftwalk";
            _playerAnimCtl.SendMessage("AnimControl", "walk_start");
        }
        InputFlag = true;
        _direction = Vector3.left.normalized;
        _player.transform.localScale = new Vector3(-_PlayerDefaultScale.x, _PlayerDefaultScale.y, _PlayerDefaultScale.z);
        camera.SendMessage("SendDir", "left");
    }

    /// <summary>
    /// プレイヤー右方向移動処理
    /// </summary>
    public void RighttoPlayer()
    {
        if (!_playerAirFlag)
        {
            _PlayerState = "Rightwalk";
            _playerAnimCtl.SendMessage("AnimControl", "walk_start");
        }
        InputFlag = true;
        _direction = Vector3.right.normalized;
        _player.transform.localScale = new Vector3(_PlayerDefaultScale.x, _PlayerDefaultScale.y, _PlayerDefaultScale.z);
        camera.SendMessage("SendDir", "right");
    }
    */

    /// <summary>
    /// プレイヤージャンプ処理
    /// </summary>
    public void JumptoPlayer()
    {
        // 空中にいる際もしくは壁上り時
        if ((!_playerAirFlag || _PlayerState == "climb") && !_playerLandingFlag)
        {
            _playerAirFlag = true;
            _playerAnimCtl.SendMessage("AnimControl", "jump_start");
            _playerAnimCtl.SendMessage("AnimControl", "walk_end");

            // 通常ジャンプ
            if (_PlayerState != "climb")
            {
                _PlayerState = "jump";
                _player.velocity += new Vector2(0.0f, _jumpForce);
                if (!InputFlag)
                {
                    _direction = Vector3.zero.normalized;
                }
            }
            else
            {
                // 壁ジャンプ用コルーチン
                if (!_playerClimbJumpFlag)
                {
                    StartCoroutine("ClimbJump");
                }                
            }
        }
    }

    #endregion

    #region ▼ 壁ジャンプ処理

    IEnumerator ClimbJump()
    {
        if (_player.velocity.y < 0.0f)
        {
            _PlayerState = "climbjump";
            ClimbController._charaStay = false;
            _playerClimbJumpFlag = true;

            // 入力方向にジャンプ
            if (InputFlag)
            {
                _player.velocity += new Vector2(0.0f, _climbJumpForce);
            }
            // 垂直にジャンプ
            else
            {
                // 一度逆方向に加速度を付与したあとに、順方向に同量の力を加える
                _player.velocity = new Vector2(_climbJumpSideForce * ClimbController._climbDir, _climbJumpForce);
                yield return new WaitForSeconds(_climbJumpWait);
                _player.velocity = new Vector2(_climbJumpSideForce * -ClimbController._climbDir, _climbJumpForce);
            }            
        }
        _playerClimbJumpFlag = false;

        yield break;
    }

    #endregion

    #region ▼ プレイヤー着地処理

    IEnumerator LandingPlayer()
    {
        _playerLandingFlag = true;
        ClimbController._charaStay = false;
        
        _playerAnimCtl.SendMessage("AnimControl", "jump_end");
        
        // アニメーション終了まで待機
        _playerAnimator.Play(Animator.StringToHash("Jump_Landing"));
        yield return null;
        yield return new PlayerWaitForAnimation(_playerAnimator, 0);

        // アニメーション終了後の処理
        if (InputFlag)
        {
            _playerAnimCtl.SendMessage("AnimControl", "walk_start");
            if (_direction.x == Vector3.left.normalized.x) { _PlayerState = "Leftwalk"; }
            else if (_direction.x == Vector3.right.normalized.x) { _PlayerState = "Rightwalk"; }
        }
        else
        {
            InputFlag = false;
            _direction = Vector3.zero.normalized;
            _PlayerState = "wait";
        }

        _playerLandingFlag = false;

        yield break;
    }

    #endregion

    #region ▼ 当たり判定処理

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ground" || collision.gameObject.name == "IceBlock_floor")
        {
            if (_PlayerState == "jump" || _PlayerState == "climb" || _PlayerState == "climbjump" || _PlayerState == "jump_attack")
            {
                StartCoroutine(LandingPlayer());
            }
        }

        if(collision.gameObject.name == "IceBlock_bottom")
        {
            _playerActiveFlag = false;
            gameObject.SetActive(false);
        }
    }

    #endregion

    #region ▼ 当たり判定処理

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Ground" || collision.gameObject.name == "IceBlock_floor")
        {         
            _playerAirFlag = false;
        }
    }

    #endregion
}
