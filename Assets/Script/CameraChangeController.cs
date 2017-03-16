using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeController : MonoBehaviour {

    #region ▼ 変数

    /// <summary>現在のカメラの範囲(名前指定)</summary>
    private string CurrentCameraRangeName = "";
    private string NextCameraRangeName = "";

    /// <summary>別カメラ移行前の情報記録</summary>
    private float _maincameraSize = 0.0f;

    /// <summary>画面切り替え演出のフラグ(切り替えまでコリダー設置)</summary>
    public static bool _ChangeCameraFlag = false;

    #endregion

    #region ▼ オブジェクト

    public Camera mainCamera = null;

    #endregion

    void Start ()
    {
        _ChangeCameraFlag = false;
        _maincameraSize = mainCamera.orthographicSize;
        
        // 初期はどちらも同じ箇所から
        CurrentCameraRangeName = "1";
        NextCameraRangeName = "1";
    }

    void Update()
    {
        // ここでトリガーを毎フレームチェック
        // カメラ切り替えが終了するまで移動処理を行わない
        if(CurrentCameraRangeName != NextCameraRangeName && !_ChangeCameraFlag)
        {
            _ChangeCameraFlag = true;
            StartCoroutine(MoveCameraFunc(NextCameraRangeName));
        }
    }

    /// <summary>
    /// 移動予定のカメラ名を保存
    /// </summary>
    /// <param name="objName"></param>
    public void Trigger(string NextCameraName)
    {        
        if (CurrentCameraRangeName != NextCameraName)
        {
            NextCameraRangeName = NextCameraName;
        }
    }

    IEnumerator MoveCameraFunc(string toCameraName)
    {
        string to = toCameraName + "Camera";
        float time = 0.0f;
        float magni = 0.0f;

        // 現在のカメラ範囲名を保存
        CurrentCameraRangeName = toCameraName;

        // メインカメラの追従限界座標の更新
        mainCamera.SendMessage("ChangeStageRange", toCameraName);

        Camera toCamera = GameObject.Find(to).gameObject.GetComponent<Camera>();
        
        // 表示倍率 = to カメラサイズ / main カメラサイズ
        // 画面表示のアスペクトのサイズ変更
        magni = toCamera.orthographicSize / _maincameraSize;
        CameraController.CameraMagni = magni;

        while(0.1f < Mathf.Abs(mainCamera.orthographicSize - toCamera.orthographicSize))
        {
            // カメラの描画サイズの変更
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, toCamera.orthographicSize, 0.05f);
            time += Time.deltaTime;
            yield return null;
        }

        _ChangeCameraFlag = false;

        yield break;
    }
}
