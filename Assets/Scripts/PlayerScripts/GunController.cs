using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    private bool facingRight = false;
    private GameObject body;
    private SpriteRenderer sr;
    private bool handedGun = true;

    public Sprite handedSprite;
    public Sprite unhandedSprite;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        body = transform.parent.gameObject;
    }

    void Update()
    {

        RotationHandler();
    }

    void RotationHandler()
    {
        Vector2 direction = CreateDirectionVector(Input.mousePosition);

        float angle = Mathf.Atan2(direction.y, direction.x);
        float rotationZ = Mathf.Rad2Deg * angle;

        HandleSlide(angle);
        Quaternion rotation = Quaternion.Euler(180, 180, rotationZ);

        transform.rotation = rotation;

    }

    void HandleSlide(float degrees)
    {
        float cosin = Mathf.Cos(degrees);

        if (cosin < 0 && facingRight)
        {
            FlipCharacter();
            facingRight = false;
        }
        else if (cosin > 0 && !facingRight)
        {
            FlipCharacter();
            facingRight = true;
        }

        if (math.abs(cosin) > 0.7 && handedGun)
        {
            handedGun = false;
            sr.sprite = unhandedSprite;
        }
        else if (math.abs(cosin) < 0.7 && facingRight)
        {
            handedGun = true;
            sr.sprite = handedSprite;
        }
   }

    void FlipCharacter()
    {
        Vector3 euler = body.transform.eulerAngles;
        euler.y = euler.y + 180f;
        body.transform.eulerAngles = euler;

        sr.flipY = !sr.flipY;
    }

    Vector2 CreateDirectionVector(Vector2 mousePos)
    {
        Vector3 mouseToWorld = Camera.main.ScreenToWorldPoint(mousePos);
        mouseToWorld.z = 0;
        return ((Vector2)mouseToWorld - (Vector2)transform.position).normalized;
    }
}
