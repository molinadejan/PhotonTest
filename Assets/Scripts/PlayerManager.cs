using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Com.Molinadejan.TestGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        public float Health = 1f;

        [SerializeField]
        private GameObject beams;

        private bool isFiring;

        [SerializeField]
        private GameObject playerUIPrefab;

        public static GameObject LocalPlayerInstance;

        private void Awake()
        {
            if (beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            if(photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CameraWork cameraWork = gameObject.GetComponent<CameraWork>();

            if(cameraWork != null)
            {
                if(photonView.IsMine)
                {
                    cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.");
            }

            SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                CalledOnLevelWasLoaded(scene.buildIndex);
            };

            if(playerUIPrefab != null)
            {
                GameObject obj = Instantiate(playerUIPrefab);
                obj.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogError("Missing PlayerUIPrefab reference on player prefab");
            }
        }

        private void CalledOnLevelWasLoaded(int level)
        {
            if (this == null)
                return;

            if(!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject obj = Instantiate(playerUIPrefab);
            obj.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver); 
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                ProcessInput();
            }

            if(Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }

            if (beams != null && isFiring != beams.activeInHierarchy)
            {
                beams.SetActive(isFiring);
            }
        }

        private void ProcessInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (isFiring)
                {
                    isFiring = false;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
                return;

            if (!other.name.Contains("Beam"))
                return;

            Health -= 0.1f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
                return;

            if (!other.name.Contains("Beams"))
                return;

            Health -= 0.1f * Time.deltaTime;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(isFiring);
                stream.SendNext(Health);
            }
            else
            {
                isFiring = (bool)stream.ReceiveNext();
                Health = (float)stream.ReceiveNext();
            }
        }
    }
}
