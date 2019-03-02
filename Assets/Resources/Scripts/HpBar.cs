using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public Transform target;
    public Enemy owner;

    private TextMesh text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMesh>();
        target = owner.player;
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"{owner.hp}/{owner.maxHp}";
        transform.LookAt(transform.position + target.transform.rotation * Vector3.forward,
          target.transform.rotation * Vector3.up);
    }
}
