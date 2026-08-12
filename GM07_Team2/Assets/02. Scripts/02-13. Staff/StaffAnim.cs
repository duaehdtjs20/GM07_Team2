using GM07.Order;

using UnityEngine;

public class StaffAnim : MonoBehaviour
{
    [SerializeField]
    private TableOrderController _orderController;
    [SerializeField]
    private Animator _animator;

    private bool _isCooking = false;

    void Awake()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        ChangeState();
    }
    private void ChangeState()
    {
        if (_orderController == null)
        {
            return;
        }
        if (_isCooking == _orderController.IsCooking)
        {
            return;
        }
        _isCooking = _orderController.IsCooking;
        _animator.SetBool("Work", _isCooking);
    }
}
