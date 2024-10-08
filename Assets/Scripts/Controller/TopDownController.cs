using System;
using UnityEngine;

namespace UnityPractice.Controller
{
    [RequireComponent(typeof(Rigidbody))]
    public class TopDownController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        
        private Camera _mainCamera;
        private Rigidbody _rigidbody;
        private Vector3 _velocity;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            UpdateLookPosition();
            CalculateVelocity();
        }
        private void FixedUpdate()
        {
            UpdateRigidbody();
        }
        
        private void UpdateLookPosition()
        {
            var ray = _mainCamera.ScreenPointToRay (Input.mousePosition);
            var groundPlane = new Plane (Vector3.up, Vector3.zero);

            if (!groundPlane.Raycast(ray, out var rayDistance)) return;
            var point = ray.GetPoint(rayDistance);
            var heightCorrectedPoint = new Vector3 (point.x, transform.position.y, point.z);
            transform.LookAt(heightCorrectedPoint);
        }
        
        private void CalculateVelocity()
        {
            var input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            var direction = input.normalized;
            _velocity = direction * moveSpeed;
        }

        private void UpdateRigidbody()
        {
            _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
        }
    }
}
