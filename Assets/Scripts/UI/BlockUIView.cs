using UnityEngine;
using TMPro;

namespace LearningJenga
{
    public class BlockUIView : MonoBehaviour
    {
        JengaBlock currentHit;

        [SerializeField] GameObject popUp;
        [SerializeField] TextMeshProUGUI descriptionLabel;

        bool popUpOpened = false;
        public bool PopUpOpened => popUpOpened;

        void FixedUpdate()
        {
            if (!popUpOpened)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.transform.GetComponent<JengaBlock>() != null)
                    {
                        if (hit.transform.gameObject != currentHit)
                        {
                            if (currentHit != null) currentHit.EnableMaterialEmission(false);
                            currentHit = hit.transform.GetComponent<JengaBlock>();
                            currentHit.EnableMaterialEmission(true);
                        }
                    }
                    else
                    {
                        if (currentHit != null) currentHit.EnableMaterialEmission(false);
                        currentHit = null;
                    }
                }
                else
                {
                    if (currentHit != null) currentHit.EnableMaterialEmission(false);
                    currentHit = null;
                }
            }
        }

        public void OpenPopUp(JengaBlock block)
        {
            popUpOpened = true;
            descriptionLabel.text = block.Description;
            currentHit.EnableMaterialEmission(false);
            currentHit = null;
            popUp.SetActive(true);
        }

        public void ClosePopUp()
        {
            popUpOpened = false;
            popUp.SetActive(false);
        }
    }
}
