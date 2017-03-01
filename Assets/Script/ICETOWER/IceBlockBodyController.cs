using UnityEngine;
using System.Collections;

public class IceBlockBodyController : MonoBehaviour {

    #region ▼ 変数

    private float _Life = 15.0f;

    #endregion

    #region ▼ オブジェクト

    private GameObject _parent = null;

    #endregion

    void Start () {
        _parent = transform.parent.gameObject;
    }

	void Update () {
	
	}

    #region ▼ 当たり判定処理

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "NormalBullet")
        {
            _Life -= BulletController._NormalBulletDamage;
            DamageCheck();
        }
        if (collision.gameObject.name == "ChargeBullet")
        {
            _Life -= BulletController._ChargeBulletDamage;
            DamageCheck();
        }
    }

    #endregion

    #region ▼ ダメージチェック

    private void DamageCheck()
    {
        // ここの下に演出なり加える

        if (_Life <= 0)
        {
            Destroy(_parent.gameObject);
        }
    }

    #endregion
}
