using UnityEngine;
using System.Collections;

namespace DN{
    public class CharacterController : MonoBehaviour {

        public CharacterController cc;
        public float inputDelay = .01f;
        public float ForwardVel = 12;
        public float rotateVel = 100;
        Quaternion targetRotation;
        Rigidbody rbody;
        float forwardInput, SideWaysInput, turnInput;
        public Quaternion TargetRotation
        {
            get { return targetRotation; }
        }

        void Start()
        {
            targetRotation = transform.rotation;
            if (GetComponent<Rigidbody>())
                rbody = GetComponent<Rigidbody>();
            else
                Debug.Log("This characther needs a rigidbody");
            forwardInput = turnInput = 0;
        }
        void GetInput()
        {
            //W And S keys also LYAxis
            forwardInput = Input.GetAxis("Vertical");
            //forwardInput = Input.GetAxis("LYAxis");

            //A And D keys also LXAxis
            //SideWaysInput = Input.GetAxis("Horizontal");
            //SideWaysInput = Input.GetAxis("LXAxis");
            //MouseInput keys also lXAxis Will change later
            turnInput = Input.GetAxis("Horizontal");
            //turnInput = Input.GetAxis("RXAxis");

        }
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Debug.Log("i hit something");
        }

        // Update is called once per frame
        void Update() {
            GetInput();
            Turn();
        }
        void FixedUpdate()
        {
            Run();
        }
        void Run()
        {
            if (Mathf.Abs(forwardInput) > inputDelay)
            {
                //move
                rbody.velocity = transform.forward * forwardInput * ForwardVel;

            }
            /* else if (Mathf.Abs(SideWaysInput) > inputDelay)
             {
                 rbody.velocity = transform.right * SideWaysInput * ForwardVel;
             }*/
            else
                rbody.velocity = Vector3.zero;
        }
        void Turn()
        {
            if (Mathf.Abs(turnInput) > inputDelay)
            {
                targetRotation *= Quaternion.AngleAxis(rotateVel * turnInput * Time.deltaTime, Vector3.up);
            }

            transform.rotation = targetRotation;
        }
    }
}