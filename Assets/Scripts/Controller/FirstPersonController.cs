using UnityEngine;

namespace UnityPractice.Controller
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3;
        [SerializeField] private float smoothMoveTime = 0.1f;
        [SerializeField] private float mouseSensitivity = 10;
        [SerializeField] private Vector2 pitchMinMax = new Vector2 (-40, 85);
        [SerializeField] private float rotationSmoothTime = 0.1f;

        [SerializeField] private bool isAlwaysOnGround = true;

        private CharacterController _controller;
        private Transform _cam;
        private float _yaw;
        private float _pitch;
        private float _smoothYaw;
        private float _smoothPitch;
        private float _yawSmoothV;
        private float _pitchSmoothV;
        private Vector3 _velocity;
        private Vector3 _smoothV;
        private float _timer;
        private void Awake ()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            InitController();
        }

        private void Update()
        {
            Move();
        }

        private void InitController()
        {
            _cam = transform.GetChild(0);
            _controller = GetComponent<CharacterController> ();
            _yaw = transform.eulerAngles.y;
            if (_cam != null) _pitch = _cam.transform.localEulerAngles.x;
            _smoothYaw = _yaw;
            _smoothPitch = _pitch;
            if (isAlwaysOnGround)
                _controller.stepOffset = 0f;
        }
        
        private void Move ()
        {
            UpdateEyeAngles();
            UpdatePosition();
        }
        
        private void UpdatePosition ()
        {
            var input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
            
            var inputDir = new Vector3 (input.x, 0, input.y).normalized;
            var worldInputDir = transform.TransformDirection (inputDir);

            var targetVelocity = worldInputDir * moveSpeed;
            _velocity = Vector3.SmoothDamp (_velocity, targetVelocity, ref _smoothV, smoothMoveTime);
            _controller.Move (_velocity * Time.deltaTime);
            if(isAlwaysOnGround)
                MakeSureCharacterOnTheGround();
        }
        
        private void UpdateEyeAngles ()
        {
            var mX = Input.GetAxisRaw ("Mouse X");
            var mY = Input.GetAxisRaw ("Mouse Y");

            var mMag = Mathf.Sqrt (mX * mX + mY * mY);
            if (mMag > 5) {
                mX = 0;
                mY = 0;
            }

            _yaw += mX * mouseSensitivity;
            _pitch -= mY * mouseSensitivity;
            _pitch = Mathf.Clamp (_pitch, pitchMinMax.x, pitchMinMax.y);
            _smoothPitch = Mathf.SmoothDampAngle (_smoothPitch, _pitch, ref _pitchSmoothV, rotationSmoothTime);
            _smoothYaw = Mathf.SmoothDampAngle (_smoothYaw, _yaw, ref _yawSmoothV, rotationSmoothTime);

            transform.eulerAngles = Vector3.up * _smoothYaw;
            _cam.transform.localEulerAngles = Vector3.right * _smoothPitch;
        }

        private void MakeSureCharacterOnTheGround()
        {
            transform.position = new Vector3 (transform.position.x, 0f , transform.position.z);
        }
    }
}
