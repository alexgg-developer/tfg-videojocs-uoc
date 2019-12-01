using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField]
    UnitController unitController;
    [SerializeField]
    CityController cityController;

    int currentPlayerID = 0;
    float clicked = 0;
    float clicktime = 0;
    float clickdelay = 0.5f;
    bool isDoubleClick = false;
    Logic logic;

    // Start is called before the first frame update
    void Start()
    {
        logic = this.gameObject.GetComponent<Logic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (logic.IsEndOfGame) return;

        if (Input.GetKeyUp(KeyCode.B)) {
            unitController.SettlerCommand();
        }


        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (Input.GetMouseButtonDown(0)) {
                clicked++;
                if (clicked == 1) {
                    clicktime = Time.time;
                }
                else if (clicked > 1 && Time.time - clicktime < clickdelay) {
                    clicked = 0;
                    clicktime = 0;
                    isDoubleClick = true;
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                if (!isDoubleClick) {
                    unitController.Select();
                    cityController.Unselect();
                }
                else {
                    cityController.Select();
                    unitController.Unselect();
                }

                isDoubleClick = false;
            }
            else if (Input.GetMouseButtonUp(1)) {
                unitController.Move();
            }
        }


        if (Time.time - clicktime >= clickdelay) {
            clicked = 0;
            clicktime = 0;
        }
    }
}
