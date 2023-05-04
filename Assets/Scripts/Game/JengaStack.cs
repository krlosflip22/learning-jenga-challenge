using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;
using System;

namespace LearningJenga
{
    public class JengaStack : MonoBehaviour
    {
        #region Constants
        const float LERP_TIME = 0.0025f;
        const float LERP_STEP = 0.01f;
        #endregion

        #region Variables
        [SerializeField] TextMeshPro label;
        [SerializeField] GameObject cover;
        List<JengaBlock> blocks;
        #endregion

        #region Glass Material Variables
        int glassCount = 0;
        int glassDissapeared = 0;
        Color glassTransparentColor;
        Color glassColor;
        #endregion

        #region Setup
        public void Initialize(IOrderedEnumerable<API.Block> dataBlocks, JengaBlock blockPrefab, Material[] materials, Action<JengaBlock> onBlockClick)
        {
            label.text = name;

            glassColor = materials[0].color;
            glassTransparentColor = new Color(materials[0].color.r, materials[0].color.g, materials[0].color.b, 0);

            blocks = new List<JengaBlock>();

            for (int blockCount = 0; blockCount < dataBlocks.Count(); blockCount++)
            {
                JengaBlock jb = (JengaBlock)Instantiate(blockPrefab, transform);
                blocks.Add(jb);

                if (dataBlocks.ElementAt(blockCount).mastery == 0) glassCount++;

                jb.Initialize(dataBlocks.ElementAt(blockCount), materials[dataBlocks.ElementAt(blockCount).mastery], onBlockClick);

                SetBlockPosRot(jb, blockCount);
            }
        }
        #endregion

        #region Jenga Stack Functions
        public void ResetStack()
        {
            for (int blockCount = 0; blockCount < blocks.Count; blockCount++)
            {
                blocks[blockCount].GetComponent<Rigidbody>().isKinematic = true;
                if (blocks[blockCount].Mastery == 0)
                {
                    blocks[blockCount].gameObject.SetActive(true);
                    blocks[blockCount].GetComponent<Renderer>().material.color = glassColor;
                }

                SetBlockPosRot(blocks[blockCount], blockCount);
            }
        }

        public void TestMyStack()
        {
            glassDissapeared = 0;
            foreach (JengaBlock b in blocks.FindAll(x => x.Mastery == 0))
            {
                StartCoroutine(GlassDissapearRoutine(b.GetComponent<Renderer>()));
            }
        }
        #endregion

        #region Test My Stack Functions
        IEnumerator GlassDissapearRoutine(Renderer r)
        {
            while (r.material.color.a > 0.01f)
            {
                r.material.color = Color.Lerp(r.material.color, glassTransparentColor, LERP_STEP);
                yield return new WaitForSeconds(LERP_TIME);
            }

            glassDissapeared++;
            r.gameObject.SetActive(false);

            if (glassDissapeared == glassCount)
            {
                EnableStackGravity();
            }

            yield return null;
        }

        public void EnableStackGravity()
        {
            foreach (JengaBlock b in blocks)
            {
                b.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
        #endregion

        #region Reset Stack Functions
        void SetBlockPosRot(JengaBlock jb, int blockCount)
        {
            int rot = blockCount % 6 < 3 ? 0 : -90;
            int currX = blockCount % 3 == 0 ? -3 : blockCount % 3 == 1 ? 0 : 3;
            jb.transform.localPosition = rot == 0 ? new Vector3(currX, 0, 0) : new Vector3(0, 0, currX);
            jb.transform.localPosition += Vector3.up * 1.5f * (blockCount / 3);
            jb.transform.localRotation = Quaternion.Euler(0, rot, 0);
        }
        #endregion

        #region Other Functions
        public void EnableCover(bool active)
        {
            cover.SetActive(active);
        }
        #endregion
    }
}
