using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipGame.Environment.Cannons
{
    public class Cannon : MonoBehaviour
    {
        [HideInInspector] public float reloadTime = 5;
        [HideInInspector] public GameObject cannonBall;
        [SerializeField] private GameObject shootVFX;

        [SerializeField] private bool onlyChildrenAsCanon = true;
        [SerializeField] private Vector3 openPosition;
        [SerializeField] private Vector3 closedPosition;

        [SerializeField, HideInInspector] private Transform[] cannons;
        [SerializeField, HideInInspector] private bool isOpen;
        [SerializeField, HideInInspector] private bool finishedOpen;
        [SerializeField, HideInInspector] private bool canShoot;
        private float timer;

        public bool CanShoot => canShoot;

        public Vector3 CannonPosition => openPosition;

        private void OnValidate() => LoadChildren();

        private void Start()
        {
            if (cannons == null || cannons.Length == 0)
                LoadChildren();

            transform.localPosition = closedPosition;
            isOpen = false;
            canShoot = true;
            finishedOpen = false;
        }

        private void Update()
        {
            timer += Time.deltaTime * 2;
            Vector3 a = isOpen ? closedPosition : openPosition;
            Vector3 b = isOpen ? openPosition : closedPosition;
            transform.localPosition = Vector3.Lerp(a, b, timer);

            if (isOpen && timer >= 1)
                finishedOpen = true;

            if (!GameManager.IsPlaying)
                HideCannon();
        }

        private void LoadChildren()
        {
            int size = transform.childCount;
            if (!onlyChildrenAsCanon) size++;

            List<Transform> aux = new List<Transform>();
            
            if (!onlyChildrenAsCanon)
                aux.Add(transform);
            for (int i = 0; i < transform.childCount; i++)
                aux.Add(transform.GetChild(i));

            cannons = aux.ToArray();
        }
    
        public void HideCannon()
        {
            if (!isOpen) return;

            finishedOpen = false;
            isOpen = false;
            timer = 0;
        }

        public void ShowCannon()
        {
            if (isOpen || !canShoot || !GameManager.IsPlaying) return;

            isOpen = true;
            timer = 0;
        }
    
        public void Shoot()
        {
            if (!finishedOpen || !canShoot) return;
            foreach (var cannon in cannons)
            {
                GameObject ball = Instantiate(cannonBall, cannon);
                ball.transform.localPosition = new Vector3(.5f, 0, 0);
                ball.transform.localEulerAngles = new Vector3(0, 0, -90);
                ball.transform.parent = null;

                GameObject fx = Instantiate(shootVFX, ball.transform.position, Quaternion.identity);

                Destroy(ball, 5);
                Destroy(fx, 1);
            }

            canShoot = false;
            HideCannon();
            StartCoroutine(Reload());
        }

        private IEnumerator Reload()
        {
            yield return new WaitForSeconds(reloadTime);
            canShoot = true;
        }
    }
}