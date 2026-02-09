using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;
/**
 * 플레이어 캐릭터의 공격, 스킬 사용과 다른 클라이언트의 캐릭터와의 상호작용을 구현합니다.
 */
public class Player : Character
{
    public bool isSkillCoolTime { get; private set; } = false;
    public HitPoints hitPoints;
    public GameObject coinItem;
    [HideInInspector] public Inventory inventory;

    #region private memeber
    private int shootedAmmoCnt = 0;
    private HealthBar healthBar;
    private GameObject[] aim;
    private GameObject[] uiCanvas;
    private GameObject[] deadCanvas;
    private Text bulletTextUI;
    private Text coolTimeTextUI;
    private Image skillCoolImage;
    private BulletPooling bulletPooling;
    private float reloadingTime = 0.0f;
    private bool invading = false;
    private bool attacked = false;
    private bool barriering = false;
    private bool reloading = false;
    private float barrierTime = 0.0f;
    private float barrierCooltime = 0.0f;
    private Color color;
    private SpriteRenderer spren;
    private MovementController movementController;
    private PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject barrierEffect;
    private GameObject reloadEffect;
    private GameObject crownEffect;
    private GameObject attackedUI;
    private GameObject invadedUI;
    private GameObject coolTimeTextGameObject;
    private GameObject skillCoolImageGameObject;
    private RPGGameManager rpgGameManager;
    [SerializeField] private Inventory inventoryPrefab;
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private GameObject ammo;
    #endregion

