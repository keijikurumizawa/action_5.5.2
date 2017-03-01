using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{

    private static BulletPool _instance = null;

    // シングルトン
    public static BulletPool instance
    {
        get
        {
            if (_instance == null)
            {
                // シーン上から取得する
                _instance = FindObjectOfType<BulletPool>();
            }

            return _instance;
        }
    }


    #region ▼ 定数


    #endregion

    #region ▼ オブジェクト

    private Dictionary<int, List<GameObject>> pooledGameObjects = new Dictionary<int, List<GameObject>>();

    #endregion

    // ゲームオブジェクトをpooledGameObjectsから取得する。必要であれば新たに生成する
    public GameObject GetGameObject(GameObject prefab, Vector2 position, Quaternion rotation)
    {
        // プレハブのインスタンスIDをkeyとする
        int key = prefab.GetInstanceID();

        // Dictionaryにkeyが存在しなければ作成する
        if (pooledGameObjects.ContainsKey(key) == false)
        {
            pooledGameObjects.Add(key, new List<GameObject>());
        }

        List<GameObject> gameObjects = pooledGameObjects[key];

        GameObject go = null;

        for (int i = 0; i < gameObjects.Count; i++)
        {
            go = gameObjects[i];

            // 現在非アクティブ（未使用）であれば
            if (go.activeInHierarchy == false)
            {
                // 位置・回転情報を設定
                go.transform.localPosition = position;
                go.transform.localRotation = rotation;

                // これから使用するのでアクティブにする
                go.SetActive(true);

                return go;
            }
        }

        // 使用できるものがないので新たに生成する(親オブジェクトを自身に設定)
        go = (GameObject)Instantiate(prefab, position, rotation);
        go.transform.name = prefab.name;
        go.transform.SetParent(transform, false);

        // リストに追加
        gameObjects.Add(go);

        return go;
    }

    // ゲームオブジェクトを非アクティブにする。こうすることで再利用可能状態にする
    public void ReleaseGameObject(GameObject go)
    {
        // 非アクティブにする
        go.SetActive(false);
    }
}
