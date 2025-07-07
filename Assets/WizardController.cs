using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wizardCon : MonoBehaviour
{
    [SerializeField] private bool Jumpflag;
    [SerializeField] private float JumpSpeed = 5.0f;
    [SerializeField] private float MoveSpeed = 5.5f;
    private int jumpcount = 0;
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public float gravityScale = 2f; // �d�̓X�P�[��
    private bool isGrounded = true;// �n�ʂ𓥂�ł��邩�ǂ����̃t���O

    //���@�̒e�̃v���n�u
    [SerializeField] private GameObject magicPrefab;
    //���@�̒e�̔��ˈʒu
    [SerializeField] private Transform magicPoint;
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
        //Velocity��p�����ړ�����
        float moveInput = Input.GetAxis("Horizontal"); // ���E�̓��͂��擾
        rb.velocity = new Vector2(moveInput * MoveSpeed, rb.velocity.y); // ���������̑��x��ݒ�
        // ���E�̓��͂ɉ����ăL�����N�^�[�̌�����ύX
        if(moveInput != 0) playerSprite.flipX = moveInput < 0; // �������Ȃ�X�v���C�g�𔽓]
        int flipPoint = playerSprite.flipX ? -1 : 1; // �v���C���[�̌����ɉ����ăt���b�v�|�C���g��ݒ�
        magicPoint.localPosition = new Vector2(flipPoint * Mathf.Abs(magicPoint.localPosition.x), magicPoint.localPosition.y);

        // �W�����v����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            isGrounded = false; // �W�����v�����̂Œn�ʂ𓥂�ł��Ȃ�
            jumpcount++;
        }

        // ���@�̒e�𔭎˂��鏈��
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(magicPrefab);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // �n�ʂɐڐG�����̂Œn�ʂ𓥂�ł���
            jumpcount = 0; // �W�����v�J�E���g�����Z�b�g
        }
    }
    private void Shoot(GameObject magicPrefab)
    {
        // ���@�̒e�𐶐�
        GameObject magic = Instantiate(magicPrefab, magicPoint.position, Quaternion.identity);
        Rigidbody2D magicRb = magic.GetComponent<Rigidbody2D>();
        SpriteRenderer sprite = magic.GetComponent<SpriteRenderer>(); // �X�v���C�g���擾�i�K�v�ɉ����āj
        sprite.flipX = playerSprite.flipX; // �v���C���[�̌����ɉ����ăX�v���C�g�𔽓]
        if (magicRb != null)
        {
            // ���@�̒e�ɗ͂�������

            float direction = playerSprite.flipX ? -1: 1; // �v���C���[�̌����ɉ����ĕ���������
            //�e���ˏo
            magicRb.velocity = new Vector2(direction * 10f, 0f); // �v���C���[�̌����ɉ����Ĕ���

            //magicRb.velocity = new Vector2(10f, 0f); // �E�����ɔ���
        }
    }
}