    public void Start()
    {
        #region  Initialize
        pv = GetComponent<PhotonView>();
        bulletPooling = GetComponent<BulletPooling>();
        aim = GameObject.FindGameObjectsWithTag("aim");
        uiCanvas = GameObject.FindGameObjectsWithTag("UI");
        spren = GetComponent<SpriteRenderer>();
        deadCanvas = GameObject.FindGameObjectsWithTag("deadcanvas");
        bulletTextUI = uiCanvas[0].transform.GetChild(4).gameObject.GetComponent<Text>();
        barrierEffect = transform.GetChild(3).gameObject;
        reloadEffect = transform.GetChild(4).gameObject;
        skillCoolImageGameObject = uiCanvas[0].transform.GetChild(6).gameObject;
        coolTimeTextGameObject = uiCanvas[0].transform.GetChild(7).gameObject;
        skillCoolImage = skillCoolImageGameObject.GetComponent<Image>();
        coolTimeTextUI = coolTimeTextGameObject.GetComponent<Text>();
        movementController = GetComponent<MovementController>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        attackedUI=uiCanvas[0].transform.GetChild(8).gameObject;
        invadedUI=uiCanvas[0].transform.GetChild(9).gameObject;
        rpgGameManager = GameObject.Find("RPGgamemanager").transform.gameObject.GetComponent<RPGGameManager>();
        #endregion Initialize
        //이 플레이어가 현재 클라이언트의 플레이어거나, 오프라인 플레이어라면 인벤토리와 HP바를 설정합니다.
        if (pv.IsMine || PhotonNetwork.CurrentRoom == null)
        {
            inventory = Instantiate(inventoryPrefab);
            healthBar = Instantiate(healthBarPrefab);
            healthBar.character = this;
            hitPoints.value = startingHitPoints;
            //게임매니저로부터 HP와 코인의 수를 가져와서 초기화합니다.
            Set(rpgGameManager.hitpoints, rpgGameManager.quantity);
        }
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            hitPoints.value = startingHitPoints;
        }
        //이 플레이어가 다른 클라이언트의 플레이어라면, Enemy 태그를 붙이고, 어둡게 만듭니다.
        else if (PhotonNetwork.CurrentRoom != null)
        {
            gameObject.tag = "Enemy";
            spren.color = new Color(0.5f, 0.5f, 0.5f);
        }
        
    }
    
    public void Set(float hp, int quantity)
    {
        if (hp != 0) hitPoints.value = hp;
        else quantity = 0;
        for (int i = 0; i < quantity; i++) Instantiate(coinItem, transform.position, Quaternion.identity);
        isSkillCoolTime = false;
        barrierCooltime = 0.0f;
        if (rpgGameManager.gotCrown)
        {
            transform.GetChild(5).gameObject.SetActive(true);
        }
    }


    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            //서버 룸에 두명의 플레이어가 존재할 경우
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                //자신이 룸을 생성한 클라이언트라면, 침입당했으므로 attacked를 표시합니다.
                if (PhotonNetwork.IsMasterClient || attacked)
                {
                    attackedUI.SetActive(true);
                    attacked = true;
                }
                //자신이 룸에 참가했다면, 침입했으므로 invade를 표시합니다.
                else
                {
                    invadedUI.SetActive(true);
                    invading = true;
                }
            }
            //다른 플레이어에게 승리한 경우
            else if (attacked || invading)
            {
                attacked = false;
                invading = false;
                attackedUI.SetActive(false);
                invadedUI.SetActive(false);
                //코인 10개를 추가합니다.
                for (int i = 0; i < 10; i++) Instantiate(coinItem, transform.position, Quaternion.identity);
            }
        }
        //오프라인 플레이어의 경우 UI를 다시 숨깁니다.
        else
        {
            attacked = false;
            invading = false;
            attackedUI.SetActive(false);
            invadedUI.SetActive(false);
        }

        
        if (pv.IsMine || PhotonNetwork.CurrentRoom == null)
        {
            //총알을 발사하는 코드입니다.
            if (Input.GetButtonDown("Fire1") && (!movementController.isTalking &&
                                                 (EventSystem.current.currentSelectedGameObject == null ||
                                                  EventSystem.current.currentSelectedGameObject
                                                      .GetComponent<TMP_InputField>() == null)))
            {
                //발사할수 있는 총알이 남아있고, 재장전이 완료되었을 때 발사합니다.
                if ((4-shootedAmmoCnt) > 0 && reloadingTime == 0.0f)
                {
                    shootedAmmoCnt++;

                    FireBullet(aim[0].transform.position);
                    //다른 클라이언트에서도 발사합니다.
                    pv.RPC("FireBullet", RpcTarget.Others, aim[0].transform.position);
                }
            }
            //총알 UI를 업데이트합니다.
            bulletTextUI.text = ((4 - shootedAmmoCnt).ToString()) + "/4";

            //재장전하는 코드입니다.
            if (Input.GetKeyDown(KeyCode.R) && (!movementController.isTalking &&
                                                (EventSystem.current.currentSelectedGameObject == null ||
                                                 EventSystem.current.currentSelectedGameObject
                                                     .GetComponent<TMP_InputField>() == null))) reloading = true;
           //남아있는 총알이 없거나, 수동으로 R키를 입력했을 경우 재장전합니다.
            if ((4-shootedAmmoCnt) == 0 || reloading)
            {
                reloadEffect.SetActive(true);
                reloadingTime += Time.deltaTime;
                if (reloadingTime > 2.0f)
                {
                    reloadingTime = 0;
                    shootedAmmoCnt = 0;
                    reloadEffect.SetActive(false);
                    reloading = false;
                }
            }
            
            //배리어 스킬을 사용하는 코드입니다.
            if (Input.GetKeyDown(KeyCode.Space) && !barriering && !isSkillCoolTime && !(movementController.isTalking) &&
                (EventSystem.current.currentSelectedGameObject == null ||
                 EventSystem.current.currentSelectedGameObject.GetComponent<TMP_InputField>() == null))
            {
                barriering = true;
                isSkillCoolTime = true;
                barrierEffect.gameObject.SetActive(true);
                skillCoolImageGameObject.SetActive(true);
                coolTimeTextGameObject.SetActive(true);
                color = skillCoolImage.color;
            }
            if (barriering)
            {
                barrierTime += Time.deltaTime;
                if ((barrierTime > 2.0f))
                {
                    barriering = false;
                    barrierTime = 0.0f;
                    barrierEffect.gameObject.SetActive(false);
                }
            }
            //스킬 사용후 10초의 쿨타임을 적용합니다.
            if (isSkillCoolTime)
            {
                color.a = 0.9f - (barrierCooltime * 0.1f * 0.6f);
                skillCoolImage.color = color;
                coolTimeTextUI.text = (10.0f - barrierCooltime).ToString("F1");
                barrierCooltime += Time.deltaTime;
                if (barrierCooltime > 10.0f)
                {
                    isSkillCoolTime = false;
                    barrierCooltime = 0.0f;
                    skillCoolImageGameObject.SetActive(false);
                    coolTimeTextGameObject.SetActive(false);
                }
            }
        }
    }

