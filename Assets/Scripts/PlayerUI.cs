using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill;
    private PlayerController controller;
    public void SetController(PlayerController controller)
    {
        this.controller = controller;
    }
    void SetFuelAmount(float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }
    private void Update()
    {
        SetFuelAmount(controller.GetThrusterAmount());
    }
}
