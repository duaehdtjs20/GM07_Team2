using System.Collections;

using UnityEngine;

public class HostessAnim : MonoBehaviour
{
    [SerializeField]
    private Animator _anim;
    [SerializeField]
    private Vector3 _center;
    [SerializeField]
    private Vector3 _size;
    [SerializeField]
    private LayerMask _customerLayer;

    private bool _isGreeted = false;

    private void Awake()
    {
        if (_anim == null)
        {
            _anim = GetComponentInChildren<Animator>();

        }
    }
    //private void Update()
    //{
    //    if(Physics.OverlapBox(transform.position + _center, _size, Quaternion.identity, _customerLayer).Length > 0)
    //    {
    //        Greeting();
    //    }
    //}
    private void Greeting()
    {
        if (_isGreeted)
        {
            return;
        }
        StartCoroutine(GreetCo());
    }
    IEnumerator GreetCo()
    {
        _isGreeted = true;
        _anim.SetTrigger("Greeting");
        yield return new WaitForSeconds(2.0f);
        _isGreeted = false;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(transform.position + _center, _size);
    //}
}
