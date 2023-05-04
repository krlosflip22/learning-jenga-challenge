using UnityEngine;

namespace LearningJenga
{
    public class JengaLabel : MonoBehaviour
    {
        #region Variables and Actions
        event System.Action<int> onClick;
        int stackIndex;
        #endregion

        #region Setup
        public void AddClickEvent(int index, System.Action<int> evt)
        {
            stackIndex = index;
            onClick += evt;
        }
        #endregion

        #region Unity Functions
        public void OnMouseDown()
        {
            onClick?.Invoke(stackIndex);
        }

        public void OnMouseEnter()
        {
            transform.localScale = 1.25f * Vector3.one;
        }

        public void OnMouseExit()
        {
            transform.localScale = Vector3.one;
        }

        void OnDestroy()
        {
            onClick = null;
        }
        #endregion
    }
}