///자신에게 데미지를 입히는 코루틴입니다.
    protected override IEnumerator DamageCharacter(int damage, float interval)
    {
        while (true)
        {
            if (!barriering)
            {
                DealedDamage(damage);
            }

            spren.color = new Color(1, 148 / 255.000f, 148 / 255.000f);

            if (hitPoints.value <= float.Epsilon)
            {
                spren.color = new Color(1, 1, 1);
                damageCoroutine = null;
                KillCharacter();
                break;
            }

            if (interval <= float.Epsilon)
            {
                spren.color = new Color(1, 1, 1);
                damageCoroutine = null;
                break;
            }
            else
            {
                yield return new WaitForSeconds(interval);
                if (damageCoroutine == null)
                {
                    spren.color = new Color(1, 1, 1);
                    break;
                }
            }
        }
    }

///캐릭터가 사망했을 경우 게임매니저에게 값을 전달합니다.
    public override void KillCharacter()
    {
        Destroy(healthBar.gameObject);
        Destroy(inventory.gameObject);

        if (hitPoints.value <= float.Epsilon)
        {
            if (PhotonNetwork.CurrentRoom != null) PhotonNetwork.LeaveRoom();
            deadCanvas[0].transform.GetChild(0).gameObject.SetActive(true);
        }

        rpgGameManager.hitpoints = hitPoints.value;
        if (inventory.items[0] == null)
        {
            rpgGameManager.quantity = 0;
        }
        else
        {
            rpgGameManager.quantity = inventory.items[0].quantity;
        }
        base.KillCharacter();
    }
///캐릭터를 초기화합니다.
    public override void ResetCharacter()
    {
        inventory = Instantiate(inventoryPrefab);
        healthBar = Instantiate(healthBarPrefab);
        healthBar.character = this;

        hitPoints.value = startingHitPoints;
    }
///데미지를 입을 경우 코루틴을 시작합니다.
    public override void TakeDamaged(int damage, float interval)
    {
        if (damageCoroutine == null)
        {
            damageCoroutine = StartCoroutine(DamageCharacter(damage, interval));
        }
    }
/// 주울수 있는 게임오브젝트를 줍습니다.
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CanBePickedUp") && (pv.IsMine || PhotonNetwork.CurrentRoom == null))
        {
            Item hitObject = collision.gameObject.GetComponent<ConsumableSOItem>().item;

            if (hitObject != null)
            {
                bool shouldDisappear = false;

                switch (hitObject.itemType)
                {
                    case Item.ItemType.COIN:
                        shouldDisappear = inventory.AddItem(hitObject);
                        break;
                    case Item.ItemType.HEALTH:
                        shouldDisappear = AdjustHitPoints(hitObject.quantity);
                        break;
                    default:
                        break;
                }

                if (shouldDisappear)
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }
/// 회복량만큼 HP의 최대치를 넘지 않게 회복합니다.
    private bool AdjustHitPoints(int amount)
    {
        if (hitPoints.value < maxHitPoints)
        {
            hitPoints.value = hitPoints.value + amount;
            return true;
        }
        
        return false;
    }

///다른 클라이언트에서 총알 발사시 현재 클라이언트에서 실행되어 총알을 발사합니다.
    [PunRPC]
    public void FireBullet(Vector3 position)
    {
        bool isplayer = false;
        if (gameObject.CompareTag("Player")) isplayer = true;

        //GameObject ins = Instantiate(ammo, transform.position, Quaternion.identity);

        GameObject ins = bulletPooling.SetGameObject();
        if (ins != null)
        {
            ins.transform.position = gameObject.transform.position;
            ins.GetComponent<Ammo>().Shoot(position, isplayer);
        }
    }

    [PunRPC]
    public void DealedDamage(int damage)
    {
        hitPoints.value = hitPoints.value - damage;
    }

    public void gotCrown()
    {
        rpgGameManager.gotCrown = true;
        transform.GetChild(5).gameObject.SetActive(true);
    }
}