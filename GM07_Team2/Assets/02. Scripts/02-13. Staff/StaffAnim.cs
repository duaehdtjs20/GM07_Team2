using GM07.Order;

using UnityEngine;

public class StaffAnim : MonoBehaviour
{
    [SerializeField]
    private TableOrderController _orderController;
    [SerializeField]
    private Animator _animator;

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
        foreach (OrderData order in  _orderController.Orders)
        {
            if (order.State == EOrderState.Cooking)
            {
                _animator.SetBool("Work", true);
                return;
            }
        }
        _animator.SetBool("Work", false);
    }
}
