using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour {

    #region ▼ 変数

    private float velocityY = 0.0f;

    #endregion

    #region ▼ オブジェクト

    private Animator _CharaAnimator = null;

    private Rigidbody2D _Charactor = null;

    #endregion

    void Start ()
    {
        _Charactor = GetComponent<Rigidbody2D>();
        _CharaAnimator = GetComponent<Animator>();
    }
	
	void Update ()
    {
        velocityY = _Charactor.velocity.y;
        _CharaAnimator.SetFloat("velocityY", velocityY);
        _CharaAnimator.SetFloat("Horizontal", Mathf.Abs(PlayerController.Horizontal));
        _CharaAnimator.SetFloat("Vertical", Mathf.Abs(PlayerController.Vertical));
    }

    void FixedUpdate()
    {
        
    }

    public void AnimControl(string trigger)
    {
        switch (trigger)
        {
            case "walk_start":
                _CharaAnimator.SetBool("Walk", true);
                _CharaAnimator.SetBool("Attack", false);
                break;
            case "walk_end":
                _CharaAnimator.SetBool("Walk", false);
                break;

            case "jump_start":
                _CharaAnimator.SetBool("Jump", true);
                _CharaAnimator.SetBool("Attack", false);
                break;
            case "jump_end":
                _CharaAnimator.SetBool("Jump", false);
                _CharaAnimator.SetBool("Attack", false);
                break;

            case "landing_start":
                _CharaAnimator.SetBool("Landing", true);
                break;
            case "landing_end":
                _CharaAnimator.SetBool("Landing", false);
                break;
        }

        //_CharaAnimator.SetBool("Attack", false);
    } 
}
