using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


// TODO : �f�o�b�O����Quad���f���ł͂Ȃ�����C��
public class MinicharaBillboard : MonoBehaviour
{
    // �~�j�L�����̃}�e���A���擾�pGameObject
    [SerializeField] private GameObject CharaMate = default;

    // �\������e�N�X�`���i�[�p
    [Tooltip("�\������e�N�X�`��")]
    [SerializeField] private Texture[] spriteImage;

    // �e�N�X�`������p
    private bool isFront = true;

    // ���Ԃ��X�s�[�h
    [Tooltip("���Ԃ��X�s�[�h")]
    [SerializeField] private float speed = 1.0f;

    // �J�n���̃X�P�[��
    // ���]�������J�n����Ƃ��̑傫��
    private Vector3 startScale = new Vector3(1.0f, 1.0f, 1.0f);

    // ���Ԓn�_�̃X�P�[�� (x = 0)
    // x��0�Ȃ̂Ō����Ȃ��i���]���̑傫���j
    private Vector3 endScale = new Vector3(0f, 1.0f, 1.0f);

    // Scale�i�[�p�̑傫��
    private Vector3 localScale = new Vector3();

    // �⊮�p�ϐ�(0f�`1f�̊�)
    private float tick;

    // �q�I�u�W�F�N�g�̉�]
    Quaternion childRotation;

    // y���A�j���[�V�����̓��B�n�_
    [Tooltip("y���A�j���[�V�����̓��B�n�_")]
    [SerializeField] private float arrivalPositionY = -0.5f;

    // arrivalPositionY�܂ł̈ړ�����
    [Tooltip("���Ԃ��܂ł̎���")]
    [SerializeField] private float arrivalTime = 2f;



    /// <summary>
    /// Awake����
    /// </summary>
    private void Awake()
    {
        // ��]�̏����l�ݒ�
        StartYRotation();
    }

    /// <summary>
    /// �J�n����
    /// </summary>
    private void Start()
    {
        // �ŏ��ɕ\������e�N�X�`���̐ݒ�
        CharaMate.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", spriteImage[0]);

        StartCoroutine(Turn());
    }

    /// <summary>
    /// �X�V����
    /// </summary>
    void Update()
    {
        // �r���{�[�h����
        // ���C���J�����̈ʒu�̎擾
        Vector3 p = Camera.main.transform.position;
        p.y = transform.position.y;
        transform.LookAt(p);
    }


    /// <summary>
    /// doTween�A�j���[�V����(Y���̂�)
    /// </summary>
    /// <param name="arrivalPositionY">���B�ʒu</param>
    /// <param name="duration">���B����܂ł̎���</param>
    private void DoTweenAnimMiniChara(float arrivalPositionY, float duration)
    {
        // y���݂̂̃A�j���[�V����
        transform.DOLocalMoveY(arrivalPositionY, duration).
            SetEase(Ease.OutBounce).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// Sprite�𔽓]������֐�
    /// �X�P�[����x��b���ŕς��Ă���
    /// </summary>
    /// <returns></returns>
    IEnumerator Turn()
    {
        // �g�D�[���A�j���[�V�����J�n
        DoTweenAnimMiniChara(arrivalPositionY, arrivalTime);

        // �������[�v
        while (true)
        {
            yield return new WaitForSeconds(2f);
            tick = 0f;
            // (1/speed)�b�Œ��Ԓn�_�܂łЂ�����Ԃ�
            while (tick < 1.0f)
            {
                tick += Time.deltaTime * speed;
                // ���`���
                localScale = Vector3.Lerp(startScale, endScale, tick);

                CharaMate.transform.localScale = localScale;

                yield return null;
            }

            // ���\��ς���
            isFront = !isFront;

            // sprite��ς���
            CahngeTexture();

            // �⊮�l�̏�����
            tick = 0f;

            // (1/speed)�b�Œ��Ԃ���Ō�܂łЂ�����Ԃ�
            while (tick < 1.0f)
            {
                tick += Time.deltaTime * speed;

                localScale = Vector3.Lerp(endScale, startScale, tick);

                CharaMate.transform.localScale = localScale;

                yield return null;
            }
        }
    }

    /// <summary>
    /// �e�N�X�`����؂�ւ���֐��i�}�e���A���̃e�N�X�`����ς��Ă��邾���j
    /// </summary>
    private void CahngeTexture()
    {
        if (isFront == true)
        {
            CharaMate.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", spriteImage[0]);
        }
        else
        {
            CharaMate.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", spriteImage[1]);
        }
    }

    /// <summary>
    /// �q�I�u�W�F�N�gRotation��y����-180�ɂ���
    /// TODO:�\�ł����Quad����ύX�����̏���������
    /// </summary>
    private void StartYRotation()
    {
        // �q�I�u�W�F�N�g�̉�]���擾
        childRotation = CharaMate.transform.rotation;

        childRotation.y = -180f;

        // ���ۂ̎q�I�u�W�F�N�g�̉�]�ɔ��f
        CharaMate.transform.rotation = childRotation;
    }
}
