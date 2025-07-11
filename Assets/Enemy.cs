using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 2.0f; // �G�̈ړ����x
    [SerializeField] private MoveType moveType = MoveType.Vertical; // �ړ��^�C�v�̑I��
    [SerializeField] private float moveRange = 5.0f; // �ړ��͈́iTracking�ȊO�Ŏg�p�j
    private Rigidbody2D rb;
    private SpriteRenderer enemySprite;
    public float gravityScale = 2f; // �d�̓X�P�[��
    private bool isGrounded = true;// �n�ʂ𓥂�ł��邩�ǂ����̃t���O
    public enum MoveType
    {
        Vertical,
        Horizontal,
        Tracking,
    }
   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D�R���|�[�l���g���擾
        enemySprite = GetComponent<SpriteRenderer>();
        rb.gravityScale = gravityScale; // Rigidbody2D�̏d�̓X�P�[����ݒ�
    }

    // Update is called once per frame
    void Update()
    {
        //switch����velocity���g�����ړ�����
        switch (moveType)
        {
            case MoveType.Vertical:
                //moveRange�̕������ړ�
                rb.velocity = new Vector2(rb.velocity.x, MoveSpeed);
                break;
            case MoveType.Horizontal:
                rb.velocity = new Vector2(MoveSpeed, rb.velocity.y);
                break;
            case MoveType.Tracking:
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Vector2 direction = (player.transform.position - transform.position).normalized;
                    rb.velocity = direction * MoveSpeed;
                }
                break;
        }
        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            Debug.Log("Enemy hit by attack!"); // �U���ɓ��������烍�O���o��
            Destroy(gameObject); // �U���ɓ���������G���폜
        }
    }
}
