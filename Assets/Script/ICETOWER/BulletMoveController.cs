using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BulletMoveController : MonoBehaviour
{

    #region ▼ 定数

    /// <summary>x軸移動限界</summary>
    public float _XPositionLimit = 650.0f;

    /// <summary>スピードの設定</summary>
    public float _speed = 0.0f;

    /// <summary>向き</summary>
    private Vector3 _direction = Vector3.zero;

    #endregion

    #region ▼ オブジェクト

    private Rigidbody2D _Bullet = null;

    #endregion

    #region ▼ 初期処理

    void Start()
    {
        
    }

    #endregion

    #region ▼ 表示された時に稼働

    void OnEnable()
    {
        // バレットの向きとプレイヤーの向きを統一
        _direction = GameObject.Find("Charactor").gameObject.transform.localScale;
        gameObject.transform.localScale = _direction;

        _Bullet = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine("Move");
    }

    #endregion

    #region ▼ 移動

    private IEnumerator Move()
    {
        while (Mathf.Abs(transform.localPosition.x) <= _XPositionLimit)
        {
            _Bullet.velocity = new Vector2(_direction.x, 0) * _speed;
            yield return null;
        }        
        BulletPool.instance.ReleaseGameObject(gameObject);
        yield break;
    }

    #endregion

    #region ▼ 当たり判定処理

    IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "IceBlock_body")
        {
            // リリースタイミングを遅延させる
            yield return new WaitForSeconds(0.01f);
            BulletPool.instance.ReleaseGameObject(gameObject);
        }
        yield break;
    }

    #endregion
}