using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContoroller : MonoBehaviour
{

    #region ▼ 変数

    /// <summary>カメラ追従座標指定</summary>
    private Vector3 _LerpCameraPos = Vector3.zero;

    /// <summary>カメラ座標リミット</summary>
    private Vector3 _LerpCameraLimitPos = Vector3.zero;

    /// <summary>カメラ座標リミットx軸,y軸</summary>
    private float _CameraLimitPosX = 0.0f;
    private float _CameraLimitPosY = 0.0f;

    /// <summary>カメラ追従中かフラグ</summary>
    private bool _LerpCameraFlag = false;

    private Vector3 cameraBottomLeft;
    private Vector3 cameraTopLeft;
    private Vector3 cameraBottomRight;
    private Vector3 cameraTopRight;
    private int cameraRangeWidth;
    private int cameraRangeHeight;

    #endregion

    #region ▼ オブジェクト

    /// <summary>プレイヤー</summary>
    public GameObject _player = null;

    /// <summary>ステージ</summary>
    public RectTransform Stage = null;

    #endregion

    void Start()
    {
        StartCoroutine(TrackLimitPosCalc());
        StartCoroutine(CheckDistance());
        StartCoroutine(LerpPosCalc());
        //StartCoroutine("check");
    }

    IEnumerator check()
    {
        while (true)
        {
            Debug.Log(_CameraLimitPosX);
            Debug.Log(_CameraLimitPosY);
            yield return new WaitForSeconds(1.0f);
        }        
    }

    // Update is called once per frame
    void Update()
    {        
        
    }

    /// <summary>
    /// カメラの追従限界座標計算
    /// </summary>
    /// <returns></returns>
    IEnumerator TrackLimitPosCalc()
    {
        while (true)
        {
            //ビューポート座標をワールド座標に変換
            cameraBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 500));
            cameraTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 500));

            // カメラの左上座標と右下座標を算出
            cameraTopLeft = new Vector3(cameraBottomLeft.x, cameraTopRight.y, cameraBottomLeft.z);
            cameraBottomRight = new Vector3(cameraTopRight.x, cameraBottomLeft.y, cameraTopRight.z);
            
            // カメラが映している大きさを算出
            cameraRangeWidth = (int)Vector3.Distance(cameraBottomLeft, cameraBottomRight);
            cameraRangeHeight = (int)Vector3.Distance(cameraBottomLeft, cameraTopLeft);
            
            // ステージ外を見せない為の追従限界座標を算出(見える範囲とステージ範囲から算出)
            _CameraLimitPosX = Mathf.Clamp(_LerpCameraPos.x, ((-Stage.sizeDelta.x / 2) + cameraRangeWidth / 2), ((Stage.sizeDelta.x / 2) - cameraRangeWidth / 2));
            _CameraLimitPosY = Mathf.Clamp(_LerpCameraPos.y, ((-Stage.sizeDelta.y / 2) + cameraRangeHeight / 2), ((Stage.sizeDelta.y / 2) - cameraRangeHeight / 2));

            _LerpCameraLimitPos = new Vector3(_CameraLimitPosX, _CameraLimitPosY, transform.position.z);
            yield return null;
        }
    }

    /// <summary>
    /// カメラ追従するかチェック
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckDistance()
    {
        while (true)
        {
            if( 180.0f < Vector2.Distance(transform.position, _player.transform.position) && !_LerpCameraFlag)
            {
                _LerpCameraFlag = true;
                StartCoroutine(Control());
            }
            yield return null;
        }
    }

    /// <summary>
    /// カメラ追従させる際の最終ポジション計算
    /// </summary>
    /// <returns></returns>
    IEnumerator LerpPosCalc()
    {
        // ここはプレイヤーが消えるまで計算を続ける
        while (true)
        {
            _LerpCameraPos = new Vector3(_player.transform.position.x, _player.transform.position.y + 100.0f, transform.position.z);
            yield return null;
        }
    }

    /// <summary>
    /// カメラ追従
    /// </summary>
    /// <returns></returns>
    IEnumerator Control()
    {
        // カメラの位置が標準位置±5.0fまで来るまで追従
        while (5.0f < Vector2.Distance(transform.position, _LerpCameraLimitPos))
        {            
            transform.position = Vector3.Lerp(transform.position, _LerpCameraLimitPos, 2.0f * Time.deltaTime);
            yield return null;
        }

        _LerpCameraFlag = false;
        yield break;
    }
}
