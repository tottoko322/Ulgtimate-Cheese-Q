using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private PlayerController pl;
    [SerializeField] private GameObject player;

    [Header("Position")]
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

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
        CommonProcess();
        PlayDust();
    }

    public void CommonProcess()
    {
        //オブジェクトの位置
        transform.position = new Vector2(player.transform.position.x + xOffset, player.transform.position.y + yOffset);

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
            color.a = Mathf.MoveTowards(spriteRenderer.color.a, 0f, transparentSpeed);
            spriteRenderer.color = color;
        }
    }

    public void PlayDust()
    {
        
    }
}
