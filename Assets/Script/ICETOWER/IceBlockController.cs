using UnityEngine;
using System.Collections;

public class IceBlockController : MonoBehaviour {

    #region ▼ 変数

    /// <summary>設置座標基準値</summary>
    private float _defaultPosition = 128.0f;
    private float seconds = 1.0f;
    private int rand = 0;
    private static int _setNum = 0;

    private static bool FormFlag = false;

    private Quaternion _defaultQuaternion = new Quaternion(0, 0, 0, 0);

    #endregion

    #region ▼ オブジェクト

    /// <summary>アイスブロック</summary>
    [SerializeField]
    private GameObject _IceBlockPrefab = null;

    /// <summary>アイスプール</summary>
    [SerializeField]
    private Transform _IcePool = null;

    #endregion

    void Start () {
        //StartCoroutine("SetIceBlock");
        _setNum = 0;
    }
	
	void Update () {
	
	}

    private IEnumerator SetIceBlock()
    {
        while (PlayerController._playerActiveFlag && _setNum < 30)
        { 
            rand = Random.Range(-3, 4);
            GameObject _iceblock = (GameObject)Instantiate(_IceBlockPrefab, new Vector3(rand * _defaultPosition, 900.0f, 0.0f), _defaultQuaternion);
            _iceblock.name = _IceBlockPrefab.name;
            _iceblock.transform.SetParent(_IcePool, false);
            _setNum++;
            yield return new WaitForSeconds(seconds);
        }
        yield break;
    }

    public void EraseIce()
    {
        GameObject erase = transform.FindChild("IcePool").gameObject;
        foreach (Transform n in erase.transform)
        {
            GameObject.Destroy(n.gameObject);
        }
        _setNum = 0;
        StartCoroutine("SetIceBlock");
    }
}
