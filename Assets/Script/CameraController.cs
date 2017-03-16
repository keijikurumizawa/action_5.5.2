using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region ▼ 変数

    /// <summary>カメラ追従座標指定</summary>
    private Vector3 _LerpCameraPos = Vector3.zero;

    /// <summary>カメラ座標リミット</summary>
    private Vector3 _LerpCameraLimitPos = Vector3.zero;

    /// <summary>カメラ座標リミットx軸,y軸</summary>
    private float _CameraLimitPosX = 0.0f;
    private float _CameraLimitPosY = 0.0f;

    /// <summary>カメラの目標x座標</summary>
    private float _CameraTargetPosX = 0.0f;

    /// <summary>カメラがキャラクタを追っているかをチェックするフラグ</summary>
    private bool _MoveCameraFlag = false;

    /// <summary>入力方向を保持する</summary>
    private static string _playerinputDir = "";

    /// <summary>プレイヤーの向き・移動・地上空中でカメラの座標を変化</summary>
    private float _CameraTargetPosXAddNum = 0.0f;
    private float _CameraTergetPosXMoveAddNum = 0.0f;
    private float _CameraTargetPosYAddNum = 100.0f;

    private Vector3 cameraBottomLeft;
    private Vector3 cameraTopLeft;
    private Vector3 cameraBottomRight;
    private Vector3 cameraTopRight;
    private int cameraRangeWidth;
    private int cameraRangeHeight;
    private float StageMinsizeX;
    private float StageMaxsizeX;
    private float StageMinsizeY;
    private float StageMaxsizeY;
    private Vector3 velocity;

    /// <summary>カメラの表示倍率</summary>
    public static float CameraMagni = 1.0f;

    #endregion

    #region ▼ オブジェクト

    /// <summary>プレイヤー</summary>
    public GameObject _player = null;

    /// <summary>ステージ</summary>
    public GameObject StageObject = null;
    private RectTransform StageRange = null;

    #endregion

    void Start()
    {        
        Application.targetFrameRate = 60;
        _playerinputDir = "right";
        StageRange = StageObject.transform.FindChild("1").GetComponent<RectTransform>();
        StartCoroutine(MoveCameraForWait());
    }

    void Update()
    {
        /// <summary>
        /// カメラを置いて欲しい座標を更新
        /// 入力・状態で判別
        /// </summary>
        if (_playerinputDir == "left")
        {
            _CameraTargetPosX = _player.transform.position.x - _CameraTargetPosXAddNum - _CameraTergetPosXMoveAddNum;
        }
        if (_playerinputDir == "right")
        {
            _CameraTargetPosX = _player.transform.position.x + _CameraTargetPosXAddNum + _CameraTergetPosXMoveAddNum;
        }
        //else if (PlayerController._direction.x == Vector3.zero.normalized.x) { _CameraTargetPosX = _player.transform.position.x; }

        // カメラの目標座標位置(画面外問わず)
        _LerpCameraPos = new Vector3(_CameraTargetPosX, _player.transform.position.y + _CameraTargetPosYAddNum, transform.position.z);

        //ビューポート座標をワールド座標に変換
        cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, -1));
        cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, -1));

        // カメラの左上座標と右下座標を算出
        cameraTopLeft = new Vector3(cameraBottomLeft.x, cameraTopRight.y, cameraBottomLeft.z);
        cameraBottomRight = new Vector3(cameraTopRight.x, cameraBottomLeft.y, cameraTopRight.z);

        // カメラが映している大きさを算出
        cameraRangeWidth = (int)(Vector3.Distance(cameraBottomLeft, cameraBottomRight) * CameraMagni);
        cameraRangeHeight = (int)(Vector3.Distance(cameraBottomLeft, cameraTopLeft) * CameraMagni);

        // ステージ外を見せない為の追従限界座標を算出(見える範囲とステージ範囲から算出)
        StageMinsizeX = (StageRange.sizeDelta.x * StageRange.pivot.x) + StageRange.transform.position.x;
        StageMaxsizeX = (StageRange.sizeDelta.x) + StageRange.transform.position.x;
        StageMinsizeY = (StageRange.sizeDelta.y * StageRange.pivot.y) + StageRange.transform.position.y;
        StageMaxsizeY = (StageRange.sizeDelta.y) + StageRange.transform.position.y;

        //_CameraLimitPosX = Mathf.Clamp(_LerpCameraPos.x, ((-Stage.sizeDelta.x / 2) + cameraRangeWidth / 2), ((Stage.sizeDelta.x / 2) - cameraRangeWidth / 2));
        _CameraLimitPosX = Mathf.Clamp(_LerpCameraPos.x, ((StageMinsizeX) + cameraRangeWidth / 2), ((StageMaxsizeX) - cameraRangeWidth / 2));
        _CameraLimitPosY = Mathf.Clamp(_LerpCameraPos.y, ((StageMinsizeY) + cameraRangeHeight / 2), ((StageMaxsizeY) - cameraRangeHeight / 2));

        // 画面外を表示させず、カメラの設置したい座標を算出
        _LerpCameraLimitPos = new Vector3(_CameraLimitPosX, _CameraLimitPosY, transform.position.z);
    }

   void FixedUpdate()
    {        
        if((PlayerController._PlayerState == "Rightwalk" || PlayerController._PlayerState == "Leftwalk") && 
            //Mathf.Abs(PlayerController.Axis) > 0.5f &&
                1.0f < Mathf.Abs(Vector2.Distance(new Vector2(transform.position.x, 0.0f), new Vector2(_player.transform.position.x, 0.0f))))
        {
            _CameraTargetPosXAddNum = 50.0f;
            _MoveCameraFlag = true;

            transform.position = Vector3.SmoothDamp(transform.position, _LerpCameraLimitPos, ref velocity, Time.deltaTime*10);            
        }

        _MoveCameraFlag = false;
    }

    /// <summary>
    /// キャラの向きを取得
    /// </summary>
    /// <input>PlayerController:LefttoPlayer,RighttoPlayer</input>
    /// <returns></returns>
    public void SendDir(string dir)
    {
        _CameraTargetPosXAddNum = 100.0f;
        /// <summary>
        /// カメラを置いて欲しい座標を更新
        /// </summary>
        if (dir == "left")
        {
            _CameraTargetPosX = _player.transform.position.x - _CameraTargetPosXAddNum;
        }
        if (dir == "right")
        {
            _CameraTargetPosX = _player.transform.position.x + _CameraTargetPosXAddNum;
        }
        _playerinputDir = dir;
    }

    /// <summary>
    /// カメラ追従(待機時)
    /// </summary>
    IEnumerator MoveCameraForWait()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if ((PlayerController._PlayerState == "wait" || PlayerController._PlayerState == "ready") && !_MoveCameraFlag)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _LerpCameraLimitPos, ref velocity, Time.deltaTime*10);
            }
            if((PlayerController._PlayerState == "climb") && !_MoveCameraFlag)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _LerpCameraLimitPos, ref velocity, Time.deltaTime*5);
            }
            if ((PlayerController._PlayerState == "jump" || PlayerController._PlayerState == "climbjump" || PlayerController._PlayerState == "jump_attack") && !_MoveCameraFlag)
            {
                transform.position = Vector3.SmoothDamp(transform.position, _LerpCameraLimitPos, ref velocity, Time.deltaTime*12);
            }

            yield return null;
        }
    }

    /// <summary>
    /// ステージレンジを動的に変更
    /// </summary>
    public void ChangeStageRange(string StageName)
    {
        StageRange = StageObject.transform.FindChild(StageName).GetComponent<RectTransform>();
        return;
    }
}
