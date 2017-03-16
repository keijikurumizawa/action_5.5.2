using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour {

    #region ▼ 変数

    private float comboTime = 0.0f;
    private int comboCnt = 0;
    private float stateTime = 0.0f;
    private int stateHash = 0;

    // 攻撃予約フラグ
    private bool reserveFlag = false;

    // 空中攻撃一回制限
    private bool airAttackLimit = false;

    #endregion

    #region ▼ オブジェクト

    /// <summary>キャラアニメーター</summary>
    private Animator _playerAnimator = null;

    /// <summary>プレイヤーrigidbody</summary>
    private Rigidbody2D _player = null;

    private AnimatorStateInfo currentState;

    #endregion

    void Start () {
        _playerAnimator = GetComponent<Animator>();
        _player = GetComponent<Rigidbody2D>();
        StartCoroutine(ComboManage());

        // 状態取得用
        currentState = _playerAnimator.GetCurrentAnimatorStateInfo(0);
        string a = currentState.fullPathHash.ToString();
        string b = Animator.StringToHash("Blade Layer.wait").ToString();
    }
	
	void Update () {
        //_playerAnimator.SetFloat("stateTime", stateTime);
        //_playerAnimator.SetFloat("comboTime", comboTime);

        // 地面着地時に空中攻撃強制終了
        if(_playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Blade Layer.Jump_Landing") )
        {
            _playerAnimator.SetBool("Air_Attack", false);
            PlayerController._PlayerState = "jump";
            _player.gravityScale = 120.0f;
            airAttackLimit = false;
        }
    }

    IEnumerator ComboManage()
    {
        while (true)
        {
            if (_playerAnimator.GetBool("Attack") || _playerAnimator.GetBool("Air_Attack"))
            {
                AnimatorStateInfo currentState = _playerAnimator.GetCurrentAnimatorStateInfo(0);
                stateTime = currentState.length;

                _playerAnimator.SetFloat("stateTime", stateTime);
                _playerAnimator.SetFloat("comboTime", comboTime);

                comboTime += Time.deltaTime;

                // 前フレームの状態ハッシュ値と現フレームの状態ハッシュ値が違う場合はコンボの入力時間を初期化する
                // 攻撃アニメーションが変わった時に条件が一致する
                if(stateHash != currentState.fullPathHash)
                {
                    comboTime = 0.0f;
                }

                // 次の攻撃の予約をした場合はトリガー始動、予約を初期化する
                if (reserveFlag)
                {
                    _playerAnimator.SetTrigger("ComboTrigger");
                    reserveFlag = false;
                }

                // 空中と地上攻撃で終了判断を変える
                if (_playerAnimator.GetBool("Attack"))
                {
                    // アニメーションの時間よりコンボ入力時間が上回った時、攻撃終了と判断する
                    if (comboTime >= stateTime)
                    {   
                        _playerAnimator.SetBool("Attack", false);
                        comboTime = 0.0f;
                        comboCnt = 0;
                    }
                }

                if (_playerAnimator.GetBool("Air_Attack"))
                {                    
                    if (comboTime > stateTime && comboCnt > 0)
                    {
                        _playerAnimator.SetBool("Air_Attack", false);
                        PlayerController._PlayerState = "jump";
                        _player.gravityScale = 120.0f;
                        airAttackLimit = true;
                        comboTime = 0.0f;
                        comboCnt = 0;
                    }
                }

                // フレームの最後で状態のハッシュ値を保存
                stateHash = currentState.fullPathHash;
            }
            else
            {
                // 攻撃中のフラグでない場合は
                // コンボ時間・攻撃予約・トリガーリセット
                comboTime = 0.0f;
                comboCnt = 0;
                reserveFlag = false;
                _playerAnimator.ResetTrigger("ComboTrigger");
            }

            yield return null;
        }
    }

    // ここで攻撃の形態を判断、呼び出す関数を変える
    public void AttackController()
    {
        if (PlayerController._playerAirFlag)
        {
            AirAttack();
        }
        else
        {
            NormalAttack();
            comboCnt++;
        }        

        
    }

    private void AirAttack()
    {
        if (!airAttackLimit && comboCnt < 3)
        {
            PlayerController._PlayerState = "jump_attack";

            _player.velocity = new Vector2(0.0f, 150.0f);
            _player.gravityScale = 60.0f;
            _playerAnimator.SetBool("Air_Attack", true);

            //次攻撃の予約(現stateとタップ時の時間を比べて一定時間以下であれば予約)
            if ((Mathf.Abs(stateTime - comboTime) != 0.0f && comboCnt != 0) && comboCnt < 4)
            {
                reserveFlag = true;
            }

            comboCnt++;
        }        
    }

    private void NormalAttack()
    {
        _playerAnimator.SetBool("Attack", true);

        //次攻撃の予約(現stateとタップ時の時間を比べて一定時間以下であれば予約)
        if ((Mathf.Abs(stateTime - comboTime) != 0.0f && comboCnt != 0))
        {
            reserveFlag = true;
        }
    }
}
