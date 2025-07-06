using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.AStar;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour
{
    public ScheduleDataList_SO scheduleData;
    private SortedSet<ScheduleDetails> scheduleSet;
    private ScheduleDetails currentSchedule;


    [SerializeField] private string currentScene;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;

    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;
    public string StartScene { set => currentScene = value; }
    [Header("�ƶ�����")]
    public float normalSpeed = 2f;
    private float minSpeed = 1;
    private float maxSpeed = 3;
    private Vector2 dir;
    public bool isMoving;

    //Components
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Animator anim;

    private Grid grid;

    private Stack<MovementStep> movementSteps;

    private bool isInitialised;

    private bool npcMove;

    private bool sceneLoaded;
    private float animationBreakTime;

    private bool canPlayStopAnimation;

    private AnimationClip stopAnimationClip;
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;

    private TimeSpan GameTime => TimeManager.Instance.GameTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps = new Stack<MovementStep>();

        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride;
        scheduleSet = new SortedSet<ScheduleDetails>();
        foreach (var schedule in scheduleData.scheduleList)
        {
            scheduleSet.Add(schedule);
        }
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;

        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
    }

    

    private void Update()
    {
        if (sceneLoaded)
            SwitchAnimation();
        //��ʱ��
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTime <= 0;
    }
    private void FixedUpdate()
    {
        if (sceneLoaded)
            Movement();
    }
    private void OnGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        int time = (hour * 100) + minute;
        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet)
        {
            if(schedule.Time == time)
            {
                if (schedule.day != day && schedule.day != 0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if(schedule.Time > time)
            {
                break;
            }
        }
        if(matchSchedule != null)
        {
            BuildPath(matchSchedule);
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded = false;
    }

    
    private void OnAfterSceneLoadedEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckVisiable();
        if (!isInitialised)
        {
            InitNPC();
            isInitialised = true;
        }
        sceneLoaded = true;
    }

    private void CheckVisiable()
    {
        Debug.Log(currentScene);
        Debug.Log(SceneManager.GetActiveScene().name);
        if (currentScene == SceneManager.GetActiveScene().name)
        {
            SetActiveInScene();
        }
        else
        {
            SetInactiveInScene();
        }
    }
    private void InitNPC()
    {
        targetScene = currentScene;

        //�����ڵ�ǰ������������ĵ�
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition = currentGridPosition;
    }
    /// <summary>
    /// ��Ҫ�ƶ�����
    /// </summary>
    private void Movement()
    {
        if (!npcMove)
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();
                currentScene = step.sceneName;
                CheckVisiable();
                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);

                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if (!isMoving && canPlayStopAnimation)
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos,TimeSpan stepTime)
    {
        StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos,TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPosition(gridPos);
        //����ʱ�������ƶ�
        if(stepTime > GameTime)
        {
            //�����ƶ���ʱ������Ϊ��λ
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //ʵ���ƶ�����
            float distance = Vector3.Distance(transform.position, nextWorldPosition);

            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));

            if(speed <= maxSpeed)
            {
                while(Vector3.Distance(transform.position,nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition - transform.position).normalized;
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //���ʱ���Ѿ����˾�˲��
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;
    }



    /// <summary>
    /// ����Schedule����·��
    /// </summary>
    /// <param name="schedule"></param>
    public void BuildPath(ScheduleDetails schedule)
    {
        movementSteps.Clear();
        currentSchedule = schedule;
        targetScene = schedule.targetScene;
        targetGridPosition = (Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;
        if (schedule.targetScene == currentScene)
        {
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, schedule.targetGridPosition, movementSteps);
        }else if(schedule.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);
            if(sceneRoute != null)
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++)
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];
                    if(path.fromGridCell.x >= Settings.maxGridSize || path.fromGridCell.y >= Settings.maxGridSize)
                    {
                        fromPos = (Vector2Int)currentGridPosition;
                    }
                    else
                    {
                        fromPos = path.fromGridCell;
                    }
                    if (path.gotoGridCell.x >= Settings.maxGridSize || path.gotoGridCell.y >= Settings.maxGridSize)
                    {
                        gotoPos = schedule.targetGridPosition;
                    }
                    else
                    {
                        gotoPos = path.gotoGridCell;
                    }

                    AStar.Instance.BuildPath(path.sceneName, fromPos, gotoPos, movementSteps);
                }
            }
        }
        //TODO:�糡���ƶ�
        if (movementSteps.Count > 1)
        {
            //����ÿһ����Ӧ��ʱ���
            UpdateTimeOnPath();
        }
    }

    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;
        TimeSpan currentGameTime = GameTime;
        TimeSpan gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
        TimeSpan gridMovementDiagonalStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
        foreach (MovementStep step in movementSteps)
        {
            if (previousStep == null)
                previousStep = step;
            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;
            if (MoveInDiagonal(step, previousStep))
            {
                currentGameTime = currentGameTime.Add(gridMovementDiagonalStepTime);
            }
            else
            {
                currentGameTime = currentGameTime.Add(gridMovementStepTime);
            }
            previousStep = step;


        }
    }
    private bool MoveInDiagonal(MovementStep curretnStep, MovementStep previousStep)
    {
        return (curretnStep.gridCoordinate.x != previousStep.gridCoordinate.x) && (curretnStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }
    /// <summary>
    /// �������귵����������
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2);
    }


    private void SwitchAnimation()
    {
        isMoving = transform.position != GetWorldPosition(targetGridPosition);

        anim.SetBool("isMoving", isMoving);
        if (isMoving)
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
        {
            anim.SetBool("Exit", false);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        //ǿ������ͷ
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);

        animationBreakTime = Settings.animationBreakTime;
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }


    #region ����NPC��ʾ���
    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }
    #endregion
}
