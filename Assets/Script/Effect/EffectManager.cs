using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private PlayerController pl;

    [Header("DisplayTime")]
    [SerializeField] private float DisplayTime;

    [Header("Scale")]
    [SerializeField] private float scaleGrowUpSpeed;
    [SerializeField] private float scaleLimit;

    [Header("Angle")]
    [SerializeField] private float endAngle;
    [SerializeField] private float rotateSpeed;

    [Header("transparency")]
    [SerializeField] private float startTransparentTime;
    [SerializeField] private float transparentSpeed;

    private float timer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        PlayDust();
    }

    public void PlayDust()
    {
        //オブジェクトの表示
        timer += Time.fixedDeltaTime;

        if (DisplayTime > timer)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        //scaleを大きく
        if (transform.localScale.x < scaleLimit)
        {
            transform.localScale *= scaleGrowUpSpeed;
        }

        //オブジェクトの回転
        if (endAngle >= 0)
        {
            if (transform.eulerAngles.z < endAngle)
            {
                transform.Rotate(0f, 0f, rotateSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            if (transform.eulerAngles.z == 0)
            {
                transform.Rotate(0f, 0f, -0.01f);
            }
            else if (transform.eulerAngles.z > endAngle + 360f)
            {
                transform.Rotate(0f, 0f, -rotateSpeed * Time.fixedDeltaTime);
            }
        }

        //透過開始
        if (startTransparentTime < timer)
        {
            Color color = spriteRenderer.color;
            color.a -= transparentSpeed * Time.fixedDeltaTime;
            spriteRenderer.color = color;
        }
    }
}
