using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChangeTrigger : MonoBehaviour {

    #region ▼ 変数

    private string _objectName = "";

    #endregion

    #region ▼ オブジェクト

    private GameObject _ParentObject = null;

    #endregion

    void Start ()
    {
        _objectName = transform.name;
        _ParentObject = transform.parent.gameObject;
	}
	
	void Update ()
    {

    }

    #region ▼ プレイヤー接触判定

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "CharaCenter")
        {
            _ParentObject.SendMessage("Trigger", _objectName);
        }
    }

    #endregion
}
