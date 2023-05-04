using System;
using TMPro;
using UnityEngine;

namespace LearningJenga
{
    public class JengaBlock : MonoBehaviour
    {
        #region Constant and Enums
        const string EMISSION_KEYWORKD = "_EMISSION";
        enum MasteryType { Learned = 1, Mastered = 2 }
        enum MasteryMaterial { Glass = 0, Wood = 1, Stone = 2 }
        #endregion

        #region Variables and Actions
        [SerializeField] TextMeshPro label;
        event Action<JengaBlock> onClick;
        Renderer renderer;
        #endregion

        #region Block Variables
        private API.Block block;
        public int ID => block.id;
        public int Mastery => block.mastery;
        public string Description => $"{block.grade} : {block.domain}\n\n{block.cluster}";
        #endregion

        #region Unity Functions
        void Awake()
        {
            renderer = GetComponent<Renderer>();
        }

        public void OnMouseDown()
        {
            onClick?.Invoke(this);
        }

        void OnDestroy()
        {
            onClick = null;
        }
        #endregion

        #region Setup
        public void Initialize(API.Block block, Material mat, Action<JengaBlock> onClick)
        {
            this.block = block;
            renderer.material = mat;

            name = $"{block.grade}_{block.id}_{((MasteryMaterial)block.mastery).ToString()}";

            if (block.mastery == 0) label.gameObject.SetActive(false);
            else label.text = ((MasteryType)block.mastery).ToString();

            this.onClick += onClick;
        }
        #endregion

        #region Other Functions
        public void EnableMaterialEmission(bool active)
        {
            if (active) renderer.material.EnableKeyword(EMISSION_KEYWORKD);
            else renderer.material.DisableKeyword(EMISSION_KEYWORKD);
        }
        #endregion
    }
}
