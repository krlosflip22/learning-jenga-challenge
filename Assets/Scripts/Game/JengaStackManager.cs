using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LearningJenga
{
    public class JengaStackManager : MonoBehaviour
    {
        #region Constants and Default Values
        const int STACK_DISTANCE = 20;
        const float CAMERA_CLOSEST_DISTANCE = 0.05f;
        const float LERP_TIME = 0.0025f;
        const float LERP_STEP = 0.01f;
        const float ROTATION_SPEED = 2f;
        Quaternion cameraStartRot = Quaternion.Euler(25, 0, 0);
        Vector3 cameraStartPos;
        string[] grades = new[] { "6th Grade", "7th Grade", "8th Grade" };
        #endregion

        #region Variables
        [SerializeField] JengaStack stackPrefab;
        [SerializeField] JengaBlock blockPrefab;
        [SerializeField] Material[] materials;
        [SerializeField] BlockUIView blockView;

        int selectedStackIndex = 0;
        List<JengaStack> stacks;
        #endregion

        #region Unity Functions
        void Start()
        {
            cameraStartPos = Camera.main.transform.position;

            stacks = new List<JengaStack>();

            API.RequestHandler.RequestData((result) =>
            {
                string newResult = "{\n\"data\":  \n" + result + "    \n}";
                var blocks = JsonUtility.FromJson<API.BlockList>(newResult).data.ToList();

                GenerateStacks(blocks);
            }, (error) => { Debug.LogError(error); });
        }

        float inputX;
        float rotationX;
        float inputY;
        float rotationY;
        float mouseSensivity = 3f;

        Vector3 currentRotation;
        void Update()
        {
            if (blockView.PopUpOpened) return;

            if (Input.GetMouseButton(0))
            {
                Cursor.visible = false;

                inputX = Input.GetAxis("Mouse X");
                inputY = Input.GetAxis("Mouse Y");

                if (Mathf.Abs(inputX) >= Mathf.Abs(inputY))
                {
                    Camera.main.transform.RotateAround(stacks[selectedStackIndex].transform.position, stacks[selectedStackIndex].transform.up, inputX * ROTATION_SPEED);
                }
                else
                {
                    Vector3 dir = (Camera.main.transform.eulerAngles.y % 180 < 135 && Camera.main.transform.eulerAngles.y % 180 > 45)
                        || (Camera.main.transform.eulerAngles.y % 180 > -135 && Camera.main.transform.eulerAngles.y % 180 < -45)
                        ? -stacks[selectedStackIndex].transform.forward : stacks[selectedStackIndex].transform.right;

                    Camera.main.transform.RotateAround(stacks[selectedStackIndex].transform.position, dir, -inputY * ROTATION_SPEED);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Cursor.visible = true;
            }
        }
        #endregion

        #region Jenga Stack Functions
        void GenerateStacks(List<API.Block> blocks)
        {
            int gradeCount = 0;

            foreach (string g in grades)
            {
                JengaStack jengaStack = (JengaStack)Instantiate(stackPrefab, transform);

                jengaStack.GetComponentInChildren<JengaLabel>().AddClickEvent(gradeCount, MoveCameraToPoint);

                jengaStack.name = g;
                jengaStack.transform.localPosition += Vector3.right * gradeCount * STACK_DISTANCE;

                var stackBlockList = blocks.FindAll(x => x.grade == g).OrderBy(x => x.domain).ThenBy(x => x.cluster).ThenBy(x => x.standardid);
                jengaStack.Initialize(stackBlockList, blockPrefab, materials, blockView.OpenPopUp);

                stacks.Add(jengaStack);

                if (gradeCount == 0) jengaStack.EnableCover(false);
                gradeCount++;
            }
        }

        public void TestMyStack()
        {
            stacks[selectedStackIndex].TestMyStack();
        }

        public void ResetStack()
        {
            stacks[selectedStackIndex].ResetStack();
        }
        #endregion

        #region Camera Movement Functions
        void MoveCameraToPoint(int index)
        {
            StartCoroutine(MoveCameraToPointRoutine(index));
        }

        IEnumerator MoveCameraToPointRoutine(int index)
        {
            stacks[selectedStackIndex].EnableCover(true);
            selectedStackIndex = index;
            stacks[selectedStackIndex].EnableCover(false);

            while (Mathf.Abs(Camera.main.transform.position.x - selectedStackIndex * STACK_DISTANCE) > CAMERA_CLOSEST_DISTANCE)
            {
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraStartPos + Vector3.right * selectedStackIndex * STACK_DISTANCE, LERP_STEP);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraStartRot, LERP_STEP);
                yield return new WaitForSeconds(LERP_TIME);
            }

            yield return null;
        }
        #endregion
    }
}
