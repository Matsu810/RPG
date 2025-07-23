using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoliceCon : MonoBehaviour
{
    [SerializeField] private bool Jumpflag;
    [SerializeField] private float JumpSpeed = 5.0f;
    [SerializeField] private float MoveSpeed = 5.5f;
    private int jumpcount = 0;
    private Rigidbody rb;
    private SpriteRenderer playerSprite;
    public float gravityScale = 0f; // �d�̓X�P�[��
    private bool isGrounded = true;// �n�ʂ𓥂�ł��邩�ǂ����̃t���O
    private bool isAttack = false; // �U�������ǂ����̃t���O
    private float elapsedTime = 0f; // �o�ߎ���

    private const float AttackDuration = 0.25f; // �U���̎�������

    //�U�����镐��̃v���n�u
    [SerializeField] private GameObject attackPrefab;
    //�U���ʒu
    [SerializeField] private Transform AttackPoint;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody2D�R���|�[�l���g���擾
        playerSprite = GetComponent<SpriteRenderer>();
       // rb.gravityScale = gravityScale; // Rigidbody2D�̏d�̓X�P�[����ݒ�
    }

    // Update is called once per frame
    void Update()
    {
        // �W�����v����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
            isGrounded = false; // �W�����v�����̂Œn�ʂ𓥂�ł��Ȃ�
            jumpcount++;
        }
        // �U�����鏈��
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot(attackPrefab);
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // �n�ʂɐڐG������n�ʂ𓥂�ł���
            jumpcount = 0; // �W�����v�J�E���g�����Z�b�g
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by enemy!"); // �G�ɓ��������烍�O���o��
        }
    }
    private void FixedUpdate()
    {
        if (isAttack)
        {
            elapsedTime += Time.deltaTime; // �o�ߎ��Ԃ��X�V
            if (elapsedTime >= AttackDuration)
            {
                isAttack = false; // �U�����I��
                elapsedTime = 0f; // �o�ߎ��Ԃ����Z�b�g
            }
            return;
        }

        //�㉺�̓��͂��擾
        float verticalInput = Input.GetAxis("Vertical"); // �㉺�̓��͂��擾

        float moveInput = Input.GetAxis("Horizontal"); // ���E�̓��͂��擾
        rb.velocity = new Vector2(moveInput, verticalInput) * MoveSpeed * Time.deltaTime; // �㉺�����̑��x��ݒ�

        // ���E�̓��͂ɉ����ăL�����N�^�[�̌�����ύX
        if (moveInput != 0) playerSprite.flipX = moveInput < 0; // �������Ȃ�X�v���C�g�𔽓]
        int flipPoint = playerSprite.flipX ? -1 : 1; // �v���C���[�̌����ɉ����ăt���b�v�|�C���g��ݒ�
        AttackPoint.localPosition = new Vector2(flipPoint * Mathf.Abs(AttackPoint.localPosition.x), AttackPoint.localPosition.y);
    }
    private void Shoot(GameObject attackPrefab)
    {
        // �U�����镐��𐶐�
        GameObject attack = Instantiate(attackPrefab, AttackPoint.position, Quaternion.identity);
        // �U�����͑���s��
        rb.velocity = Vector2.zero; // �U�����͈ړ����~
        isAttack = true; // �U�����t���O�𗧂Ă�

        SpriteRenderer sprite = attack.GetComponent<SpriteRenderer>(); // �X�v���C�g���擾�i�K�v�ɉ����āj
        sprite.flipX = playerSprite.flipX; // �v���C���[�̌����ɉ����ăX�v���C�g�𔽓]
        //0.5�b��ɍU�����폜
        Destroy(attack, AttackDuration);

    }
}
