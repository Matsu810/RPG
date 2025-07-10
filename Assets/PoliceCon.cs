using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCon : MonoBehaviour
{
    [SerializeField] private bool Jumpflag;
    [SerializeField] private float JumpSpeed = 5.0f;
    [SerializeField] private float MoveSpeed = 5.5f;
    private int jumpcount = 0;
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public float gravityScale = 0f; // �d�̓X�P�[��
    private bool isGrounded = true;// �n�ʂ𓥂�ł��邩�ǂ����̃t���O
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D�R���|�[�l���g���擾
        playerSprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = gravityScale; // Rigidbody2D�̏d�̓X�P�[����ݒ�
    }

    // Update is called once per frame
    void Update()
    {
        
       
       
    }

    private void FixedUpdate()
    {
        //�㉺�̓��͂��擾
        float verticalInput = Input.GetAxis("Vertical"); // �㉺�̓��͂��擾

        float moveInput = Input.GetAxis("Horizontal"); // ���E�̓��͂��擾
        rb.velocity = new Vector2(moveInput, verticalInput) * MoveSpeed * Time.deltaTime; // �㉺�����̑��x��ݒ�

        // ���E�̓��͂ɉ����ăL�����N�^�[�̌�����ύX
        if (moveInput != 0) playerSprite.flipX = moveInput < 0; // �������Ȃ�X�v���C�g�𔽓]
        int flipPoint = playerSprite.flipX ? -1 : 1; // �v���C���[�̌����ɉ����ăt���b�v�|�C���g��ݒ�
    }
}
