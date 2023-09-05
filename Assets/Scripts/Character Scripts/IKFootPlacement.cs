using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKFootPlacement : MonoBehaviour
{
    [Header("Main")]
    [Range(0, 1)] public float Weight = 1f;
    [Header("Settings")]
    public float MaxStep = 0.5f;
    public float FootRadius = 0.15f;
    public LayerMask Ground = 1;
    public float Offset = 0f;
    [Header("Speed")]
    public float HipsPositionSpeed = 1f;
    public float FeetPositionSpeed = 2f;
    public float FeetRotationSpeed = 90;
    [Header("Weight")]
    [Range(0, 1)] public float HipsWeight = 0.75f;
    [Range(0, 1)] public float FootPositionWeight = 1f;
    [Range(0, 1)] public float FootRotationWeight = 1f;
    public bool ShowDebug = true;
    
    Vector3 LIKPosition, RIKPosition, LNormal, RNormal;
    Quaternion LIKRotation, RIKRotation, LastLeftRotation, LastRightRotation;
    float LastRFootHeight, LastLFootHeight;

    Animator Anim;
    float Velocity;
    float FalloffWeight;
    float LastHeight;
    Vector3 LastPosition;
    bool LGrounded, RGrounded, IsGrounded;

    
    void Start()
    {
        Anim = GetComponent<Animator>();
    }

       
    private void FixedUpdate()
    {
        if (Weight == 0 || !Anim) { return; }

        Vector3 Speed = (LastPosition - Anim.transform.position) / Time.fixedDeltaTime;
        Velocity = Mathf.Clamp(Speed.magnitude, 1, Speed.magnitude);
        LastPosition = Anim.transform.position;

        //��������� ������� �� ����
        FeetSolver(HumanBodyBones.LeftFoot, ref LIKPosition, ref LNormal, ref LIKRotation, ref LGrounded); //˳�� ����
        FeetSolver(HumanBodyBones.RightFoot, ref RIKPosition, ref RNormal, ref RIKRotation, ref RGrounded); //����� ����
        // �����������
        GetGrounded();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (Weight == 0 || !Anim) { return; }

        //������ ����
        MovePelvisHeight();
        //�� ��� ����
        MoveIK(AvatarIKGoal.LeftFoot, LIKPosition, LNormal, LIKRotation, ref LastLFootHeight, ref LastLeftRotation);
        //�� ����� ����
        MoveIK(AvatarIKGoal.RightFoot, RIKPosition, RNormal, RIKRotation, ref LastRFootHeight, ref LastRightRotation);

        //�� - ���������� ���������.
    }

    //������������ ������ ����
    private void MovePelvisHeight()
    {
        //��������� ������
        float LeftOffset = LIKPosition.y - Anim.transform.position.y;
        float RightOffset = RIKPosition.y - Anim.transform.position.y;
        float TotalOffset = (LeftOffset < RightOffset) ? LeftOffset : RightOffset;
        //��������� ������� ������
        Vector3 NewPosition = Anim.bodyPosition;
        float NewHeight = TotalOffset * (HipsWeight * FalloffWeight);
        LastHeight = Mathf.MoveTowards(LastHeight, NewHeight, HipsPositionSpeed * Time.deltaTime);
        NewPosition.y += LastHeight + Offset;
        //������������ ������
        Anim.bodyPosition = NewPosition;
    }

    //����
    void MoveIK(AvatarIKGoal Foot, Vector3 IKPosition, Vector3 Normal, Quaternion IKRotation, ref float LastHeight, ref Quaternion LastRotation)
    {
        Vector3 Position = Anim.GetIKPosition(Foot);
        Quaternion Rotation = Anim.GetIKRotation(Foot);

        //�������
        Position = Anim.transform.InverseTransformPoint(Position);
        IKPosition = Anim.transform.InverseTransformPoint(IKPosition);
        LastHeight = Mathf.MoveTowards(LastHeight, IKPosition.y, FeetPositionSpeed * Time.deltaTime);
        Position.y += LastHeight;

        Position = Anim.transform.TransformPoint(Position);
        Position += Normal * Offset;

        //�����
        Quaternion Relative = Quaternion.Inverse(IKRotation * Rotation) * Rotation;
        LastRotation = Quaternion.RotateTowards(LastRotation, Quaternion.Inverse(Relative), FeetRotationSpeed * Time.deltaTime);

        Rotation *= LastRotation;

        //������������ ��
        Anim.SetIKPosition(Foot, Position);
        Anim.SetIKPositionWeight(Foot, FootPositionWeight * FalloffWeight);
        Anim.SetIKRotation(Foot, Rotation);
        Anim.SetIKRotationWeight(Foot, FootRotationWeight * FalloffWeight);
    }

    void GetGrounded()
    {
        //������������ ����
        IsGrounded = LGrounded || RGrounded;
        //��������� MainWeight ���� ���� ����������  
        FalloffWeight = LerpValue(FalloffWeight, IsGrounded ? 1f : 0f, 1f, 10f, Time.fixedDeltaTime) * Weight;
    }


    public float LerpValue(float Current, float Desired, float IncreaseSpeed, float DecreaseSpeed, float DeltaTime)
    {
        if (Current == Desired) return Desired;
        if (Current < Desired) return Mathf.MoveTowards(Current, Desired, (IncreaseSpeed * Velocity) * DeltaTime);
        else return Mathf.MoveTowards(Current, Desired, (DecreaseSpeed * Velocity) * DeltaTime);
    }


    //����'������ ��
    private void FeetSolver(HumanBodyBones Foot, ref Vector3 IKPosition, ref Vector3 Normal, ref Quaternion IKRotation, ref bool Grounded)
    {
        Vector3 Position = Anim.GetBoneTransform(Foot).position;
        Position.y = Anim.transform.position.y + MaxStep;

        //������ ��������
        RaycastHit Hit;
        //������ ����
        Position -= Normal * Offset;
        float FeetHeight = MaxStep;

        if (ShowDebug)
            Debug.DrawLine(Position, Position + Vector3.down * (MaxStep * 2), Color.yellow);

        if (Physics.SphereCast(Position, FootRadius, Vector3.down, out Hit, MaxStep * 2, Ground))
        {
            //������� (������)
            FeetHeight = Anim.transform.position.y - Hit.point.y;
            IKPosition = Hit.point;
            //������� (����)
            Normal = Hit.normal;
            if (ShowDebug)
                Debug.DrawRay(Hit.point, Hit.normal, Color.blue);
            //����� (�������)
            Vector3 Axis = Vector3.Cross(Vector3.up, Hit.normal);
            float Angle = Vector3.Angle(Vector3.up, Hit.normal);
            IKRotation = Quaternion.AngleAxis(Angle, Axis);
        }

        Grounded = FeetHeight < MaxStep;

        if (!Grounded)
        {
            IKPosition.y = Anim.transform.position.y - MaxStep;
            IKRotation = Quaternion.identity;
        }
    }
}