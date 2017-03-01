using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BulletController : MonoBehaviour {

    #region ▼ 変数

    // チャージ秒数・リミット
    private float _ChargeSeconds = 0.0f;
    private float _ChargeLimitSeconds = 2.0f;

    // ボタン入力フラグ
    // true:押下  false:離す
    private bool _ChargeButton = false;

    // キャラの向き
    private float _CharaDir = 0.0f;

    /// <summary>バレットダメージ</summary>
    public static float _NormalBulletDamage = 1.0f;
    public static float _ChargeBulletDamage = 15.0f;

    #endregion

    #region ▼ オブジェクト

    // キャラクター(アニメーションによって向きを判別
    public GameObject _Charactor = null;

    // バレットプール
    public Transform _BulletPool = null;

    // 各種バレット
    public GameObject _NormalBullet = null;
    public GameObject _ChargeBullet = null;

    // チャージアニメーション
    public Animator _ChargeAnim = null;
    public Animator _FullChargeAnim = null;

    #endregion

    void Start () {

    }
	
	void Update () {
        _CharaDir = _Charactor.transform.localScale.x;
        _ChargeAnim.transform.localPosition = _Charactor.transform.localPosition;
        _FullChargeAnim.transform.localPosition = _Charactor.transform.localPosition;
    }

    public void ChargeButtonDown()
    {
        _ChargeButton = true;
        _ChargeSeconds = 0.0f;        
        StartCoroutine("Charge");
    }

    public void ChargeButtonUp()
    {
        _ChargeButton = false;
        StartCoroutine("Shot");

        _ChargeAnim.Stop();
        _FullChargeAnim.Stop();
        _ChargeAnim.gameObject.SetActive(false);
        _FullChargeAnim.gameObject.SetActive(false);
    }

    private IEnumerator Charge()
    {
        while (_ChargeButton)
        {            
            _ChargeSeconds += Time.deltaTime;
            if(_ChargeSeconds > _ChargeLimitSeconds / 4.0f)
            {
                _ChargeAnim.gameObject.SetActive(true);
                _ChargeAnim.Play(Animator.StringToHash("charge"));
                if (_ChargeSeconds > _ChargeLimitSeconds)
                {
                    _FullChargeAnim.gameObject.SetActive(true);
                    _FullChargeAnim.Play(Animator.StringToHash("fullcharge"));
                }
            }            
            yield return null;
        }        
        yield break;
    }

    private IEnumerator Shot()
    {
        float ChargeShotInterval = 120.0f;
        float NormalShotInterval = 90.0f;

        if(_ChargeSeconds > _ChargeLimitSeconds)
        {
            // チャージショット
            BulletPool.instance.GetGameObject(_ChargeBullet, new Vector3(_Charactor.transform.localPosition.x + (ChargeShotInterval * _CharaDir), _Charactor.transform.localPosition.y, 0.0f), new Quaternion(0, 0, 0, 0));
        }
        else
        {
            // ノーマルショット
            BulletPool.instance.GetGameObject(_NormalBullet, new Vector3(_Charactor.transform.localPosition.x + (NormalShotInterval * _CharaDir), _Charactor.transform.localPosition.y, 0.0f), new Quaternion(0, 0, 0, 0));
        }

        yield break;
    }
}
